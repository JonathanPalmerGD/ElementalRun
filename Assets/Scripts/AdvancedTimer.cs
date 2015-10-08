using UnityEngine;
using System.Collections;

public class AdvancedTimer : ScriptableObject
{
	public float counter;
	//public float Counter
	//{
	//	get { return counter; }
	//	set { counter = value; }
	//}
	//public float lastResetValue;

	public float timerSetValue;
	public float timerTargetValue;
	public float minDurVariation;
	public float maxDurVariation;

	public bool countUpwards = false;

	public bool respectPause = true;
	public bool RespectPause 
	{
		get { return respectPause; }
		set { respectPause = value; }
	}
	private bool running;
	public bool Running
	{
		get { return running; }
		set { running = value; }
	}
	private bool continuousReset;
	public bool ContinuousReset
	{
		get { return continuousReset; }
		set { continuousReset = value; }
	}

	//This is turned to true when the timer is complete.
	private bool timerComplete;

	/// <summary>
	/// Initializes the Timer variables
	/// </summary>
	/// <param name="timerDuration">The default starting value for the timer upon resets</param>
	/// <param name="startAt">The amount the timer should be set to the first time, -1 for whatever the timerDuration is</param>
	/// <param name="durMinVariation">Randomize the reset amounts, minimum adjustment</param>
	/// <param name="durMaxVariation">Randomize the reset amounts, maximum adjustment</param>
	/// <param name="beginImmediately">Should the timer begin immediately for the current Updates</param>
	/// <param name="resetAfterProc">Shoudl the timer automatically reset and start timing again when it finishes</param>
	/// <param name="runWhilePaused">Should the timer respect pausing.</param>
	public void Init(float timerDuration, float startAt = -1, bool countDownwards = true, float durMinVariation = 0, float durMaxVariation = 0, bool beginImmediately = true, bool resetAfterProc = true, bool runWhilePaused = false)
	{
		countUpwards = !countDownwards;

		if (startAt != -1)
		{
			if (countUpwards)
			{
				counter = 0;
				timerTargetValue = startAt;
			}
			else
			{
				counter = startAt;
				timerTargetValue = 0;
			}
		}
		else
		{
			if (countUpwards)
			{
				counter = 0;
				timerTargetValue = timerDuration;
			}
			else
			{
				counter = timerDuration;
				timerTargetValue = 0;
			}
		}

		timerSetValue = timerDuration;

		minDurVariation = durMinVariation;
		maxDurVariation = durMaxVariation;

		running = beginImmediately;

		continuousReset = resetAfterProc;

		respectPause = runWhilePaused;

		timerComplete = false;
	}

	public void Stop()
	{
		running = false;
	}
	public void Start()
	{
		running = true;
	}

	/// <summary>
	/// Call to check if the timer has completed the last cycle.
	/// Will reset automatically if continuous reset is set.
	/// </summary>
	/// <returns>Whether the timer has finished.</returns>
	public bool CheckTimer()
	{
		if (timerComplete)
		{
			running = false;
			timerComplete = false;
			Reset(continuousReset);
			return true;
		}
		return false;
	}

	public void AdjustTime(float timeToAdjust, bool capAtMax = false, bool disregardNegative = true)
	{
		//TODO: Have this appreciate counting upwards or downwards. Only appreciates counting up atm.
		if (counter + timeToAdjust < 0)
		{
			timerComplete = true;
			running = false;

			if (disregardNegative)
			{
				counter = 0;
			}
			else
			{
				counter += timeToAdjust;
			}
		}
		else if (capAtMax && counter + timeToAdjust > timerSetValue + maxDurVariation)
		{
			counter = timerSetValue + maxDurVariation;
		}
		else
		{
			counter += timeToAdjust;
		}
	}

	public void Reset(bool beginTimer)
	{
		running = beginTimer;

		float randVal = Random.Range(minDurVariation, maxDurVariation);

		//lastResetValue = timerSetValue + randVal;
		//counter += timerSetValue + randVal;

		//If we count up
		if (countUpwards)
		{
			//Set the counter to 0
			counter = 0;
			//Set our target based on what the timer's set value is plus the variation
			timerTargetValue = timerSetValue + randVal;
		}
		//If we count down, do the opposite.
		else
		{
			counter += timerSetValue + randVal;
		}

		//Debug.Log("AdvancedTimer Ring!\nResetting counter to: " + counter + "\tRandVal: " + randVal + "\tVarMin: " + minDurVariation + "\tVarMax: " + maxDurVariation + "\n");
	}

	public void DecrementTimer(float timePassed)
	{
		counter -= timePassed;

		if (counter <= timerTargetValue)
		{
			timerComplete = true;
			running = false;
		}
	}

	public void IncrementTimer(float timePassed)
	{
		counter += timePassed;

		if (counter >= timerTargetValue)
		{
			timerComplete = true;
			running = false;
		}
	}

	public void UpdateTimer(float timePassed)
	{
		if (running && !timerComplete)
		{
			//if (!respectPause)
			//{
			if (countUpwards)
			{
				IncrementTimer(timePassed);
			}
			else
			{
				DecrementTimer(timePassed);
			}

			//}
			//else if (!GameManager.Inst.paused)
			//{
			//	DecrementTimer(timePassed);
			//}
		}
	}
}
