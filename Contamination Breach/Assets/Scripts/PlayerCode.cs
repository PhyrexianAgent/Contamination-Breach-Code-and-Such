using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCode : MonoBehaviour
{
    public Rigidbody2D rigidbodySelf;
    public GameObject cameraFollowing;
    public float speedSelf = 1000;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangePlayerVelocity();
        cameraFollowing.transform.position = new Vector3(transform.position.x, transform.position.y, -1); //keeps camra at player's position (if making that child of player, then camera will rotate with player)
    }

    void ChangePlayerVelocity() //uses input from player to generate normalized velocity (numbers always between 0 and 1). will then multiply that by speed and Time.deltaTime
    {
        bool[] inputTests = new bool[] { Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S) };
        Vector2 velocityLocal = new Vector2(Convert.ToInt32(inputTests[1]) - Convert.ToInt32(inputTests[0]), Convert.ToInt32(inputTests[2]) - Convert.ToInt32(inputTests[3]));
        if (inputTests[0] || inputTests[1] || inputTests[2] || inputTests[3]) 
        {
            rigidbodySelf.velocity = velocityLocal.normalized * speedSelf;
            
            transform.rotation = Quaternion.identity;
        }
        else if (rigidbodySelf.velocity != new Vector2(0, 0)) //makes sure velocities from collisions don't cause any movement
        {
            rigidbodySelf.velocity = new Vector2(0, 0);
        }
    }
}
