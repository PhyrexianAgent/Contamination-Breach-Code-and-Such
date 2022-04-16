using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipableGunScript : MonoBehaviour
{
    const float DISTANCE_TO_PLAYER_MAX = 3;

    public GameObject player;
    public GameObject prompt;
    public SpriteRenderer other;
    public bool isShotGun = false;

    private bool used = false;


    private bool promptActive;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)));
        promptActive = distance <= DISTANCE_TO_PLAYER_MAX && GetComponent<SpriteRenderer>().enabled && !used;
        prompt.GetComponent<SpriteRenderer>().enabled = promptActive;
        CheckForInput();
    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && promptActive)
        {
            
            other.enabled = true;
            other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true; other.enabled = true;
            GetComponent<SpriteRenderer>().enabled = false;
            player.GetComponent<PlayerCode>().ChangePrimary(isShotGun);
            used = true;
        }
    }
}
