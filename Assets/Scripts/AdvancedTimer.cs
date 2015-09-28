﻿using UnityEngine;
using System.Collections;

public class AdvancedTimer : ScriptableObject
{
	public float counter;
	//public float Counter
	//{
	//	get { return counter; }
	//	set { counter = value; }
	//}
	public float lastResetValue;

	public float countFromValue;
	public float variationMin;
	public float variationMax;
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
	/// <param name="timerAmount">The default starting value for the timer upon resets</param>
	/// <param name="startAt">The amount the timer should be set to the first time, -1 for whatever the timerAmount is</param>
	/// <param name="minVariation">Randomize the reset amounts, minimum adjustment</param>
	/// <param name="maxVariation">Randomize the reset amounts, maximum adjustment</param>
	/// <param name="beginImmediately">Should the timer begin immediately for the current Updates</param>
	/// <param name="resetAfterProc">Shoudl the timer automatically reset and start timing again when it finishes</param>
	/// <param name="runWhilePaused">Should the timer respect pausing.</param>
	public void Init(float timerAmount, float startAt = -1, float minVariation = 0, float maxVariation = 0, bool beginImmediately = true, bool resetAfterProc = true, bool runWhilePaused = false)
	{
		if(startAt != -1)
			counter = startAt;
		else
			counter = timerAmount;
		countFromValue = timerAmount;

		variationMin = minVariation;
		variationMax = maxVariation;

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
		else if (capAtMax && counter + timeToAdjust > countFromValue + variationMax)
		{
			counter = countFromValue + variationMax;
		}
		else
		{
			counter += timeToAdjust;
		}
	}

	public void Reset(bool beginTimer)
	{
		running = beginTimer;

		float randVal = Random.Range(variationMin, variationMax);

		lastResetValue = countFromValue + randVal;
		counter += countFromValue + randVal;

		//Debug.Log("AdvancedTimer Ring!\nResetting counter to: " + counter + "\tRandVal: " + randVal + "\tVarMin: " + variationMin + "\tVarMax: " + variationMax + "\n");
	}

	public void IncrementTimer(float timePassed)
	{
		counter -= timePassed;

		if (counter <= 0)
		{
			timerComplete = true;
			running = false;
		}
	}

	public void UpdateTimer(float timePaseed)
	{
		if (running && !timerComplete)
		{
			if (!respectPause)
			{
				IncrementTimer(timePaseed);
			}
			//else if (!GameManager.Inst.paused)
			//{
				IncrementTimer(timePaseed);
			//}
		}
	}
}