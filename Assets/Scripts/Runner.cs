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

	private GameObject burnedPrefab;
	private GameObject soakedPrefab;
	private GameObject gustedPrefab;
	private GameObject muckedPrefab;

	private GameObject burnedParticle;
	private GameObject soakedParticle;
	private GameObject gustedParticle;
	private GameObject muckedParticle;
	#endregion

	#region Lane and World Info
	public Camera[] Cameras;
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

	private void DisableAllParticles()
	{
		burnedParticle.SetActive(false);
		soakedParticle.SetActive(false);
		gustedParticle.SetActive(false);
		muckedParticle.SetActive(false);
	}

	void Start()
	{
		burnedPrefab = Resources.Load<GameObject>("Burned");
		soakedPrefab = Resources.Load<GameObject>("Soaked");
		gustedPrefab = Resources.Load<GameObject>("Gusted");
		muckedPrefab = Resources.Load<GameObject>("Mucked");

		burnedParticle = GameObject.Instantiate(burnedPrefab, transform.position, Quaternion.identity) as GameObject;
		soakedParticle = GameObject.Instantiate(soakedPrefab, transform.position, Quaternion.identity) as GameObject;
		gustedParticle = GameObject.Instantiate(gustedPrefab, transform.position, Quaternion.identity) as GameObject;
		muckedParticle = GameObject.Instantiate(muckedPrefab, transform.position, Quaternion.identity) as GameObject;

		burnedParticle.transform.SetParent(transform);
		soakedParticle.transform.SetParent(transform);
		gustedParticle.transform.SetParent(transform);
		muckedParticle.transform.SetParent(transform);

		DisableAllParticles();

		if (Runner.Inst == null)
		{
			Runner.Inst = this;
			//Debug.Log(Runner.Inst);
		}

		WorldIndex = 0;

		healthSlider.maxValue = maxHealth;
		healthSlider.value = health;

		Lanes = new GameObject[4,4];

		for(int i = 0; i < Cameras.Length; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				//Lanes[i,j] = Cameras[i].transform.FindChild("Lane Parent").FindChild("Lane ["+j+"]").gameObject;
			}
		}
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
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			int newWorld = WorldIndex - 1;
			if (newWorld == -1)
			{
				PhaseShift(3);
			}
			else
			{
				PhaseShift(newWorld);
			}
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			PhaseShift((WorldIndex + 1) % 4);
		}
		#endregion

		#region Speed Adjustment Effect Changing
		if (Input.GetKey(KeyCode.LeftArrow))
		{
			if (speed < maxSpeed)
			{
				AdjustSpeed(speed);
			}
		}
		if (Input.GetKey(KeyCode.RightArrow))
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

	public void ChangeStatusEffect(int newStatus)
	{
		DisableAllParticles();

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
			burnedParticle.SetActive(true);
			statusUI.text = "Burning";
			burning = false;
		}
		if (newStatus == 2)
		{
			soakedParticle.SetActive(true);
			statusUI.text = "Soaked";
		}
		if (newStatus == 3)
		{
			muckedParticle.SetActive(true);
			statusUI.text = "Mucked";
		}
		if (newStatus == 4)
		{
			gustedParticle.SetActive(true);
			statusUI.text = "Gusted";
		}

		//Debug.Log("Status: " + statusEffect + " to " + newStatus +"\n");
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
			Vector3 relativePos = Cameras[WorldIndex].transform.position - transform.position;
			Debug.DrawLine(Cameras[targetPlane].transform.position, Cameras[targetPlane].transform.position + relativePos, Color.black, 15.0f);
			Vector3 destination = Cameras[targetPlane].transform.position - relativePos;
			transform.position = destination;

			WorldIndex = targetPlane;

			if (statusEffect != 0)
			{
				ChangeStatusEffect(0);
			}

		}
	}
}