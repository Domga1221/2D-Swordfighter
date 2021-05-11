using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{

    public float moveRate = 5;

    public bool faceRight = false;

    public Animator anim;

    Rigidbody2D rb;

    bool grounded = true;


    public Transform attackPoint;
    public float attackRange = 0.43f;
    public LayerMask enemyLayer;

    public Vector3 attackHitKnockback;
    public float knockBackMultiplier = 5f;

    public float nextEvade;
    public float evadeTime;


    float horizontal = -1f;

    public bool invincible;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        for (int i = 0; i < anim.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (anim.runtimeAnimatorController.animationClips[i].name == "Enemy_evade")
            {
                evadeTime = anim.runtimeAnimatorController.animationClips[i].length;
                //Debug.Log(evadeTime);
            }
        }

       
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = 0;
        // Vertical Movement
        /*
        float horizontal = Input.GetAxis("Horizontal");

        transform.position += new Vector3(horizontal, 0, 0) * Time.deltaTime * moveRate;
        */

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            horizontal = -1;
            transform.position += new Vector3(-1, 0, 0) * Time.deltaTime * moveRate;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            horizontal = 1;
            transform.position += new Vector3(1, 0, 0) * Time.deltaTime * moveRate;
        }

        // Jumping
        if (Input.GetKeyDown(KeyCode.Keypad1) && grounded)
        {
            rb.AddForce(new Vector2(0, 7), ForceMode2D.Impulse);
            anim.SetBool("Jumping", true);
            grounded = false;
        }

        // Combat
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            anim.SetBool("StartAttacking", true);
        }
        if (Input.GetKeyUp(KeyCode.Keypad4))
        {
            anim.SetBool("StartAttacking", false);
            anim.SetTrigger("Attack");

            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

            foreach (Collider2D enemy in hitEnemies)
            {
                if (!enemy.GetComponent<PlayerMovement>().invincible)
                {
                    Debug.Log("We hit " + enemy.name);

                    // direction calculation
                    attackHitKnockback = enemy.GetComponent<Transform>().position - transform.position;
                    // upwards movement
                    attackHitKnockback.y = attackHitKnockback.y + 1;
                    // multiply with factor
                    attackHitKnockback = attackHitKnockback * knockBackMultiplier;

                    enemy.GetComponent<Rigidbody2D>().AddForce(attackHitKnockback, ForceMode2D.Impulse);
                }
            }
        }

        // Debug.Log(Time.time + " " +nextEvade);
        if (Input.GetKeyDown(KeyCode.Keypad5) && Time.time > nextEvade)
        {
            //Debug.Log("Enemy collider disabled");
            // GetComponent<BoxCollider2D>().enabled = false;
            invincible = true;
            nextEvade = Time.time + evadeTime;
            anim.SetTrigger("Evade");
        }

        
        if (Time.time >= nextEvade)
        {
            //Debug.Log("Enemy collider activated");
            // GetComponent<BoxCollider2D>().enabled = true;
            invincible = false;
        }
        


        // Change Facing
        if (horizontal > 0 && faceRight == false)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
            faceRight = true;
        }
        if (horizontal < 0 && faceRight == true)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
            faceRight = false;
        }


        // Animations
        if (horizontal != 0 && grounded)
        {
            anim.SetBool("Running", true);
        }
        else if (grounded)
        {
            anim.SetBool("Running", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("enter");
        grounded = true;
        anim.SetBool("Jumping", false);
    }


    // Draw AttackPoint Radius
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}
