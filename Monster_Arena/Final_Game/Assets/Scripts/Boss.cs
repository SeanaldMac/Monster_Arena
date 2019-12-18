using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Boss : MonoBehaviour
{
    private float startX = 5.12f;
    private bool startDelay = true, faceDelay = true;
    public float riseSpeed, riseDistance;
    public float health;
    public bool immune;
    public GameObject slimeSpike, slimeFist;

    public HealthBar bossHealthBar;
    public GameObject hBar;
    private int attackNum; // choose which attack is used

    private void Start()
    {
        bossHealthBar.SetSize(health);

        StartCoroutine(RiseUp());
        StartCoroutine(AttackPattern());
    }

    public void Hit(float damage)
    {
        health -= damage;
        if (health < 0)
            health = 0;
        if (health != 0)
            bossHealthBar.SetSize(health);
        else
        {
            hBar.SetActive(false);
            Destroy(this);
            SceneManager.LoadScene("EndScreen");
        }


        StartCoroutine(Immunity());
    }

    // rise to end position over time
    public IEnumerator RiseUp()
    {
        immune = true;

        if(startDelay)
        {
            yield return new WaitForSeconds(2f);
            startDelay = false;
        }

        while (transform.position.y < -0.23)
        {
            if(faceDelay && transform.position.y > -5.73)
            {
                yield return new WaitForSeconds(2f);
                riseSpeed = 0.02f;
                faceDelay = false;
            }

            transform.position = new Vector2(startX, transform.position.y + riseDistance);

            yield return new WaitForSeconds(riseSpeed);
        }

        immune = false;
    }

    public IEnumerator Immunity()
    {
        immune = true;
        yield return new WaitForSeconds(.4f);
        immune = false;
    }

    public IEnumerator AttackPattern()
    {
        yield return new WaitForSeconds(14f);

        while(health > .5f)
        {
            attackNum = Random.Range(1, 3);

            if(attackNum == 1)
            {
                Instantiate(slimeFist, new Vector3(3.55f, Random.Range(-2.78f, 0.30f), -0.16f), new Quaternion(0,0,0,0));
            }
            if(attackNum == 2)
            {
                Instantiate(slimeSpike, new Vector3(1.72f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
                Instantiate(slimeSpike, new Vector3(-1.26f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
                Instantiate(slimeSpike, new Vector3(-4.24f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
            }


            yield return new WaitForSeconds(4f);
        }

        while(health <= .5f && health > 0)
        {
            attackNum = Random.Range(1, 3);

            if (attackNum == 1)
            {
                Instantiate(slimeFist, new Vector3(3.55f, Random.Range(-2.78f, 0.30f), -0.16f), new Quaternion(0, 0, 0, 0));
                Instantiate(slimeFist, new Vector3(3.55f, Random.Range(-2.78f, 1f), -0.16f), new Quaternion(0, 0, 0, 0));
            }
            if (attackNum == 2)
            {
                Instantiate(slimeSpike, new Vector3(1.72f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
                Instantiate(slimeSpike, new Vector3(-1.26f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
                Instantiate(slimeSpike, new Vector3(-4.24f, -4, -0.16f), new Quaternion(0, 0, 0, 0));
            }

            yield return new WaitForSeconds(3f);
        }


    }




}
