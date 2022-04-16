using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    public Sprite endImage;

    private Animator animPlayer;
    
    void Start()
    {
        animPlayer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisableSelf()
    {
        animPlayer.enabled = true;
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<AudioSource>().Play();
        Invoke("StopAnim", 0.4f);
    }

    void StopAnim()
    {
        animPlayer.enabled = false;
        GetComponent<SpriteRenderer>().sprite = endImage;
    }
}
