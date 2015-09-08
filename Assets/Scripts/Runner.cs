using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Runner : MonoBehaviour
{
	public static Runner Inst;
	#region Core Attributes
	public int score = 0;
	private float maxHealth = 100;
	public float health = 100;

	public float speed;
	private float maxSpeed = 5;

	public int statusEffect = 0;
	public float statusLength = 0.0f;
	public bool burning = false;

	#endregion

	#region Lane and World Info
	public GameObject[] Cameras;
	public GameObject[,] Lanes;
	public int WorldIndex;
	public int lane;
	#endregion

	#region UI Elements
	public Text statusUI;
	public Text scoreUI;
	public Slider healthSlider;
	public Slider speedSlider;
	#endregion

	void Start()
	{
		if (Runner.Inst == null)
		{
			Runner.Inst = this;
			Debug.Log(Runner.Inst);
		}

		WorldIndex = 0;

		healthSlider.maxValue = maxHealth;
		healthSlider.value = health;

		Lanes = new GameObject[4,4];

		for(int i = 0; i < Cameras.Length; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				Lanes[i,j] = Cameras[i].transform.FindChild("Lane Parent").FindChild("Lane ["+j+"]").gameObject;
			}
		}

		StepIntoLane(2);
	}

	void Update()
	{
		GetInput();

		statusLength -= Time.deltaTime;
		if (statusEffect != 0 && statusLength <= 0)
		{
			ChangeStatusEffect(0);
		}

		if (statusEffect == 1 && !burning)
		{
			StartCoroutine(BurnDamage());
		}
		
	}

	IEnumerator BurnDamage()
	{
		burning = true;
		yield return new WaitForSeconds(.25f);

		if (statusEffect == 1)
		{
			AdjustHealth(-.5f);
			burning = false;
		}
	}

	public void GetInput()
	{
		#region L/R Lane Shifting
		if (Input.GetKeyDown(KeyCode.LeftArrow))
		{
			StepLeft();
		}
		if (Input.GetKeyDown(KeyCode.RightArrow))
		{
			StepRight();
		}
		#endregion

		#region Status Effect Changing
		if (Input.GetKey(KeyCode.UpArrow))
		{
			if (speed < maxSpeed)
			{
				AdjustSpeed(speed);
			}
		}
		if (Input.GetKey(KeyCode.DownArrow))
		{
			if (speed > 2)
			{
				AdjustSpeed(speed);
			}
		}
		#endregion

		#region Status Effect Changing
		if (Input.GetKeyDown(KeyCode.RightShift))
		{
			if (statusEffect < 4)
			{
				ChangeStatusEffect(statusEffect + 1);
			}
		}
		if (Input.GetKeyDown(KeyCode.RightControl))
		{
			if (statusEffect > 0)
			{
				ChangeStatusEffect(statusEffect - 1);
			}
		}
		#endregion

		#region Lane Control
		if (Input.GetKeyDown(KeyCode.Q))
		{
			StepIntoLane(0);
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			StepIntoLane(1);
		}
		if (Input.GetKeyDown(KeyCode.E))
		{
			StepIntoLane(2);
		}
		if (Input.GetKeyDown(KeyCode.R))
		{
			StepIntoLane(3);
		}
		#endregion

		#region Plane Control
		if (Input.GetKeyDown(KeyCode.U))
		{
			PhaseShift(0);
		}
		if (Input.GetKeyDown(KeyCode.I))
		{
			PhaseShift(1);
		}
		if (Input.GetKeyDown(KeyCode.O))
		{
			PhaseShift(2);
		}
		if (Input.GetKeyDown(KeyCode.P))
		{
			PhaseShift(3);
		}
		#endregion
	}

	public void AdjustSpeed(float adjustment)
	{
		//Check statuses

		speed += adjustment;
	}

	public void StepIntoLane(int newLane = -1)
	{
		if (newLane == -1)
		{
			newLane = lane;
		}
		else
		{
			lane = newLane;
		}
		transform.position = Lanes[WorldIndex, newLane].transform.position - Vector3.forward - 4.5f * Vector3.up;
	}

	public void StepRight()
	{
		if (lane < 3)
		{
			lane++;
		}
		StepIntoLane(lane);
	}

	public void StepLeft()
	{
		if (lane > 0)
		{
			lane--;
		}
		StepIntoLane(lane);
	}

	public void ChangeStatusEffect(int newStatus)
	{
		if (newStatus == 0)
		{
			statusUI.text = "";
		}
		else
		{
			statusLength = 5.0f;
		}
		if (newStatus == 1)
		{
			statusUI.text = "Burning";
			burning = false;
		}
		if (newStatus == 2)
		{
			statusUI.text = "Soaked";
		}
		if (newStatus == 3)
		{
			statusUI.text = "Mucked";
		}
		if (newStatus == 4)
		{
			statusUI.text = "Gusted";
		}

		Debug.Log("Status: " + statusEffect + " to " + newStatus);
		statusEffect = newStatus;

	}

	public void AdjustHealth(float healthAdj)
	{
		//The Soaked status effect damage prevention.
		if (statusEffect != 2)
		{
			health += healthAdj;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			healthSlider.value = health;

			if (health <= 0)
			{
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		else
		{
			statusEffect = 0;
		}
	}

	public void PhaseShift(int targetPlane)
	{
		if (targetPlane >= 0 && targetPlane < 4)
		{
			WorldIndex = targetPlane;

			StepIntoLane();
			
			if (statusEffect != 0)
			{
				ChangeStatusEffect(0);
			}

		}
	}
}