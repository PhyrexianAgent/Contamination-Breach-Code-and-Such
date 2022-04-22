using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveScript : MonoBehaviour
{
    const float MAX_DISTANCE_FROM_PLAYER = 2;

    public GameObject player;
    public SpriteRenderer prompt;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)));
        prompt.enabled = distance <= MAX_DISTANCE_FROM_PLAYER && !GetComponent<SpriteRenderer>().enabled;

        if (prompt.enabled && Input.GetKeyDown(KeyCode.E))
        {
            GetComponent<SpriteRenderer>().enabled = true;
            player.GetComponent<PlayerCode>().bombsRemaining--;
            GetComponent<AudioSource>().Play();
            GetComponent<CircleCollider2D>().enabled = true;
            Invoke("DisableColl", 0.4f);
        }
    }

    private void DisableColl()
    {
        GetComponent<CircleCollider2D>().enabled = false;
    }
}
