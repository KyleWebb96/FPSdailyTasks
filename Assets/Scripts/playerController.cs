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

    Vector3 move;
    private Vector3 playerVelocity;
    int jumpsTimes;
    bool isSprinting;
    bool isShooting;
    float playerSpeedOrig;

    private void Start()
    {
        playerSpeedOrig = playerSpeed;
    }

    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
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
        if (isShooting == false && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if(Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDamage);
                }

            }


            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void damage(int dmg)
    {
        HP -= dmg;

    }
}