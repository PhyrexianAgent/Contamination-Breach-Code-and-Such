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

    const int SEARCHES_MAX = 2;
    const float TIME_BETWEEN_SEARCHES = 4;
    const float SEARCH_RADIUS = 4;

    public int pos;
    public float speed;
    public GameObject sprite;
    public float attackDamage = 15;
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
    private int searchesLeft = 0;
    private bool onTheWay = false;
    
    
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
                if ((agent.remainingDistance < 2f && !playerFound) || agent.isStopped)//agent.velocity.magnitude < 0.01 && accelerated) //for some reason when sound causes a loop for walking, anim never plays (have no clue why)
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
        //agent.velocity = Vector3.zero;
        //agent.isStopped = false;
        currentTarget = targetPos;
        agent.SetDestination(currentTarget);
        agent.isStopped = false;
    }

    bool GetRandInRadius()
    {
        bool succeeded = false;
        Vector2 testPos = currentTarget + (Random.insideUnitCircle * SEARCH_RADIUS);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(testPos, out hit, SEARCH_RADIUS, 1))
        {
            currentTarget = hit.position;
            succeeded = true;
        }
        return succeeded;
    }

    void FollowTarget()
    {
        if (GetRandInRadius())
        {
            agent.SetDestination(currentTarget);
            agent.isStopped = false;
        }

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
        if ((currentState != animName && currentState != DEAD) || !anim.GetCurrentAnimatorStateInfo(0).IsName(animName))
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
        
        if (collision.gameObject.tag == "Player Attack")
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
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerFound)
        {
            Debug.Log("lost player");
            playerFound = false;
            agent.isStopped = true;
            DoAnimation(IDLE);
            PlayerVarsToSave.beingChased = false;
            targetColl = null;
            onTheWay = false;
        }
    }

    void DoSearch()
    {/*
        if (searchesLeft > 0 && !onTheWay)
        {
            FollowTarget();
            searchesLeft--;
            Invoke("DoSearch", TIME_BETWEEN_SEARCHES);
        }
        else if (!onTheWay) 
        {
            agent.SetDestination(startingPos);
        }*/
        agent.SetDestination(startingPos);
        Debug.Log(startingPos + " " + transform.position);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Sound" && currentState != DEAD)
        {
            FollowTarget(collision.transform.position);
            DoAnimation(WALK);
            searchesLeft = SEARCHES_MAX;
            Invoke("DoSearch", TIME_BETWEEN_SEARCHES+1);
            onTheWay = true;
        }
        else if (collision.gameObject.tag == "Player Area" && currentState != DEAD)
        {
            playerFound = true;
            DoAnimation(WALK);
            targetColl = collision;
            FollowTarget(collision.transform.position);
            collision.transform.parent.gameObject.GetComponent<PlayerCode>().isBeingChased = true;
            onTheWay = true;
        }
    }

    void EndWalk()
    {
        DoAnimation(IDLE);
    }

    public void Attack()
    {
        DoAnimation(ATTACK);
        sprite.GetComponent<BoxCollider2D>().enabled = false;
        Invoke("ResetColl", 2);
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
