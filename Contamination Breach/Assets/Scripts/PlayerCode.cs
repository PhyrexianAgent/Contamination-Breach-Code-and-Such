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
    const int SHOT_GUN_SPREAD_COUNT = 15;
    const float SHOT_GUN_SPREAD_DIFF = 25;
    const float KNIFE_DAMAGE_SNEAK = 100;
    const float KNIFE_DAMAGE_NORMAL = 50;
    const float RIFLE_DAMAGE = 25;
    const float SHOT_GUN_DAMAGE = 30;

    struct ShotRay
    {
        public Vector2 startPoint, endPoint;
    }

    public GameObject camera;
    public GameObject flashlight;
    public GameObject sprite;
    public Animator animPlayer;
    public float speed = 1000; //note this number is not correct since it is different in unity editor
    public Transform shootPoint;
    public GameObject[] soundColls;
    public CircleCollider2D sprintingColl;
    public BoxCollider2D knifeAttackArea;

    public bool isBeingChased = false;
    public float currentDamage = KNIFE_DAMAGE_NORMAL;

    private Rigidbody2D rigidbody;
    
    private string currentState = STATE_IDLE;
    private string currentWeapon = WEAPON_KNIFE;
    private bool isAttacking = false;
    private bool isReloading = false;
    private Vector2 handGunShootPos = new Vector2(0.55f, 1.17f);

    private Ray2D basicShootRay;// = new Ray2D(handGunShootPos);

    private bool wasShot = false;
    private ShotRay[] shots = new ShotRay[SHOT_GUN_SPREAD_COUNT];
    private Vector2[] shotPositions = { new Vector2(1.2f, -0.55f), new Vector2(1.4f, -0.5f)};
    private bool isSprinting = false;
    private List<string> collectedCards = new List<string>();
    
    


    
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animPlayer = sprite.GetComponent<Animator>();
        basicShootRay = new Ray2D(handGunShootPos, Vector2.left);
        knifeAttackArea.enabled = false;
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
            foreach (ShotRay ray in shots)
            {
                Debug.DrawLine(ray.startPoint, ray.endPoint, Color.red);
            }
        }
    }

    void ShootShotGun()
    {
        float[] angles = { transform.rotation.eulerAngles.z + SHOT_GUN_SPREAD_DIFF, transform.rotation.eulerAngles.z - SHOT_GUN_SPREAD_DIFF };
        float randAngle;
        RaycastHit2D hit;
        for (int i=0; i<SHOT_GUN_SPREAD_COUNT; i++)
        {
            randAngle = UnityEngine.Random.Range(angles[0], angles[1]);
            hit = ShootGunNormal(randAngle);
            shots[i].endPoint = hit.point;
            shots[i].startPoint = shootPoint.position;
        }
        wasShot = true;
    }

    public void CollectCard(string code)
    {
        collectedCards.Add(code);
    }

    public bool HasCard(string code)
    {
        return collectedCards.IndexOf(code) > -1;
    }

    void DisableColls()
    {
        foreach (GameObject coll in soundColls)
        {
            coll.GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    void EmitAttackSound()
    {
        switch (currentWeapon)
        {
            case WEAPON_HAND_GUN:
                soundColls[0].GetComponent<CircleCollider2D>().enabled = true;
                break;
            case WEAPON_RIFLE:
            case WEAPON_SHOT_GUN:
                soundColls[1].GetComponent<CircleCollider2D>().enabled = true;
                break;
        }
        Invoke("DisableColls", 0.3f);
    }

    void KnifeAttack()
    {
        knifeAttackArea.enabled = true;
        Invoke("DisableKnifeAttack", 0.4f);
    }

    void DisableKnifeAttack()
    {
        knifeAttackArea.enabled = false;
    }

    void TestForInput() //tests for various inputs to do various things not related to movement
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading)
        {
            if (currentWeapon == WEAPON_SHOT_GUN)
            {
                ShootShotGun();
                EmitAttackSound();
            }
            else if (currentWeapon != WEAPON_KNIFE)
            {
                ShootGunNormal();
                EmitAttackSound();
            }
            else
            {
                Invoke("KnifeAttack", 0.25f);
                //KnifeAttack();
            }
            ChangeCurrentAnim(STATE_ATTACK);
            
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentWeapon == WEAPON_KNIFE)
            {
                SwitchWeapon(WEAPON_RIFLE);
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
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        sprintingColl.enabled = isSprinting || isBeingChased;
    }

    float GetDamageToDeal()
    {
        float foundDamage = 0;
        switch (currentWeapon)
        {
            case WEAPON_HAND_GUN:
                foundDamage = HAND_GUN_DAMAGE;
                break;
            case WEAPON_KNIFE:
                foundDamage = KNIFE_DAMAGE_NORMAL;
                break;
            case WEAPON_RIFLE:
                foundDamage = RIFLE_DAMAGE;
                break;
            case WEAPON_SHOT_GUN:
                foundDamage = SHOT_GUN_DAMAGE;
                break;
        }
        return foundDamage;
    }

    Vector2 GetShootDirection(float angleDeg)
    {
        float angle = Mathf.Deg2Rad * (angleDeg + 90);
        Vector2 newPos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        return newPos.normalized;
    }

    RaycastHit2D ShootGunNormal(float angle = -1)
    {
        Vector2 direction = GetShootDirection(angle != -1 ? angle : transform.rotation.eulerAngles.z);
        RaycastHit2D hit = Physics2D.Raycast(shootPoint.position, direction, Mathf.Infinity, 3);


        if (hit.collider != null)
        {
            if (hit.collider.gameObject.tag == "Enemy")
            {
                hit.transform.gameObject.GetComponent<zombieController>().TakeDamage(currentDamage);
            }
            
        }
        return hit;
    }

    void MovePlayer() //uses input from player to generate normalized velocity (numbers always between 0 and 1). will then multiply that by speed and Time.deltaTime
    {
        bool[] inputTests = new bool[] { Input.GetKey(KeyCode.A), Input.GetKey(KeyCode.D), Input.GetKey(KeyCode.W), Input.GetKey(KeyCode.S) };
        Vector2 velocityLocal = new Vector2(Convert.ToInt32(inputTests[1]) - Convert.ToInt32(inputTests[0]), Convert.ToInt32(inputTests[2]) - Convert.ToInt32(inputTests[3]));
        rigidbody.velocity = velocityLocal.normalized * (speed * (isSprinting ? 2 : 1));

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
    }

    void SwitchWeapon(string newWeapon)
    {
        currentWeapon = newWeapon;
        animPlayer.Play(currentWeapon+" "+currentState);
        switch (currentWeapon)
        {
            case WEAPON_KNIFE:
            case WEAPON_HAND_GUN:
                shootPoint.localPosition = shotPositions[0];
                break;
            case WEAPON_RIFLE:
            case WEAPON_SHOT_GUN:
                shootPoint.localPosition = shotPositions[1];
                break;
        }
        //flashlight.transform.localPosition = shootPoint.localPosition;
        currentDamage = GetDamageToDeal();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        zombieController enemyScript;
        if (collision.gameObject.tag == "Attack Area")
        {
            enemyScript = collision.gameObject.transform.parent.GetComponent<zombieController>();
            enemyScript.Attack();
            enemyScript.sprite.GetComponent<BoxCollider2D>().enabled = false;
            enemyScript.Invoke("ResetColl", 1);
            isBeingChased = true;
        }
    }

    //.505 .53
}
