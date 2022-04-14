using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class zombieController : MonoBehaviour
{
    const string WALK = "Zombie Walk";
    const string IDLE = "Idle";
    const string DOWN = "Down";
    const string UP = "Up";
    const string LEFT = "Left";
    const string RIGHT = "Right";

    public int pos;
    public float speed;
    public GameObject sprite;

    // Start is called before the first frame update
    private Rigidbody2D rb;
    private Animator anim;
    private NavMeshAgent agent;
    private string currentState = IDLE;
    private string currentDirection = LEFT;
    private Vector2 startingPos;
    private Vector2 currentTarget;
    
    
    void Start()
    {
       
        //rb = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;

        anim = sprite.GetComponent<Animator>();
        anim.enabled = false;
        startingPos = transform.position;
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void WasShot()
    {
        Debug.Log("was hit");
    }


}
