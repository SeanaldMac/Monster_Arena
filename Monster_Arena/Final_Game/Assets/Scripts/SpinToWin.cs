using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinToWin : MonoBehaviour
{
    [SerializeField] private float spinner = 0;
    public float spinDir;

    void Start()
    {
        StartCoroutine(Spin());
    }

    private IEnumerator Spin()
    {
        while (true)
        { 
        yield return new WaitForSeconds(.05f);
        transform.Rotate(new Vector3(0, 0, spinner));
        spinner += spinDir;
        }
    }
}
