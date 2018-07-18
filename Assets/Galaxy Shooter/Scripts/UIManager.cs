using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Sprite[] lives;
    public Image livesImageDisplay;
    public Text scoreText;
    public Text difficultyText;
    public int score;
    public GameObject titleScreen;

    public void UpdateLives(int currentLives)
    {
        livesImageDisplay.sprite = lives[currentLives];
    }
    
    public void ShowTitleScreen()
    {
        titleScreen.SetActive(true);
    }

    public void HideTitleScreen()
    {
        titleScreen.SetActive(false);
        scoreText.text = "Score: 0";
    }

    public void UpdateScore()
    {
        score++;
        scoreText.text = "Score: " + score;
    }

    public void UpdateDifficulty(float difficulty)
    {
        difficultyText.text = "Difficulty: " + difficulty.ToString();
    }

}
