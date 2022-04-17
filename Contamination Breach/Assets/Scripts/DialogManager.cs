using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public Queue<string> messages = new Queue<string>();

    public Dialog[] dialogs;
    public TextMeshProUGUI dialogText;
    public DialogAdvance dialogObject;
    public bool isEnd = true;
    public GameObject endImage;

    private int currentDialogIndex = 0;

    public void TriggerDialog(Dialog dialog)
    {

    }

    private void Update()
    {
    }

    private void Start()
    {
        StartDialog();
    }

    public void StartDialog()
    {
        messages.Clear();
        foreach (string message in dialogs[currentDialogIndex].messages)
        {
            messages.Enqueue(message);
        }
        DisplayNextSentance();
    }

    IEnumerator TypeMessage(string message)
    {
        dialogText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(0.025f);
        }
    }

    public void DisplayNextSentance()
    {
        string sentance;
        if (messages.Count == 0)
        {
            EndDialog();
        }
        else
        {
            dialogObject.GetComponent<DialogAdvance>().ChangeDialogVisibility(true);//animPlayer.SetBool("isOpen", true);
            sentance = messages.Dequeue();
            //dialogText.text = sentance;
            StopAllCoroutines();
            StartCoroutine(TypeMessage(sentance));
        }
        
        
    }

    private void EndDialog()
    {
        dialogText.text = "";
        dialogObject.GetComponent<DialogAdvance>().ChangeDialogVisibility(false);//animPlayer.SetBool("isOpen", false);
        currentDialogIndex++;
        if (currentDialogIndex >= dialogs.Length && isEnd)
        {
            endImage.SetActive(true);
            Invoke("EndGame", 5);
        }

    }

    void EndGame()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
