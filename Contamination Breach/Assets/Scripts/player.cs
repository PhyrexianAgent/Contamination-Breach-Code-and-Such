using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public GameObject[] sprite;
    private Rigidbody2D prisoner;
    public float speed;
    SpriteRenderer spriteAnimation;
    private float timer=0 ;
    float horiz, vert;
    void Start()
    {
        prisoner = GetComponent<Rigidbody2D>();
      
        spriteAnimation = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
      horiz =  Input.GetAxis("Horizontal");
        vert = Input.GetAxis("Vertical");
        prisoner.velocity = new Vector2(horiz*speed, vert*speed);

      
       



    }
   
    
}
