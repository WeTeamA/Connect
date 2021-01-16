using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameController : MonoBehaviour
{
    float timeScale;
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject PouseMenu;
    [SerializeField] GameObject InGame;
    [SerializeField] GameObject GameOver;
    [SerializeField] GameObject currentUI;

    bool changeTime;

    [SerializeField] TextMeshProUGUI ScoreTable;
    [SerializeField] TextMeshProUGUI GameOverText;
    [SerializeField] TextMeshProUGUI HighScore;


    public static GameController manager;

    private void Awake()
    {
        manager = this;
        Scene scene = SceneManager.GetActiveScene();
        HighScore.text = PlayerPrefs.GetInt("HighScore" + scene.buildIndex).ToString();
    }

    public void PauseGame()
    {
        StopTime();

        ChangeUI(PouseMenu);
    }

    void StopTime()
    {
        timeScale = Time.timeScale;
        Time.timeScale = 0;
    }

    public void RestartLvl()
    {
        Scene scene = SceneManager.GetActiveScene();
        StartCoroutine(LoadYourAsyncScene(scene.buildIndex));
    }

    public void LoadMainMenu()
    {
        //StartCoroutine(LoadYourAsyncScene());
    }

    public void EndLevel(bool Lost)
    {


        //ScowMo
        changeTime = true;

        //Disables player
        Player.player.EndConnection();
        Player.player.enabled = false;

        if (Lost)
        {
            GameOverText.text = "Game Over!";
        }
        else
        {
            GameOverText.text = "Well Done!";
        }

        ChangeUI(GameOver);

        //Set HighScore
        Scene scene = SceneManager.GetActiveScene();
        int Score = GetScore();
        if (PlayerPrefs.GetInt("HighScore" + scene.buildIndex) < Score)
        {
            PlayerPrefs.SetInt("HighScore" + scene.buildIndex, Score);
            HighScore.text = Score.ToString();

        }
    }

    public void ResumeGame()
    {
        Time.timeScale = timeScale;

        ChangeUI(InGame);
    }

    void ChangeUI(GameObject next)
    {
        currentUI.SetActive(false);
        next.SetActive(true);
        currentUI = next;
    }

    IEnumerator LoadYourAsyncScene(int id)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(id);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    public void IncreaseScore(int Addition)
    {
        int Score = GetScore();
        Score += Addition;
        ScoreTable.text = Score.ToString();
    }

    public int GetScore()
    {
        int Score = int.Parse(ScoreTable.text);
        return Score;
    }

    private void Update()
    {
        ChangeTime();
    }

    void ChangeTime()
    {
        if (changeTime)
        {
            if(Time.timeScale > 0.2)
            {
                Time.timeScale -= Time.deltaTime;
            }
            else
            {
                StopTime();
                changeTime = false;
            }
        }
    }
}
