using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCode : MonoBehaviour
{
    
    public GameObject camera;
    public GameObject flashlight;
    public GameObject sprite;
    public float speed = 1000; //note this number is not correct since it is different in unity editor

    private Rigidbody2D rigidbody;
    private Animator animPlayer;
    private bool isMoving = false;

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
    }

    void MovePlayer() //uses input from player to generate normalized velocity (numbers always between 0 and 1). will then multiply that by speed and Time.deltaTime
    {
        bool[] inputTests = new bool[] { Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S) };
        Vector2 velocityLocal = new Vector2(Convert.ToInt32(inputTests[1]) - Convert.ToInt32(inputTests[0]), Convert.ToInt32(inputTests[2]) - Convert.ToInt32(inputTests[3]));
        rigidbody.velocity = velocityLocal.normalized * speed;
        isMoving = velocityLocal != Vector2.zero;
        animPlayer.SetBool("isIdle", !isMoving);
        animPlayer.SetBool("isRunning", isMoving);
    }

        void RotateToMouse()
    {
        Vector3 mousePos = camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);//Input.mousePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }
}
