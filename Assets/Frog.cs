using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frog : MonoBehaviour
{

    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    [SerializeField] private float jumpLenght=3f;
    [SerializeField] private float jumpHeight=4f;
    [SerializeField] private LayerMask ground;


    private Collider2D collider;
    private Rigidbody2D rb;
    private Animator anim;

    private bool facingLeft = true;

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(anim.GetBool("Jumping"))
        {
            if(rb.velocity.y < .1)
            {
                anim.SetBool("Jumping", false);
                anim.SetBool("Falling", true);
            }          
        }
        if(anim.GetBool("Falling") && collider.IsTouchingLayers(ground))
        {
            anim.SetBool("Falling", false);
            anim.SetBool("Jumping", false);
        }

    }

    private void FrogJump()
    {
        if (facingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)
                {
                    transform.localScale = new Vector3(1, 1);
                }

                if (collider.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(-jumpLenght, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)
                {
                    transform.localScale = new Vector3(-1, 1);
                }

                if (collider.IsTouchingLayers(ground))
                {
                    rb.velocity = new Vector2(jumpLenght, jumpHeight);
                    anim.SetBool("Jumping", true);
                }
            }
            else
            {
                facingLeft = true;
            }
        }
    }

    public void DeadAnim()
    {
        anim.SetTrigger("Death");
    }

    private void Delete()
    {
        Destroy(this.gameObject);
    }
}
