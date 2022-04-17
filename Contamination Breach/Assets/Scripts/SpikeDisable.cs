using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeDisable : MonoBehaviour
{
    const float MAX_DISTANCE_TO_WORK = 3;

    public GameObject[] spikes;
    public GameObject[] zombiesToEnable;
    public GameObject prompButton;
    public string acceptedCardCode;

    public GameObject player;

    private bool promptActive = false;
    private bool used = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y)* (player.transform.position.y - transform.position.y)));
        promptActive = distance <= MAX_DISTANCE_TO_WORK && !used;
        prompButton.GetComponent<SpriteRenderer>().enabled = promptActive;
        CheckForInput();
    }

    void CheckForInput()
    {
        if (Input.GetKeyDown(KeyCode.E) && promptActive)
        {
            if (player.GetComponent<PlayerCode>().HasCard(acceptedCardCode))
            {
                DisableTraps();
                used = true;
            }
            else
            {
                if (!GetComponent<AudioSource>().isPlaying)
                {
                    GetComponent<AudioSource>().Play();
                }
            }
        }
    }

    void DisableTraps()
    {
        foreach (GameObject trap in spikes)
        {
            trap.GetComponent<SpikeScript>().DisableSelf();
        }

        foreach (GameObject zombie in zombiesToEnable)
        {
            zombie.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        Debug.Log(coll.gameObject.tag);
        if (coll.gameObject.tag == "Player" || coll.gameObject.tag == "Player Attack")
        {
            Debug.Log("work");
            prompButton.GetComponent<SpriteRenderer>().enabled = true;
        }
    }
}
