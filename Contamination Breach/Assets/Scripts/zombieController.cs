using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    const string WALK = "Zombie Walk";
    const string IDLE = "Zombie Idle";
    const string DEAD = "Zombie Dead";
    const string ATTACK = "Attack";
    //const string DOWN = "Down";
    //const string UP = "Up";
    const string LEFT = "Left";
    const string RIGHT = "Right";

    public int pos;
    public float speed;
    public GameObject sprite;
    public float attackDamage = 20;
    public Sprite deadImage;

    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    private NavMeshAgent agent;
    private string currentState = IDLE;
    private string currentDirection = LEFT;
    private Vector2 startingPos;
    private Vector2 currentTarget;
    private Vector2 oldPos;
    private float health = 100;

    private bool accelerated = false;
    
    
    void Start()
    {
       
        //rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        anim = sprite.GetComponent<Animator>();
        //anim.enabled = false;
        startingPos = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case WALK:
                if (agent.remainingDistance < 0.01f)//agent.velocity.magnitude < 0.01 && accelerated) //for some reason when sound causes a loop for walking, anim never plays (have no clue why)
                {
                    DoAnimation(IDLE);
                    accelerated = false;
                }
                else if (agent.velocity.magnitude > 0.1)
                {
                    accelerated = true;
                }
                oldPos = transform.position;
                SetRotation();
                break;
            case ATTACK:
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(ATTACK))
                {
                    currentState = WALK;
                }
                break;
        }
    }

    public void WasShot(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Debug.Log("is dead");
            DoAnimation(DEAD);
            Invoke("MakeSelfDead", anim.GetCurrentAnimatorStateInfo(0).length - 0.85f);
            agent.isStopped = true;
            GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    void DoAnimation(string animName)
    {
        if (currentState != animName && currentState != DEAD)
        {
            if (animName == DEAD)
                Debug.Log("doing dead anim");
            anim.Play(animName);
            currentState = animName;
            
        }
    }

    void MakeSelfDead()
    {
        anim.enabled = false;
        sprite.GetComponent<SpriteRenderer>().sprite = deadImage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Sound" && currentState != DEAD)
        {
            currentTarget = collision.transform.position;
            DoAnimation(WALK);
            agent.SetDestination(currentTarget);
        }
        else
        {
            Debug.Log(collision.gameObject.tag);
        }
    }

    public void Attack()
    {
        DoAnimation(ATTACK);
    }

    void SetRotation()
    {
        bool flip = agent.velocity.x < 0;
        Vector2 offset = sprite.GetComponent<BoxCollider2D>().offset;
        sprite.GetComponent<SpriteRenderer>().flipX = flip;
        sprite.GetComponent<BoxCollider2D>().offset = new Vector2(flip ? -offset.x : offset.x, offset.y);
    }


}
