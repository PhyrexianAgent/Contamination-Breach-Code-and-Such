using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCode : MonoBehaviour //future note, the rifle reload is very slightly off but may not be noticable
{
    const string STATE_IDLE = "Idle"; //for these constants, they reference parts of each animation name. each name should be WEAPON STATE to work with these
    const string STATE_RUNNING = "Running";
    const string STATE_ATTACK = "Attack";
    const string STATE_RELOAD = "Reload";

    const string WEAPON_KNIFE = "Knife";
    const string WEAPON_HAND_GUN = "Hand Gun";
    const string WEAPON_RIFLE = "Rifle";
    const string WEAPON_SHOT_GUN = "Shot Gun";

    const float HAND_GUN_DAMAGE = 34;

    public GameObject camera;
    public GameObject flashlight;
    public GameObject sprite;
    public Animator animPlayer;
    public float speed = 1000; //note this number is not correct since it is different in unity editor
    public Transform shootPoint;

    private Rigidbody2D rigidbody;
    
    private string currentState = STATE_IDLE;
    private string currentWeapon = WEAPON_HAND_GUN;
    private bool isAttacking = false;
    private bool isReloading = false;
    private Vector2 handGunShootPos = new Vector2(0.55f, 1.17f);

    private Ray2D basicShootRay;// = new Ray2D(handGunShootPos);

    private bool wasShot = false;
    private Vector2 endPoint;
    private Vector2 startPoint;
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animPlayer = sprite.GetComponent<Animator>();
        basicShootRay = new Ray2D(handGunShootPos, Vector2.left);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -1); //keeps camra at player's position (if making that child of player, then camera will rotate with player)
        RotateToMouse();
        TestForInput();
        if (wasShot)
        {
            Debug.DrawLine(startPoint, endPoint, Color.red);
        }
    }

    void TestForInput() //tests for various inputs to do various things not related to movement
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
        {
            ShootGunNormal();
            ChangeCurrentAnim(STATE_ATTACK);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentWeapon == WEAPON_KNIFE)
            {
                SwitchWeapon(WEAPON_HAND_GUN);
            }
            else
            {
                SwitchWeapon(WEAPON_KNIFE);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeCurrentAnim(STATE_RELOAD);
        }
    }

    Vector2 GetDirectionToMouse()
    {
        //Vector2 mousePos = transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //return (mousePos - (Vector2)transform.position).normalized;
        //Debug.Log(transform.rotation.eulerAngles);
        float angle = Mathf.Deg2Rad * (transform.rotation.eulerAngles.z + 90);
        Vector2 newPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return newPos.normalized;
    }

    void ShootGunNormal()
    {
        Vector2 direction = GetDirectionToMouse();
        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, direction, Mathf.Infinity, 3);


        if (hit.collider != null)
        {
            //Debug.DrawLine(basicShootRay.origin, hit.collider.transform.position, Color.white);
            if (hit.collider.gameObject.tag == "Enemy")
            {
                hit.transform.gameObject.GetComponent<zombieController>().WasShot();
            }
            
        }
        //endPoint = hit.point;//transform.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        //startPoint = transform.position;
        //wasShot = true;
    }

    void MovePlayer() //uses input from player to generate normalized velocity (numbers always between 0 and 1). will then multiply that by speed and Time.deltaTime
    {
        bool[] inputTests = new bool[] { Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S) };
        Vector2 velocityLocal = new Vector2(Convert.ToInt32(inputTests[1]) - Convert.ToInt32(inputTests[0]), Convert.ToInt32(inputTests[2]) - Convert.ToInt32(inputTests[3]));
        rigidbody.velocity = velocityLocal.normalized * speed;

        if (!isAttacking && !isReloading)
        {
            if (velocityLocal == Vector2.zero)
            {
                ChangeCurrentAnim(STATE_IDLE);
            }
            else
            {
                ChangeCurrentAnim(STATE_RUNNING);
            }
        }
        
    }

    void ChangeCurrentAnim(string newState)
    {
        string oldState = currentState;
        if (currentState != newState)
        {
            currentState = newState;
            if (currentState != STATE_RELOAD || currentWeapon != WEAPON_KNIFE)
            {
                animPlayer.Play(currentWeapon + " " + currentState);
            }
            if (newState == STATE_ATTACK)
            {
                isAttacking = true;
                Invoke("OnEndAttack", ChangeTimeFromWeapon(animPlayer.GetCurrentAnimatorStateInfo(0).length, oldState));
            }
            else if (newState == STATE_RELOAD && currentWeapon != WEAPON_KNIFE)
            {
                isReloading = true;
                Debug.Log("doing anim");
                Invoke("OnEndReload", ChangeTimeFromWeapon(animPlayer.GetCurrentAnimatorStateInfo(0).length, oldState));
            }
        }
    }

    float GetProperTime()
    {
        AnimatorStateInfo currentState = animPlayer.GetCurrentAnimatorStateInfo(0);
        float lengthBase = currentState.length;
        float playSpeed = currentState.speed;
        return lengthBase - ((lengthBase * playSpeed) - lengthBase);
    }

    float ChangeTimeFromWeapon(float time, string oldState)
    {
        float subVal = 0;
        switch (currentWeapon)
        {
            case WEAPON_KNIFE:
                subVal = oldState == STATE_IDLE ? 1f : 0.6f;
                break;
            case WEAPON_HAND_GUN:
                subVal = currentState != STATE_RELOAD ? 1.3f : 0.5f;
                break;
            case WEAPON_SHOT_GUN:
            case WEAPON_RIFLE:
                subVal = currentState == STATE_RELOAD ? 0.5f : 1.5f;
                break;
        }
        return time - subVal;
    }

    void RotateToMouse()
    {
        Vector3 mousePos = camera.GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);//Input.mousePosition;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, mousePos - transform.position);
    }

    void OnEndAttack()
    {
        isAttacking = false;
        ChangeCurrentAnim(STATE_IDLE);
    }

    void OnEndReload()
    {
        isReloading = false;
        ChangeCurrentAnim(STATE_IDLE);
        Debug.Log("getting to end");
    }

    void SwitchWeapon(string newWeapon)
    {
        currentWeapon = newWeapon;
        animPlayer.Play(currentWeapon+" "+currentState);
       
    }

    //.505 .53
}
