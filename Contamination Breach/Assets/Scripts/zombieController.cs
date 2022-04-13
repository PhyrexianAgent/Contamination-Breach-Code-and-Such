using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    public int pos;
    public float speed;
    public Transform target;

    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    private NavMeshAgent agent;

    
    
    void Start()
    {
       
        //rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

       
    }

    // Update is called once per frame
    void Update()
    {
        agent.updateRotation = false;
        agent.updateUpAxis = false;
        agent.SetDestination(target.position);
    }
    //void Walk()
    //{
    //    rb.velocity = new Vector2(0f, 1f * speed);
    //    anim.SetBool("isWalking", true);
    //}

    //public void WalkDown()
    //{
//
 //       rb.velocity = new Vector2(0f, -1f * speed);
      

//    }
    //public void OnCollisionEnter2D(Collision2D other)
    //{
     //   if (other.gameObject.tag == "MR")
     //   {
//
 //          
  //          this.anim.SetBool("isWalking", false);
   //         this.anim.SetBool("down", true);
    //        this.WalkDown();
     //   }
      //  else if (other.gameObject.tag == "MU")
       // {
        //    this.anim.SetBool("down", false);
         //   this.anim.SetBool("isWalking", true);
          //  Walk();

//        }
//
  //  }


}
