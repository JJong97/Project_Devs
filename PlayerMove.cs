using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerMove : MonoBehaviour
{
    public GameObject Player;
    Animator animator;

    public float jumpPower;
    public float speed;
    public float defaultSpeed;
    private bool isdash;
    public float dashSpeed;
    public float defaultTime;
    private float dashTime;
    bool inputJump = false;

    Rigidbody2D rigid2D;
    Collider2D col2D;

    private float curTime;
    public float coolTime = 0.5f;
    public Transform pos;
    public Vector2 boxSize;

    public float atkSpeed = 1;
    public bool attacked = false;

    //여러번 공격 방지용
    void AttackTrue()
    {
        attacked = true;
    }
    void AttackFalse()
    {
        attacked = false;
    }
    void SetAttackSpeed(float speed)
    {
        animator.SetFloat("attackSpeed", speed);
        atkSpeed = speed;
    }

    void Start()
    {
        Player.transform.position = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        rigid2D = GetComponent<Rigidbody2D>();
        col2D = GetComponent<Collider2D>();
        defaultSpeed = speed;

        SetAttackSpeed(1.5f);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.localScale = new Vector3(1, 1, 1);
            animator.SetBool("isMove", true);
            transform.Translate(Vector3.right * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.localScale = new Vector3(-1, 1, 1);
            animator.SetBool("isMove", true);
            transform.Translate(Vector3.left * Time.deltaTime);
        }
        else animator.SetBool("isMove", false);

        
            if (curTime <= 0)
            {
                //공격
                if (Input.GetKey(KeyCode.A)) //&&
                //!animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))          
            {

                Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(pos.position, boxSize, 0);
                foreach (Collider2D collider in collider2Ds)
                {
                    if(collider.tag == "Enemy")
                    {
                        collider.GetComponent<Enemy>().TakeDamage(1);
                    }
                }
                animator.SetTrigger("isAttack");
                curTime = coolTime;
            }
               
            }
            else
            {
                curTime -= Time.deltaTime;
            }

        if (Input.GetKey(KeyCode.DownArrow) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Down"))
        {
            animator.SetTrigger("isDown");
        }

        if (Input.GetKeyDown(KeyCode.S) &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("Slide"))
        {
            animator.SetTrigger("isSlide");
        }

            RaycastHit2D raycastHit = Physics2D.BoxCast(col2D.bounds.center, col2D.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null)
            animator.SetBool("isJump", false);
        else animator.SetBool("isJump", true);

        if (Input.GetKeyDown(KeyCode.Space) &&
            !animator.GetBool("isJump"))
        {
            inputJump = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(pos.position, boxSize);
    }
    void FixedUpdate()
    {
        float hor = Input.GetAxis("Horizontal");
        rigid2D.velocity = new Vector2(hor * defaultSpeed, rigid2D.velocity.y);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            defaultSpeed = 2;
        }
        else
        {
            defaultSpeed = speed;
        }

        if(Input.GetKeyDown(KeyCode.D))
            
        {
            animator.SetTrigger("isDash");
            isdash = true;
        }

        if(dashTime <= 0)
        {
            //defaultSpeed = speed;
            if (isdash)
                dashTime = defaultTime;
        }
        else
        {
            dashTime -= Time.deltaTime;
            defaultSpeed = dashSpeed;
        }
        isdash = false;

        if (inputJump)
        {
            inputJump = false;
            rigid2D.AddForce(Vector2.up * jumpPower);
            //rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpPower);
        }
    }
}