using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BlinkManager : MonoBehaviour 
{
	public ForceBlink blink;
	public AdvancedTimer timer;
	private float startDelay;
	public Text Instructions;
	public Text PromptInput;

	private int trialIndex = 0;
	public int numTrials = 10;
	public float[] score;
	public float[] reactionTime;
	public bool[] forceBlink;
	private bool presentedBlinkYet = false;

	private float blinkDuration = .01f;

	public float gettingInputStart;

	private bool started;
	private bool running  = true;
	private bool receivingInput = false;

	private bool countingSinceEnabled = false;

	void Start() 
	{
		timer = ScriptableObject.CreateInstance<AdvancedTimer>();
		timer.Init(1.5f, -1, true, 0, 1f, false, false, false);

		score = new float[numTrials];
		reactionTime = new float[numTrials];
		forceBlink = new bool[numTrials];

		#region Randomly Assign Forced Blinks
		for (int i = 0; i < (int) numTrials / 2; i++)
		{
			int randIndex = Random.Range(0, numTrials);

			while (true)
			{
				if (!forceBlink[randIndex])
				{
					forceBlink[randIndex] = true;
					break;
				}
				else
				{
					randIndex = Random.Range(0, numTrials);
				}
			}
		}
		#endregion

		Instructions.gameObject.SetActive(true);
		PromptInput.gameObject.SetActive(false);
	}
	
	void Update()
	{
		#region Test Blink Input with Return
		if (Input.GetKeyDown(KeyCode.Return))
		{
			blink.ExecuteBlink(blinkDuration);
		}
		#endregion

		timer.UpdateTimer(Time.deltaTime);

		if (running)
		{
			#region Present the Blink
			if (forceBlink[trialIndex] && !presentedBlinkYet && timer.counter <= blinkDuration + .05f)
			{
				presentedBlinkYet = true;
				blink.ExecuteBlink(blinkDuration);
			}
			#endregion

			#region Timer Completion
			if (timer.CheckTimer())
			{
				receivingInput = true;
				gettingInputStart = Time.timeSinceLevelLoad;
				DisplayPromptInput();
			}
			#endregion

			HandleInputLogic();
		}
	}

	public IEnumerator PromptBlink(float time)
	{
		float adjusted = 0.01f;
		Time.timeScale = adjusted;
		yield return new WaitForSeconds(time * adjusted);
		Time.timeScale = 1f;
	}

	void HandleInputLogic()
	{
		#region Pre-trial
		if (!started)
		{
			if (CheckInput())
			{
				Instructions.gameObject.SetActive(false);
				started = true;
				timer.Start();
			}
		}
		#endregion

		#region Running Trial
		//If we're running trials
		if (started && trialIndex < numTrials)
		{
			//If we got input
			if (CheckInput())
			{
				//If we're getting input
				if (receivingInput)
				{
					//Set the score and reaction counter
					score[trialIndex] += Time.timeSinceLevelLoad - gettingInputStart;
					reactionTime[trialIndex] = Time.timeSinceLevelLoad - gettingInputStart;

					//We don't want to ask for input
					PromptInput.gameObject.SetActive(false);

					//We haven't given a blink yet
					presentedBlinkYet = false;

					//Reset the clock
					timer.Reset(true);
	
					//Next trial
					trialIndex++;

					if (trialIndex >= numTrials)
					{
						OutputScores();
						running = false;
					}

				}
				else
				{
					//Punish the user for early input
					score[trialIndex] += .25f;

					//Set the timer back so they don't miss the actual input.
					timer.AdjustTime(-.35f);
				}
			}
		}
		#endregion
	}

	void DisplayPromptInput()
	{
		//Select a random location
		float xPos = Random.Range(Screen.width * .2f, Screen.width * .8f);

		PromptInput.gameObject.SetActive(true);
		//Put the prompt text there.
	}

	bool CheckInput()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			return true;
		}
		return false;
	}

	public void OutputScores()
	{
		float totalScore = 0;
		float averageBlinkTime = 0;
		float averageNoBlinkTime = 0;
		for (int i = 0; i < score.Length; i++)
		{
			totalScore += score[i];
		}

		for (int i = 0; i < reactionTime.Length; i++)
		{
			if (forceBlink[i])
			{
				averageBlinkTime += reactionTime[i];
			}
			else
			{
				averageNoBlinkTime += reactionTime[i];
			}
		}

		averageBlinkTime = averageBlinkTime / numTrials;
		averageNoBlinkTime = averageNoBlinkTime / numTrials;

		string output = "Total Score:\t" + totalScore;
		output += "\nAverage Blink Time:\t" + averageBlinkTime;
		output += "\nAverage Blinkless Time:\t" + averageNoBlinkTime;


		Debug.Log(output + "\n\n\n\n");
	}
}
