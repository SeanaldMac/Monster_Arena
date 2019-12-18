using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargeSpikeDespawn : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(SpikeSpawn());
    }

    private IEnumerator SpikeSpawn()
    {   
        yield return new WaitForSeconds(1f);

        Destroy(this.gameObject);
    }
}
