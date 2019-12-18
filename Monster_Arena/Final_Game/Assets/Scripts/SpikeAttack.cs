using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeAttack : MonoBehaviour
{
    public GameObject realSpikyBoi;


    void Start()
    {
        StartCoroutine(SpikeSpawn());
    }

    private IEnumerator SpikeSpawn()
    {
        yield return new WaitForSeconds(2f);

        Instantiate(realSpikyBoi, new Vector3(transform.position.x, -0.11f, transform.position.z), transform.rotation);

        yield return new WaitForSeconds(1f);

        Destroy(this.gameObject);
    }
  


}
