using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EquipableGunScript : MonoBehaviour
{
    const float DISTANCE_TO_PLAYER_MAX = 3;
    const float INPUT_DELAY_TIME = 1;

    public GameObject player;
    public GameObject prompt;
    public Sprite[] gunImages;
  
    
    private bool isShotGun = true;
    private int imageIndex = 1;
    private SpriteRenderer sprite;
    private bool usable = true;



    private bool promptActive;
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)));
        promptActive = distance <= DISTANCE_TO_PLAYER_MAX && usable;
        prompt.GetComponent<SpriteRenderer>().enabled = promptActive;
        CheckForInput();
    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && promptActive && usable)
        {
            isShotGun = !isShotGun;
            imageIndex = Convert.ToInt32(isShotGun);
            sprite.sprite = gunImages[imageIndex];
            player.GetComponent<PlayerCode>().ChangePrimary(!isShotGun);
        }
    }
    
    void ChangeUsable()
    {
        usable = true;
    }
}
