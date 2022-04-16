using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCardScript : MonoBehaviour
{
    const float MAX_DISTANCE_TO_PLAYER = 1.3f;

    public string cardCode;
    public GameObject prompt;
    public GameObject player;

    private bool promptActive = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)));
        promptActive = distance <= MAX_DISTANCE_TO_PLAYER;
        prompt.GetComponent<SpriteRenderer>().enabled = promptActive;
        CheckForInput();

    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && promptActive)
        {
            player.GetComponent<PlayerCode>().CollectCard(cardCode);
            GetComponent<SpriteRenderer>().enabled = false;
            prompt.GetComponent<SpriteRenderer>().enabled = false;
            Destroy(this);
        }
    }
}
