using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collide : MonoBehaviour
{
    public string tagToDestroy;
    public Boss boss;
    private float damage = 0.05f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag(tagToDestroy))
        {
            if(this.gameObject.CompareTag("WeaponSwing") && tagToDestroy == "Boss")
            {
                if(!boss.immune)
                    boss.Hit(damage);
            }

            else if(this.gameObject.CompareTag("BossAttack") && tagToDestroy == "Player")
            {
                if(!collision.GetComponent<PlayerController>().immune)
                    collision.GetComponent<PlayerController>().LoseHealth();

                collision.GetComponent<PlayerController>().pState.recoilingX = true;
            }

            else if (this.gameObject.CompareTag("Boss") && tagToDestroy == "Player")
            {
                if (!collision.GetComponent<PlayerController>().immune)
                    collision.GetComponent<PlayerController>().LoseHealth();

                collision.GetComponent<PlayerController>().pState.recoilingX = true;
            }



        }


    }







}
