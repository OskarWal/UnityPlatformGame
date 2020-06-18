using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    public float jumpForce;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        anim.SetBool("Trampoline", true);
        other.GetComponent<Rigidbody2D>().AddForce(new Vector2(2, jumpForce));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        anim.SetBool("Trampoline", false);
    }
}