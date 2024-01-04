using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;


public class startgame : MonoBehaviour
{
    public GameObject livesPanel;
    public GameObject introPanel;
    public TMP_Text text;
    void Start()
    {
        livesPanel.SetActive(false);
        introPanel.SetActive(true);
        Time.timeScale = 0f;
    }


    public void clickPlay(){
        Time.timeScale = 1f;
        introPanel.SetActive(false);
        livesPanel.SetActive(true);

    }
}


