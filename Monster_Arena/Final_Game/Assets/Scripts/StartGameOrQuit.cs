using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGameOrQuit : MonoBehaviour
{
   

   
    void Update()
    {
        if (Input.GetButtonDown("Restart"))
        {
            SceneManager.LoadScene("BossBattle");
        }

        else if (Input.GetButtonDown("Cancel"))
            Application.Quit();
    }
}
