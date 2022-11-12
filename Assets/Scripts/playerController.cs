using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;


    [Header("----- Player Stats -----")]
    [Range(0, 10)] [SerializeField] int HP;
    [Range(1, 5)] [SerializeField] float playerSpeed;
    [Range(1.5f, 5)] [SerializeField] float sprintMod;
    [Range(8, 20)] [SerializeField] float jumpHeight;
    [Range(0, 50)] [SerializeField] float gravityValue;
    [Range(1, 3)] [SerializeField] int jumpsMax;


    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] GameObject gunModel;
    [SerializeField] GameObject hitEffect;
    [SerializeField] List<gunStats> gunStatList = new List<gunStats>();

    int origHP;
    Vector3 move;
    private Vector3 playerVelocity;
    int jumpsTimes;
    bool isSprinting;
    bool isShooting;
    float playerSpeedOrig;
    int selectedGun;

    private void Start()
    {
        playerSpeedOrig = playerSpeed;
        origHP = HP;
        respawn();
        updatePlayerHPBar();
    }

    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
        gunSelect();
    }

    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpsTimes = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") +
               transform.forward * Input.GetAxis("Vertical");

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpsTimes < jumpsMax)
        {
            jumpsTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        if(Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed = playerSpeedOrig;
            isSprinting = false;
        }
    }

    IEnumerator shoot()
    {
        if (!gameManager.instance.isPaused && gunStatList.Count > 0 && isShooting == false && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist) && shootDamage > 0)
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDamage);
                }

                Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
            }

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void damage(int dmg)
    {
        HP -= dmg;
        updatePlayerHPBar();

        StartCoroutine(gameManager.instance.playerDamageFlash());

        if(HP <= 0)
        {
            gameManager.instance.playerDeadMenu.SetActive(true);
            gameManager.instance.pauseGame();
        }
    }

    void updatePlayerHPBar()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)origHP;
    }

    public void gunPickup(gunStats gunStat)
    {
        shootRate = gunStat.shootRate;
        shootDist = gunStat.shootDist;
        shootDamage = gunStat.shootDamage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        gunStatList.Add(gunStat);
    }

    void gunSelect()
    {
        if(gunStatList.Count > 1)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunStatList.Count - 1)
            {
                selectedGun++;
                changeGuns();
            }
            else if(Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                changeGuns();
            }
        }
    }

    void changeGuns()
    {
        shootRate = gunStatList[selectedGun].shootRate;
        shootDist = gunStatList[selectedGun].shootDist;
        shootDamage = gunStatList[selectedGun].shootDamage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStatList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStatList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void respawn()
    {
        controller.enabled = false;
        HP = origHP;
        updatePlayerHPBar();
        transform.position = gameManager.instance.spawnPos.transform.position;
        gameManager.instance.playerDeadMenu.SetActive(false);
        controller.enabled = true;
    }
}