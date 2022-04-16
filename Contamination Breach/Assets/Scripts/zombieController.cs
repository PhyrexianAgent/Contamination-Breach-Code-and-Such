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
    public CircleCollider2D deathSound;

    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    private NavMeshAgent agent;
    private string currentState = IDLE;
    private string currentDirection = LEFT;
    private Vector2 startingPos;
    private Vector2 currentTarget;
    private Vector2 oldPos;
    public float health = 100;
    private Collider2D targetColl;
    private bool playerFound = false;
    
    
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
                    agent.isStopped = true;
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
        if (playerFound)
        {
            FollowTarget(targetColl.transform.position);
            DoAnimation(WALK);
        }
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(currentState))
        {
            DoAnimation(currentState);
        }
    }

    void FollowTarget(Vector2 targetPos)
    {
        currentTarget = targetPos;
        agent.SetDestination(currentTarget);
        agent.isStopped = false;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            DoAnimation(DEAD);
            Invoke("MakeSelfDead", anim.GetCurrentAnimatorStateInfo(0).length - 0.85f);
            agent.isStopped = true;
            GetComponent<BoxCollider2D>().enabled = false;
            deathSound.enabled = true;
            Debug.Log("died");
        }
    }

    void DoAnimation(string animName)
    {
        if ((currentState != animName && currentState != DEAD) || anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
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
        deathSound.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.tag == "Sound" && currentState != DEAD)
        {
            FollowTarget(collision.transform.position);
            DoAnimation(WALK);
        }
        else if (collision.gameObject.tag == "Player Attack")
        {
            if (!playerFound)
            {
                TakeDamage(100);
            }
            else
            {
                TakeDamage(collision.transform.parent.gameObject.GetComponent<PlayerCode>().currentDamage);
            }
        }
        else if (collision.gameObject.tag == "Player Area" && currentState != DEAD)
        {
            playerFound = true;
            DoAnimation(WALK);
            targetColl = collision;
            FollowTarget(collision.transform.position);
            collision.transform.parent.gameObject.GetComponent<PlayerCode>().isBeingChased = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerFound)
        {
            playerFound = false;
            agent.isStopped = true;
            DoAnimation(IDLE);
            Invoke("EndWalk", 1f);
            collision.transform.parent.gameObject.GetComponent<PlayerCode>().isBeingChased = false;
            targetColl = null;
        }
    }

    void EndWalk()
    {
        DoAnimation(IDLE);
    }

    public void Attack()
    {
        DoAnimation(ATTACK);
    }

    void ResetColl()
    {
        sprite.GetComponent<BoxCollider2D>().enabled = true;
    }

    void SetRotation()
    {
        bool flip = agent.velocity.x < 0;
        Vector2 offset = sprite.GetComponent<BoxCollider2D>().offset;
        sprite.GetComponent<SpriteRenderer>().flipX = flip;
        sprite.GetComponent<BoxCollider2D>().offset = new Vector2(flip ? -offset.x : offset.x, offset.y);
    }


}
