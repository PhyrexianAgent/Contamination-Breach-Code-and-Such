using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCode : MonoBehaviour
{
    
    public GameObject camera;
    public GameObject flashlight;
    public GameObject sprite;
    public Animator animPlayer;
    public float speed = 1000; //note this number is not correct since it is different in unity editor

    private enum States { idle, running, attacking }

    private Rigidbody2D rigidbody;
    

    private int currentState = (int)States.idle;
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animPlayer = sprite.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -1); //keeps camra at player's position (if making that child of player, then camera will rotate with player)
        RotateToMouse();
        TestForInput();
        ActionsFromState();
    }

    void ActionsFromState()
    {
        switch (currentState)
        {
            case (int)States.attacking:
                if (!animPlayer.GetBool("attacking"))
                    currentState = (int)States.idle;
                break;
        }
    }

    void TestForInput() //tests for various inputs to do various things not related to movement
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            currentState = (int)States.attacking;
            animPlayer.SetTrigger("attack");
        }
    }

    void MovePlayer() //uses input from player to generate normalized velocity (numbers always between 0 and 1). will then multiply that by speed and Time.deltaTime
    {
        bool[] inputTests = new bool[] { Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S) };
        Vector2 velocityLocal = new Vector2(Convert.ToInt32(inputTests[1]) - Convert.ToInt32(inputTests[0]), Convert.ToInt32(inputTests[2]) - Convert.ToInt32(inputTests[3]));
        rigidbody.velocity = velocityLocal.normalized * speed;
        //if (currentState == (int)States.attacking)
          //  Debug.Log("entered here");
        if (velocityLocal == Vector2.zero && currentState == (int)States.running)
        {
            currentState = (int)States.idle;
        }
        else if (currentState == (int)States.idle && velocityLocal != Vector2.zero)
        {
            currentState = (int)States.running;
        }
        animPlayer.SetInteger("currentState", currentState);
    }

    void RotateToMouse()
    {
        Vector3 mousePos = camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);//Input.mousePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }

    public void OnAttackEnd()
    {
        Debug.Log("got here");
        currentState = (int)States.idle;
        animPlayer.SetInteger("currentState", currentState);
    }
}
