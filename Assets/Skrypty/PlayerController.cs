using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    public int health;
    public int maxHealth = 100;
    public HealthBar healthBar;
    [HideInInspector] public bool canClimb = false;
    [HideInInspector] public bool bottomLadder = false;
    [HideInInspector] public bool topLadder = false;
    public Ladder ladder;
    private float naturalGravity;
    //[SerializeField]  
    private Animator anim;
    public int level;
    private enum State {idle, running, jumping, falling, hurt, crouching, climb}
    private State state = State.idle;
    private Collider2D collider;
    public GameObject gameOverUI;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] private float speed = 7f;
    [SerializeField] private float jumpforce = 10f;
    [SerializeField] private int cherries = 0;
    [SerializeField] private Text showCountCherry;
    [SerializeField] private float hurtForce = 1f;

    public Transform groundCheck;
    public float groundCheckRadius;
    public LayerMask whatIsGround;
    public bool grouned;

    private void FixedUpdate()
    {
        grouned = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
    }

    // Start is called before the first frame update
    private void Start()
    {
        Time.timeScale = 1f;
        level = SceneManager.GetActiveScene().buildIndex;
        health = maxHealth;
        healthBar.SetMaxHealth( health );
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        collider = GetComponent<Collider2D>();
        naturalGravity = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update()
    {
        if(state == State.climb)
        {
            Climb();
        }
        else if(state != State.hurt)
        {
            Movement();
        }        
        AnimationSwitch();
        anim.SetInteger("state", (int)state);

    }

    public void SavePlayer()
    {
        SaveSystem.SavePlayer(this);
    }

    public void LoadPlayer()
    {
        
        PlayerData data = SaveSystem.LoadPlayer();
        SceneManager.LoadScene(data.level);
        Start();
        Update();
        /*
        level = data.level;
        health = data.health;

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];

        transform.position = position;
        */
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Cherry")
        {
            FindObjectOfType<AudioManager>().Play("coin");
            Destroy(collision.gameObject);
            cherries++;
            showCountCherry.text = cherries.ToString();
        }
        else if(collision.tag == "Spike")
        {
            damage(health);
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }
    }

    void damage( int dam ){
        health -= dam;
        healthBar.SetHealth( health );
        if (health == 0)
        {
            Time.timeScale = 0f;
            gameOverUI.SetActive(true);
        }//resetScene();
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
                FindObjectOfType<AudioManager>().Play("damageEnemy");
                enemy.DeadAnim();
                Debug.Log("ERRor");
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

        if(canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
        {
            state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(ladder.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;
        }

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
        else if (grouned)
        { 
            rb.velocity = new Vector2(0, rb.velocity.y); 
        }

        if (Input.GetButtonDown("Jump") && grouned)
        {
            Jump();
        }
    }

    private void AnimationSwitch()
    {
        if(state == State.climb)
        {

        }
        else if(state == State.jumping)
        {
            if(rb.velocity.y < 0)
            {
                state = State.falling;
            }

        }
        else if (state == State.falling)
        {
            if (grouned)
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
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
        FindObjectOfType<AudioManager>().Play("jump");
        rb.velocity = new Vector2(rb.velocity.x, jumpforce);
        state = State.jumping;

    }

    private void Climb()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            anim.speed = 1f;
            Jump();
            return;
        }
        float vDirection = Input.GetAxis("Vertical");

        //Climbing up
        if(vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }
        //Climbing Down
        else if(vDirection < -.1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
            anim.speed = 1f;
        }
        //Still
        else
        {
            anim.speed = 0f;
            rb.velocity = Vector2.zero;
        }
    }

}
