using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.IO;
using TMPro;


public class NewBehaviourScript : MonoBehaviour
{
    public static NewBehaviourScript Instance;

    public AudioClip starsound;
    public AudioClip applesound;
    public AudioSource source;

    public int num_lives;
    public GameObject livesPanel;
    public GameObject introPanel;
    public GameObject scorePanel;
    public TMP_Text text;
    public TMP_Text scoreText;
    public TMP_Text timeText;
    private float startTime;
    private float endTime;
    private bool ballCollisionProcessed = false;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startTime = Time.time;
        num_lives = 5;
        Debug.Log("showing lives");
        livesPanel.SetActive(false);
        introPanel.SetActive(true);
        scorePanel.SetActive(false);
        //source = GetComponent<AudioSource>(); 
    }
    void Update()
    {
        if (num_lives <= 0)
        {
            endTime = Time.time;
            float elapsedScore = endTime - startTime;
            scoreText.text = "Your score : " + elapsedScore;
            Time.timeScale = 0f;
            scorePanel.SetActive(true);
        }
        text.text = "Lives left : "+num_lives; 
        endTime = Time.time;
        float elapsed = endTime - startTime;
        timeText.text = elapsed.ToString();
        ballCollisionProcessed = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Apple") && !ballCollisionProcessed)
        {
            num_lives -= 1;
            source.PlayOneShot(applesound,1F);
            ballCollisionProcessed = true;
            Destroy(other.gameObject);
        }

        else if (other.CompareTag("Ball"))
        {
            Debug.Log("hit ball");
            num_lives -= 1;
            source.PlayOneShot(applesound, 1F);
            Destroy(other.gameObject);
        }
        

        if (other.CompareTag("Star"))
        {
            source.PlayOneShot(starsound,1F);
        }
    }
    
    public void IncreaseLives()
    {
        num_lives += 1;
    }

    public void onPlayAgain()
    {
        SceneManager.LoadScene("SampleScene");
    }
}

