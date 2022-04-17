using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEnd : MonoBehaviour
{
    public DialogManager dialogManager;
    public bool moveToLevel = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!moveToLevel) 
        {
            dialogManager.StartDialog();
        }
        else
        {
            SceneManager.LoadScene("Level 1");
        }
    }
}
