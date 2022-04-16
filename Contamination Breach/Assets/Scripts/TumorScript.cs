using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TumorScript : MonoBehaviour
{
    const float MAX_DISTANCE_FROM_PLAYER = 11;

    public GameObject player;

    private Animator animPlayer;
    private string currentAnim;
    void Start()
    {
        animPlayer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Mathf.Sqrt(((player.transform.position.x - transform.position.x) * (player.transform.position.x - transform.position.x)) + ((player.transform.position.y - transform.position.y) * (player.transform.position.y - transform.position.y)));
        PlayAnim(distance <= MAX_DISTANCE_FROM_PLAYER ? "Tumor Eye Open" : "Tumor Eye Closed");
    }

    void PlayAnim(string name)
    {
        if (currentAnim != name)
        {
            animPlayer.Play(name);
        }
    }
}
