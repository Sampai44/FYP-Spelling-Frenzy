using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDifficulty : MonoBehaviour
{
    public void ChooseClick(int level)
    {
        PlayerPrefs.SetInt("Difficulty",level);
        SceneManager.LoadScene("Game");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
