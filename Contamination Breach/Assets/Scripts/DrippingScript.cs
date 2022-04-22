using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DrippingScript : MonoBehaviour
{
    const float BASE_DRIP_TIME = 5;
    const float MAX_SUB_NUM = 1.2f;

    public AudioClip[] drippingClips;

    private System.Random rand = new System.Random();


    void Start()
    {
        Invoke("DoDrip", BASE_DRIP_TIME - UnityEngine.Random.Range(0, MAX_SUB_NUM));
    }

    void DoDrip()
    {
        GetComponent<AudioSource>().clip = drippingClips[rand.Next(drippingClips.Length)];
        GetComponent<AudioSource>().Play();
        Invoke("DoDrip", BASE_DRIP_TIME - UnityEngine.Random.Range(0, MAX_SUB_NUM));
    }
}
