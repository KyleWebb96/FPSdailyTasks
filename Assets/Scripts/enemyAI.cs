using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----- Components -----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator anim;

    [Header("----- Enemy Stats -----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int speedChase;
    [SerializeField] int sightDist;
    [SerializeField] int sightAngle;
    [SerializeField] int roamDist;
    [SerializeField] int animLerpSpeed;
    [SerializeField] GameObject headPos;

    [Header("----- Gun Stats -----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;

    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;
    float stoppingDistOrig;
    Vector3 startingPos;
    float speedOrig;

    

    // Start is called before the first frame update
    void Start()
    {
        speedOrig = agent.speed;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        gameManager.instance.enemiesToKill++;
        gameManager.instance.updateUI();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animLerpSpeed));

        if(agent.enabled)
        {
            if (playerInRange)
            {
                canSeePlayer();
            }
            else if (agent.remainingDistance < 0.1f && agent.destination != gameManager.instance.player.transform.position)
            {
                roam();
            }
        }
    }

    void canSeePlayer()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.transform.position);
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        RaycastHit hit;
        if(Physics.Raycast(headPos.transform.position, playerDir, out hit))
        {
            Debug.Log(hit.transform.name);
            Debug.DrawRay(headPos.transform.position, playerDir);

            if(hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;
                agent.SetDestination(gameManager.instance.player.transform.position);

                if(agent.remainingDistance < agent.stoppingDistance)
                {
                    facePlayer();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;

        Vector3 randomDir = Random.insideUnitSphere * roamDist;
        randomDir += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(new Vector3(randomDir.x, 0, randomDir.z), out hit, 1, 1);
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }

    void facePlayer()
    {
        playerDir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        agent.stoppingDistance = 0;
        StartCoroutine(flashDamage());
        agent.SetDestination(gameManager.instance.player.transform.position);

        if (HP <= 0)
        {
            gameManager.instance.updateEnemyNumber();
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        model.material.color = Color.white;

    }

    IEnumerator shoot()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
