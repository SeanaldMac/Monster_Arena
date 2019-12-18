using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundries : MonoBehaviour
{
    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("BossAttack"))
             Destroy(collision.gameObject);


    }




}
