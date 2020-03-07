using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected Animator anim;
    protected Rigidbody2D rb;

    protected virtual void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }
    public void DeadAnim()
    {
        anim.SetTrigger("Death");
       // rb.velocity = Vector2.zero;
    }

    private void Delete()
    {
        Destroy(this.gameObject);
    }

}
