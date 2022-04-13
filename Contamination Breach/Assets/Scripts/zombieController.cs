using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombieController : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    public int pos;
    public float speed;
    
    void Start()
    {
       
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        walk();
       
    }

    // Update is called once per frame
    void Update()
    {
       
    }
    void walk()
    {
       
        
        
            rb.velocity = new Vector2(0f, 1f * speed);
            anim.SetBool("isWalking", true);
       
      
       
        
    }

    public void WalkDown()
    {

        rb.velocity = new Vector2(0f, -1f * speed);
      

    }
    public void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "MR")
        {

           
            this.anim.SetBool("isWalking", false);
            this.anim.SetBool("down", true);
            this.WalkDown();
        }
        else if (other.gameObject.tag == "MU")
        {
            this.anim.SetBool("down", false);
            this.anim.SetBool("isWalking", true);
            walk();

        }

    }


}
