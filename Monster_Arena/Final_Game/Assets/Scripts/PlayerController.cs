using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public PlayerStateList pState;
    public GameObject frontHit, aboveHit, belowHit, slash;
    private bool spawnDelay = true;


    [Header("X-Axis Movement")]

    public float walkSpeed;

    [Space(3)]

    [Header("Y-Axis Movement")]

    public float jumpSpeed, fallSpeed;
    public int jumpSteps, jumpThreshold;
    private readonly int maxJumps = 2;
    public int jumps = 0;

    [Space(3)]

    [Header("Attacking")]

    public float timeBetweenAttack;
    public Transform attackTran;
    public float attackRadius;
    public Transform downAttackTran;
    public float downAttackRadius;
    public Transform upAttackTran;
    public float upAttackRadius;
    public LayerMask attackableLayer;

    [Space(3)]

    [Header("Recoil")]

    public int recoilXSteps;
    public int recoilYSteps;
    public float recoilXSpeed, recoilYSpeed;

    [Space(3)]

    [Header("Ground Checking")]

    public Transform groundTran;
    public float groundCheckY, groundCheckX;
    public LayerMask groundLayer;

    [Space(3)]

    [Header("Roof Checking")]

    public Transform roofTran;
    public float roofCheckY, roofCheckX;

    [Space(3)]


    private float timeSinceAtk, xAxis, yAxis, gravity;
    private int stepsXRecoiled = 0, stepsYRecoiled = 0, stepsJumped = 0;
    public int health;
    public bool immune;

    Rigidbody2D rb;
    // public Animator anim;


    private void Start()
    {
        if(pState == null)
        {
            pState = GetComponent<PlayerStateList>();
        }

        rb = GetComponent<Rigidbody2D>();
        frontHit.SetActive(false);
        aboveHit.SetActive(false);
        belowHit.SetActive(false);
        gravity = rb.gravityScale;

        StopRecoilX();
        StopRecoilY();

        StartCoroutine(SpawnDelay());
    }

    private void Update()
    {
        if (!spawnDelay)
        {
            GetInputs();

            Flip();
            Walk(xAxis);
            Recoil();
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if(pState.recoilingX == true && stepsXRecoiled < recoilXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if(pState.recoilingY == true && stepsYRecoiled < recoilYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }


        if(Grounded())
        {
            StopRecoilY();
        }

        Jump();


    }

    void Jump()
    {
        // allows player to jump (multiple times)
        if(pState.jumping)
        {
            if(stepsJumped < jumpSteps && !Roofed())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
                stepsJumped++;
            }
            else
            {
                StopJumpSlow();
            }
        }

        // limits falling speed
        if(rb.velocity.y < -Mathf.Abs(fallSpeed))
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y, -Mathf.Abs(fallSpeed), Mathf.Infinity));
        }

    }

    void Walk(float moveDirection)
    {
        if(!pState.recoilingX)
        {
            rb.velocity = new Vector2(moveDirection * walkSpeed, rb.velocity.y);

            if(Mathf.Abs(rb.velocity.x) > 0)
            {
                pState.walking = true;
            }
            else
            {
                pState.walking = false;
            }

            if(xAxis > 0)
            {
                pState.lookingRight = true;
            }
            else if(xAxis < 0)
            {
                pState.lookingRight = false;
            }

        }


    }

    void Attack()
    {
        timeSinceAtk += Time.deltaTime;

        if(Input.GetButtonDown("Attack") && timeSinceAtk >= timeBetweenAttack)
        {
            timeSinceAtk = 0;

            // actual attack
            if(yAxis == 0 || yAxis < 0 && Grounded())
            {
                Collider2D[] objectsToHit = Physics2D.OverlapCircleAll(attackTran.position, attackRadius, attackableLayer);
                frontHit.SetActive(true);
                Instantiate(slash, attackTran.position, attackTran.rotation);

                if(objectsToHit.Length > 0)
                {
                    pState.recoilingX = true;
                }

                // execution for enemies that are hit
                /*
            // execution for enemies that are hit
            for (int i = 0; i < objectsToHit.Length; i++)
            {
                if (!(objectsToHit[i].GetComponent<Boss>() == null))
                {
                    objectsToHit[i].GetComponent<Boss>().Hit(damage);
                }
            }
            */
            }

            // attack above
            else if (yAxis > 0)
            {
                Collider2D[] objectsToHit = Physics2D.OverlapCircleAll(upAttackTran.position, upAttackRadius, attackableLayer);
                aboveHit.SetActive(true);
                Instantiate(slash, upAttackTran.position, upAttackTran.rotation);

                if (objectsToHit.Length > 0)
                {
                    pState.recoilingY = true;
                }

                // execution for enemies that are hit
                /*
                 // execution for enemies that are hit
                 for (int i = 0; i < objectsToHit.Length; i++)
                 {
                     if (!(objectsToHit[i].GetComponent<Boss>() == null))
                     {
                         objectsToHit[i].GetComponent<Boss>().Hit(damage);
                     }
                 }
                 */

            }

            //attack down
            else if (yAxis < 0 && !Grounded())
            {
                Collider2D[] objectsToHit = Physics2D.OverlapCircleAll(downAttackTran.position, downAttackRadius, attackableLayer);
                belowHit.SetActive(true);
                Instantiate(slash,downAttackTran.position, downAttackTran.rotation);

                if (objectsToHit.Length > 0)
                {
                    pState.recoilingY = true;
                }
                /*
                // execution for enemies that are hit
                for (int i = 0; i < objectsToHit.Length; i++)
                {
                    if (!(objectsToHit[i].GetComponent<Boss>() == null))
                    {
                        objectsToHit[i].GetComponent<Boss>().Hit(damage);
                    }
                }
                */

            }

            StartCoroutine(AttackHitBoxDelay());


        }







    }

    private  IEnumerator AttackHitBoxDelay()
    {

        yield return new WaitForSeconds(1f);

        frontHit.SetActive(false);
        aboveHit.SetActive(false);
        belowHit.SetActive(false);
    }

    void Recoil()
    {
        if(pState.recoilingX)
        {
            if(pState.lookingRight)
            {
                rb.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rb.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if(pState.recoilingY)
        {
            if(yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, recoilYSpeed);
                rb.gravityScale = 0;
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -recoilYSpeed);
                rb.gravityScale = 0;
            }
        }
        else
        {
            rb.gravityScale = gravity;
        }

    }

    void Flip()
    {
        if(xAxis > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
        else if(xAxis < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }

    }

    // stops the player's jump immediately when button is released
    void StopJumpQuick()
    {
        stepsJumped = 0;
        pState.jumping = false;
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    // stops jump and lets player hang in air briefly
    void StopJumpSlow()
    {
        stepsJumped = 0;
        pState.jumping = false;
    }

    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    // see if the player is on the ground with raycasts
    public bool Grounded()
    {
        if (Physics2D.Raycast(groundTran.position, Vector2.down, groundCheckY, groundLayer) || Physics2D.Raycast(groundTran.position + new Vector3(-groundCheckX, 0), Vector2.down, groundCheckY, groundLayer) || Physics2D.Raycast(groundTran.position + new Vector3(groundCheckX, 0), Vector2.down, groundCheckY, groundLayer))
        {
            jumps = 0;
            return true;
        }
        else
        {
            return false;
        }
    }

    // see if player is touching roof and cancels jump if true
    public bool Roofed()
    {
        if (Physics2D.Raycast(roofTran.position, Vector2.up, roofCheckY, groundLayer) || Physics2D.Raycast(roofTran.position + new Vector3(roofCheckX, 0), Vector2.up, roofCheckY, groundLayer) || Physics2D.Raycast(roofTran.position + new Vector3(roofCheckX, 0), Vector2.up, roofCheckY, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GetInputs()
    {
        // WASD or Joystick
        yAxis = Input.GetAxis("Vertical");
        xAxis = Input.GetAxis("Horizontal");

        // sensitivity
        if(yAxis > 0.25)
        {
            yAxis = 1;
        }
        else if (yAxis < -0.25)
        {
            yAxis = -1;
        }
        else
        {
            yAxis = 0;
        }

        if (xAxis > 0.25)
        {
            xAxis = 1;
        }
        else if (xAxis < -0.25)
        {
            xAxis = -1;
        }
        else
        {
            xAxis = 0;
        }

        // jumping
        if(Input.GetButtonDown("Jump") && Grounded() || Input.GetButtonDown("Jump") && jumps < maxJumps)
        {
            pState.jumping = true;
            jumps++;
        }
        if (!Input.GetButton("Jump") && stepsJumped < jumpSteps && stepsJumped > jumpThreshold && pState.jumping)
        {
            StopJumpQuick();
        }
        else if (!Input.GetButton("Jump") && stepsJumped < jumpThreshold && pState.jumping)
        {
            StopJumpSlow();
        }

    }

    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackTran.position, attackRadius);
        Gizmos.DrawWireSphere(downAttackTran.position, downAttackRadius);
        Gizmos.DrawWireSphere(upAttackTran.position, upAttackRadius);

        Gizmos.DrawLine(groundTran.position, groundTran.position + new Vector3(0, -groundCheckY));
        Gizmos.DrawLine(groundTran.position + new Vector3(-groundCheckX, 0), groundTran.position + new Vector3(-groundCheckX, -groundCheckY));
        Gizmos.DrawLine(groundTran.position + new Vector3(groundCheckX, 0), groundTran.position + new Vector3(groundCheckX, -groundCheckY));

        Gizmos.DrawLine(roofTran.position, roofTran.position + new Vector3(0, roofCheckY));
        Gizmos.DrawLine(roofTran.position + new Vector3(-roofCheckX, 0), roofTran.position + new Vector3(-roofCheckX, roofCheckY));
        Gizmos.DrawLine(roofTran.position + new Vector3(roofCheckX, 0), roofTran.position + new Vector3(roofCheckX, roofCheckY));
    }
    


    public void LoseHealth()
    {
        SceneManager.LoadScene("EndScreen");
    }

    private IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(14);

        spawnDelay = false;
    }










}
