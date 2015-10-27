using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
	public string[] scoreKeys;
	public Text[] ScoreTexts;
	public Text TotalScore;

	private string highScore = "HighScore";
	private string highScoreName = "HighScoreName";
	private int numHighScores = 20;

	class HighScore
	{
		int score;

	}

	void Start ()
	{
		int totalscr = 0;
		for (int i = 0; i < ScoreTexts.Length; i++)
		{
			Debug.Log("Score: " + scoreKeys[i] + "   " + PlayerPrefs.HasKey(scoreKeys[i]) + "\n");
			if (PlayerPrefs.HasKey(scoreKeys[i]))
			{
				ScoreTexts[i].text = "" + PlayerPrefs.GetInt(scoreKeys[i]);
				totalscr += PlayerPrefs.GetInt(scoreKeys[i]);
			}
		}

		if (TotalScore != null)
		{
			TotalScore.text = "" + totalscr;
		}

		for (int i = 0; i < numHighScores; i++)
		{
			if (PlayerPrefs.HasKey(highScore + "" + i) && PlayerPrefs.HasKey(highScoreName + "" + i))
			{
				AddHighScore(PlayerPrefs.GetInt(highScore + "" + i), PlayerPrefs.GetString(highScoreName + "" + i));
			}
		}
	}

	private void AddHighScore(int highScore, string name = "")
	{

	}

	void Update ()
	{
	
	}

	public void ReplayGame()
	{
		Application.LoadLevel("RunningScene");
	}

	public void QuitGame()
	{
		Application.Quit();
	}
}
