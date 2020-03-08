using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    public int health;
    public int maxHealth = 100;
    public HealthBar healthBar;
    //[SerializeField]  
    private Animator anim;
    private enum State {idle, running, jumping, falling, hurt, crouching}
    private State state = State.idle;
    private Collider2D collider;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text showCountCherry;
    [SerializeField] private float hurtForce = 1f;


    // Start is called before the first frame update
    private void Start()
    {
        health = maxHealth;
        healthBar.SetMaxHealth( health );
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if(state != State.hurt)
        {
            Movement();
        }        
        AnimationSwitch();
        anim.SetInteger("state", (int)state);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Cherry")
        {
            Destroy(collision.gameObject);
            cherries++;
            showCountCherry.text = cherries.ToString();
        }
    }

    void damage( int dam ){
        health -= dam;
        healthBar.SetHealth( health );
        if( health == 0 )
            resetScene();
    }

    void resetScene(){
        health = maxHealth;
        healthBar.SetMaxHealth( health );
        SceneManager.LoadScene( SceneManager.GetActiveScene().name );
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enemy" )
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if(state == State.falling)
            {
                enemy.DeadAnim();
                Jump();
            }
            else
            {
                damage( 20 );
                state = State.hurt;
                if( collision.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy z prawej
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);                   

                }
                else
                {
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
            
        }
    }


    private void Movement()
    {
        float hDirection = Input.GetAxis("Horizontal");

        if (hDirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }

        else if (hDirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        if (Input.GetButtonDown("Jump") && collider.IsTouchingLayers(ground))
        {
            Jump();
        }
    }

    private void AnimationSwitch()
    {
        if(state == State.jumping)
        {
            if(rb.velocity.y < 0)
            {
                state = State.falling;
            }

        }
        else if (state == State.falling)
        {
            if (collider.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
        else if(Mathf.Abs(rb.velocity.x) > 2f)
        {
            state = State.running;
        }
        else
        {
            state = State.idle;
        }

    }


    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;
    }

}
