using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogAdvance : MonoBehaviour
{
    public DialogManager dialogManager;
    public Image dialogBack;
    public TextMeshProUGUI dialogText;
    
    public Animator animPlayer;

    public bool enabled = false;
    void Start()
    {
        animPlayer = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            dialogManager.DisplayNextSentance();
        }
    }

    public void ChangeDialogVisibility(bool reveal)
    {
        animPlayer.SetBool("isOpen", reveal);
        if (reveal)
        {
            dialogBack.enabled = reveal;
            dialogText.enabled = reveal;
            
        }
        else
        {
            Invoke("HideDialog", 0.16f);
        }
    }

    void HideDialog() 
    {
        dialogBack.enabled = false;
        dialogText.enabled = false;
    }

}
