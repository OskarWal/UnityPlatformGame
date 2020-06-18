using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opossum : Enemy
{
    public float speed;
    [SerializeField] private float leftCap;
    [SerializeField] private float rightCap;
    public Transform groundDetect;
    public bool movingLeft = true;
    private Collider2D collider;
    // Update is called once per frame
    protected override void Start()
    {
        base.Start();
        collider = GetComponent<Collider2D>();
    }
    void Update()
    {
        if (movingLeft)
        {
            if (transform.position.x > leftCap)
            {
                if (transform.localScale.x != 1)//6)
                {
                    transform.localScale = new Vector3(1, 1);// 6, 5);
                }
                transform.Translate(new Vector3(-1, 0, 0) * speed * Time.deltaTime);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightCap)
            {
                if (transform.localScale.x != -1)//-6)
                {
                    transform.localScale = new Vector3(-1, 1);// -6, 5);
                }
                transform.Translate(new Vector3(1, 0, 0) * speed * Time.deltaTime);
            }
            else
            {
                movingLeft = true;
            }
        }

    }
}
