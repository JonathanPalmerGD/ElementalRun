using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Runner : MonoBehaviour
{
	public static Runner Inst;
	public static PlatformerCharacter2D platChar;
	public static Platformer2DUserControl platController;
	#region Core Attributes
	public bool invulnerable;
	public float invulFrames;
	private float invulLength = .75f;
	private float fallDamage = 10;

	public float[] score;
	private float maxHealth = 100;
	public float health = 100;

	public float speed;
	private float maxSpeed = 25;
	public float MaxSpeed { get { return maxSpeed; } }
	private float minSpeed = 15;

	public int statusEffect = 0;
	public float statusLength = 0.0f;
	public bool burning = false;

	private GameObject burnedPrefab;
	private GameObject soakedPrefab;
	private GameObject gustedPrefab;
	private GameObject muckedPrefab;

	private GameObject invulPrefab;


	private GameObject burnedParticle;
	private GameObject soakedParticle;
	private GameObject gustedParticle;
	private GameObject muckedParticle;

	private GameObject invulParticle;
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

	private void SetupParticles()
	{
		burnedPrefab = Resources.Load<GameObject>("Burned");
		soakedPrefab = Resources.Load<GameObject>("Soaked");
		gustedPrefab = Resources.Load<GameObject>("Gusted");
		muckedPrefab = Resources.Load<GameObject>("Mucked");

		invulPrefab = Resources.Load<GameObject>("Invul");

		burnedParticle = GameObject.Instantiate(burnedPrefab, transform.position, Quaternion.identity) as GameObject;
		soakedParticle = GameObject.Instantiate(soakedPrefab, transform.position, Quaternion.identity) as GameObject;
		gustedParticle = GameObject.Instantiate(gustedPrefab, transform.position, Quaternion.identity) as GameObject;
		muckedParticle = GameObject.Instantiate(muckedPrefab, transform.position, Quaternion.identity) as GameObject;

		invulParticle = GameObject.Instantiate(invulPrefab, transform.position, Quaternion.identity) as GameObject;

		burnedParticle.transform.SetParent(transform);
		soakedParticle.transform.SetParent(transform);
		gustedParticle.transform.SetParent(transform);
		muckedParticle.transform.SetParent(transform);

		invulParticle.transform.SetParent(transform);
	}

	private void DisableAllParticles()
	{
		burnedParticle.SetActive(false);
		soakedParticle.SetActive(false);
		gustedParticle.SetActive(false);
		muckedParticle.SetActive(false);

		invulParticle.SetActive(false);
	}

	void Start()
	{
		platChar = GetComponent<PlatformerCharacter2D>();
		platController = GetComponent<Platformer2DUserControl>();

		SetupParticles();
		DisableAllParticles();

		if (Runner.Inst == null)
		{
			Runner.Inst = this;
			//Debug.Log(Runner.Inst);
		}

		score = new float[]{0.0f,0.0f,0.0f,0.0f};

		WorldIndex = 0;

		healthSlider.maxValue = maxHealth;
		healthSlider.value = health;

		speedSlider.maxValue = maxSpeed;
		speedSlider.minValue = minSpeed;
		speedSlider.value = speed;

		speed = 5;

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

		#region UpdateSpeed
		speedSlider.value = speed;
		#endregion

		#region Invulnerability
		if (invulnerable)
		{
			invulFrames -= Time.deltaTime;

			if (invulFrames <= 0)
			{
				EndInvulnerability();
			}
		}
		#endregion

		statusLength -= Time.deltaTime;
		if (statusEffect != 0 && statusLength <= 0)
		{
			ChangeStatusEffect(0);
		}

		if (statusEffect == 1 && !burning)
		{
			StartCoroutine(BurnDamage());
		}

		Debug.Log("W Index:" + WorldIndex + "\n");
		score[WorldIndex] += Time.deltaTime;
		
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
		if(Input.GetMouseButtonDown(1))
		{
			platController.m_ForceJump = true;
		}

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
			AdjustSpeed(-2.5f * Time.deltaTime);

		}
		if (Input.GetKey(KeyCode.RightArrow))
		{
			AdjustSpeed(2.5f * Time.deltaTime);
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

		if (adjustment > 0)
		{
			if (statusEffect != 3)
			{
				speed += adjustment;

				if (speed > maxSpeed)
				{
					speed = maxSpeed;
				}
			}
		}
		else
		{
			if (statusEffect != 4)
			{
				speed += adjustment;

				if (speed < minSpeed)
				{
					speed = minSpeed;
				}
			}
		}
		if (speed + adjustment <= maxSpeed)
		{
			speed += adjustment;
		}
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
			speed = minSpeed;
		}
		if (newStatus == 4)
		{
			gustedParticle.SetActive(true);
			statusUI.text = "Gusted";
			speed = maxSpeed;
		}

		//Debug.Log("Status: " + statusEffect + " to " + newStatus +"\n");
		statusEffect = newStatus;

	}

	public void AdjustHealth(float healthAdj)
	{
		//The Soaked status effect damage prevention.
		if (statusEffect != 2 || invulnerable)
		{
			health += healthAdj;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			healthSlider.value = health;

			if (health <= 0)
			{
				Debug.Log("Death!\tScore:\n\t" + score[0] + ", " + score[1] + ", " + score[2] + ", " + score[3] + "\n");
				Application.LoadLevel(Application.loadedLevel);
			}
		}
		else
		{
			statusEffect = 0;
		}
	}

	/// <summary>
	/// Whether or not to trigger the invulnerability frames.
	/// </summary>
	/// <param name="invulDurationOverride">Provide a >0 value to override the default .5f seconds</param>
	public void SetInvulnerability(float invulDurationOverride = -1)
	{
		invulnerable = true;
		if (invulDurationOverride > 0)
		{
			invulFrames = invulDurationOverride;
		}
		else
		{
			invulFrames = invulLength;
		}

		invulParticle.SetActive(true);

		// Maybe give some sort of invulnerability appeance?
	}

	private void EndInvulnerability()
	{
		invulnerable = false;

		invulFrames = 0;

		invulParticle.SetActive(false);
		//End invul appearance?
	}

	public void FellBelow()
	{
		//Invul stops falling below damage
		if (statusEffect != 2 && !invulnerable)
		{
			AdjustHealth(-fallDamage);

			//We could choose not to give invul frames for falling.
			SetInvulnerability();
		}
		else if(statusEffect == 2)
		{
			ChangeStatusEffect(0);
		}

		//Knock the player upwards;
		platController.m_ForceJump = true;

		//We could apply a status effect when you fall?
		//ChangeStatusEffect(WorldIndex);
	}

	public void PhaseShift(int targetPlane)
	{
		if (targetPlane >= 0 && targetPlane < 4)
		{
			transform.SetParent(null);

			Vector3 relativePos = Cameras[WorldIndex].transform.position - transform.position;
			//Debug.DrawLine(Cameras[targetPlane].transform.position, Cameras[targetPlane].transform.position + relativePos, Color.black, 15.0f);
			Vector3 destination = Cameras[targetPlane].transform.position - relativePos;
			transform.position = destination;

			WorldIndex = targetPlane;

			if (statusEffect != 0)
			{
				ChangeStatusEffect(0);
			}

			//When the player shifts, they are temporarily invulnerable.
			SetInvulnerability(.5f);

		}
	}
}