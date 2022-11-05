using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{

    [SerializeField] gunStats gunStat;

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.gunPickup(gunStat);
            Destroy(gameObject);
        }
    }

}
