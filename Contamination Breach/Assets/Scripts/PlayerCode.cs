using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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

    const float HAND_GUN_DAMAGE = 40;
    const int SHOT_GUN_SPREAD_COUNT = 15;
    const float SHOT_GUN_SPREAD_DIFF = 25;
    const float KNIFE_DAMAGE_SNEAK = 100;
    const float KNIFE_DAMAGE_NORMAL = 50;
    const float RIFLE_DAMAGE = 25;
    const float SHOT_GUN_DAMAGE = 30;
    const float RELOAD_TIME = 1.7f;
    const float STEP_MAX_NUM_WALK = 0.7f;
    const float STEP_MAX_NUM_RUN = 0.5f;

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
    public AudioClip rifleShot;
    public AudioClip shotGunShot;
    public AudioClip handGunShot;
    public AudioClip reload;
    public Slider healthBar;
    public GameObject ammoUI;
    public Image[] gunsUI;
    public Image[] highlights;
    public TextMeshProUGUI ammoText;
    public Image ammoImage;

    public bool isBeingChased = false;
    public float currentDamage = KNIFE_DAMAGE_NORMAL;
    public AudioClip[] stepSounds;
    public GameObject deathImage;

    private Rigidbody2D rigidbody;
    
    private string currentState = STATE_IDLE;
    private string currentWeapon = WEAPON_KNIFE;
    private bool isAttacking = false;
    private bool isReloading = false;
    private Vector2 handGunShootPos = new Vector2(0.55f, 1.17f);
    private AudioSource audioSource;

    private Ray2D basicShootRay;// = new Ray2D(handGunShootPos);

    private bool wasShot = false;
    private ShotRay[] shots = new ShotRay[SHOT_GUN_SPREAD_COUNT];
    private Vector2[] shotPositions = { new Vector2(1.2f, -0.55f), new Vector2(1.4f, -0.5f)};
    private bool isSprinting = false;
    private List<string> collectedCards = new List<string>();
    private float stepcount = 0;
    private int currentStepIndex = 0;
    private int currentWeaponIndex = 0;
    private Weapon[] weaponList = { new Weapon(WEAPON_KNIFE, true), new Weapon(WEAPON_HAND_GUN, 6, 20), new Weapon(WEAPON_RIFLE, 15, 30) };
    private float health = 100;



    
    

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animPlayer = sprite.GetComponent<Animator>();
        basicShootRay = new Ray2D(handGunShootPos, Vector2.left);
        knifeAttackArea.enabled = false;
        audioSource = GetComponent<AudioSource>();
        ammoUI.SetActive(false);
        ammoImage.enabled = false;
        //TakeDamage(100);
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        camera.transform.position = new Vector3(transform.position.x, transform.position.y, -1); //keeps camra at player's position (if making that child of player, then camera will rotate with player)
        RotateToMouse();
        if (health > 0)
            TestForInput();
        if (wasShot)
        {
            foreach (ShotRay ray in shots)
            {
                Debug.DrawLine(ray.startPoint, ray.endPoint, Color.red);
            }
        }
        if (currentState == STATE_RUNNING)
        {
            DoStep();
        }
    }

    void DoStep()
    {
        stepcount += Time.deltaTime;
        if (stepcount >= (!isSprinting ? STEP_MAX_NUM_WALK : STEP_MAX_NUM_RUN))
        {
            stepcount = 0;
            sprite.GetComponent<AudioSource>().clip = stepSounds[currentStepIndex];
            sprite.GetComponent<AudioSource>().Play();
            currentStepIndex = (currentStepIndex + 1) < stepSounds.Length ? currentStepIndex + 1 : 0;
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && !isReloading && (weaponList[currentWeaponIndex].currentClip > 0 || weaponList[currentWeaponIndex].isKnife))
        {
            if (currentWeapon == WEAPON_SHOT_GUN)
            {
                ShootShotGun();
                EmitAttackSound();
                weaponList[currentWeaponIndex].currentClip--;
                ammoText.text = weaponList[currentWeaponIndex].currentClip + "  " + weaponList[currentWeaponIndex].ammoRemaining;
            }
            else if (currentWeapon != WEAPON_KNIFE)
            {
                ShootGunNormal();
                EmitAttackSound();
                weaponList[currentWeaponIndex].currentClip--;
                ammoText.text = weaponList[currentWeaponIndex].currentClip + "  " + weaponList[currentWeaponIndex].ammoRemaining;
            }
            else
            {
                Invoke("KnifeAttack", 0.25f);
                //KnifeAttack();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                deathImage.SetActive(true);
            }
            ChangeCurrentAnim(STATE_ATTACK);
            
        }
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && weaponList[currentWeaponIndex].ammoRemaining > 0)
        {
            ChangeCurrentAnim(STATE_RELOAD);
            audioSource.clip = reload;
            audioSource.Play();
            isReloading = true;
            Invoke("EndReload", RELOAD_TIME);

        }
        TestForWeaponChange();
        isSprinting = Input.GetKey(KeyCode.LeftShift);
        sprintingColl.enabled = isSprinting || isBeingChased;
    }

    void TestForWeaponChange()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchWeapon(weaponList[0].weaponCode, 0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchWeapon(weaponList[1].weaponCode, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchWeapon(weaponList[2].weaponCode, 2);
        }
    }

    void EndReload()
    {
        isReloading = false;
        weaponList[currentWeaponIndex].ReloadWeapon();
        ammoText.text = weaponList[currentWeaponIndex].currentClip + "  " + weaponList[currentWeaponIndex].ammoRemaining;
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
        SetShotToPlay();
        GetComponent<AudioSource>().Play();
        return hit;
    }

    void SetShotToPlay()
    {
        AudioSource source = GetComponent<AudioSource>();
        switch (currentWeapon)
        {
            case WEAPON_RIFLE:
                source.clip = rifleShot;
                break;
            case WEAPON_SHOT_GUN:
                source.clip = shotGunShot;
                break;
            case WEAPON_HAND_GUN:
                source.clip = handGunShot;
                break;
        }
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

    void SwitchWeapon(string newWeapon, int index)
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
        ChangeWeaponGUI(index);
        //flashlight.transform.localPosition = shootPoint.localPosition;
        currentDamage = GetDamageToDeal();
    }

    void ChangeWeaponGUI(int newWeaponIndex)
    {
        highlights[currentWeaponIndex].enabled = false;
        currentWeaponIndex = newWeaponIndex;
        highlights[currentWeaponIndex].enabled = true;
        if (weaponList[currentWeaponIndex].isKnife)
        {
            ammoUI.SetActive(false);
            ammoImage.enabled = false;
        }
        else
        {
            ammoUI.SetActive(true);
            ammoImage.enabled = true;
            ammoText.text = weaponList[currentWeaponIndex].currentClip + "  " + weaponList[currentWeaponIndex].ammoRemaining;
        }
    }

    void TakeDamage(float damage)
    {
        health -= damage;
        healthBar.value = health;
        if (health <= 0)
        {
            deathImage.SetActive(true);
            Invoke("ReturnToMainMenu", 3);
        }
    }

    void SelectPrimary()
    {
        if (PlayerVarsToSave.hasShotGun)
        {
            weaponList[2] = new Weapon(WEAPON_SHOT_GUN, 5, 15);
            SwitchWeapon(WEAPON_SHOT_GUN, 2);
        }
        else
        {
            weaponList[2] = new Weapon(WEAPON_RIFLE, 15, 30);
            SwitchWeapon(WEAPON_RIFLE, 2);
        }
    }

    void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        zombieController enemyScript;
        if (collision.gameObject.tag == "Attack Area")
        {
            enemyScript = collision.gameObject.transform.parent.GetComponent<zombieController>();
            if (enemyScript.health > 0) 
            {
                enemyScript.Attack();
                enemyScript.sprite.GetComponent<BoxCollider2D>().enabled = false;
                enemyScript.Invoke("ResetColl", 1);
                isBeingChased = true;
                TakeDamage(enemyScript.attackDamage);
            }
        }
    }

    public void ChangePrimary(bool isShotGun)
    {
        SelectPrimary();
        PlayerVarsToSave.hasShotGun = isShotGun;
    }

    //.505 .53
}

class Weapon
{
    public string weaponCode;
    public int maxClipSize;
    public int ammoRemaining;
    public int currentClip;
    public bool isKnife;
    public Weapon(string name, int clipSize, int totalAmmo)
    {
        weaponCode = name;
        maxClipSize = clipSize;
        ammoRemaining = totalAmmo;
        ReloadWeapon();
    }
    public Weapon(string name, bool isKnife)
    {
        weaponCode = name;
        this.isKnife = isKnife;
    }

    public void ReloadWeapon()
    {
        for (int i=0; i<maxClipSize && currentClip < maxClipSize; i++)
        {
            if (ammoRemaining > 0)
            {
                ammoRemaining--;
                currentClip++;
            }
            else
                break;
        }
    }
}
public static class PlayerVarsToSave 
{
    public static bool hasShotGun = false;
}

