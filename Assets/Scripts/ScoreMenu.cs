using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
public class ScoreMenu : MonoBehaviour
{
    InputAction enterAction;
    InputAction escapeAction;
    public TextMeshProUGUI hitsTakenText;
    public TextMeshProUGUI hitsTakenGrade;
    public TextMeshProUGUI timeTakenText;
    public TextMeshProUGUI timeTakenGrade;
    public AudioSource jingle;
    public AudioSource fail;

    void Start()
    {
        enterAction = new InputAction("Enter", binding: "<Keyboard>/enter");
        enterAction.performed += ctx => Entered();
        enterAction.Enable();

        escapeAction = new InputAction("Escape", binding: "<Keyboard>/escape");
        escapeAction.performed += ctx => Escaped();
        escapeAction.Enable();

        if (ScoreManager.victory) {
            jingle.Play();
        } else
        {
            fail.Play();
        }
        try {
            ScoreManager.makeScreenshotsFolder();
            ScoreManager.writeScreenshotsToDisk();
        } catch (Exception e)
        {
            Debug.Log("ScoreMenu: Exception when writing screenshots to disk: " + e.Message);
        }
    }

    void Update()
    {
        hitsTakenText.text = "Hits taken: " + ScoreManager.hitsTaken.ToString();
        timeTakenText.text = "Time taken: " + ScoreManager.timeTaken.ToString("F2") + "s";

        string congratsHits = "";
        string congratsTime = "";

        if (ScoreManager.victory) {
            hitsTakenGrade.color = Color.gold;
            timeTakenGrade.color = Color.gold;
            if (ScoreManager.hitsTaken == 0)
            {
                congratsHits = "Congratulations, superplayer!";
                hitsTakenGrade.color = Color.green;
            }
            else if (ScoreManager.hitsTaken <= 1)
            {
                congratsHits = "Almost! Try for a no-miss!";
            }
            else if (ScoreManager.hitsTaken <= 2)
            {
                congratsHits = "Great job!";
            }
            else if (ScoreManager.hitsTaken <= 3)
            {
                congratsHits = "Not bad!";
            }
            else if (ScoreManager.hitsTaken <= 3)
            {
                congratsHits = "Keep it up!";
            } else
            {
                congratsHits = "You can do better!";
            }

            if (ScoreManager.timeTaken <= 90f)
            {
                congratsTime = "Congratulations, superscorer!";
                timeTakenGrade.color = Color.green;
            }
            else if (ScoreManager.timeTaken <= 120f)
            {
                congratsTime = "Great time!";
            }
            else if (ScoreManager.timeTaken <= 150f)
            {
                congratsTime = "Not bad!";
            }
            else if (ScoreManager.timeTaken <= 180f)
            {
                congratsTime = "Keep practicing!";
            }
            else
            {
                congratsTime = "You can do better!";
            }
        } else
        {
            congratsHits = "Defeat!";
            congratsTime = "Defeat!";
            hitsTakenGrade.color = Color.red;
            timeTakenGrade.color = Color.red;
        }

        timeTakenGrade.text = congratsTime;
        hitsTakenGrade.text = congratsHits;
    }
    
    void Entered()
    {
        SceneManager.LoadScene("Main", LoadSceneMode.Single);
    }

    void Escaped()
    {
        Application.Quit();
    }
}
