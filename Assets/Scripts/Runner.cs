﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Runner : MonoBehaviour
{
	public static Runner Inst;
	public static PlatformerCharacter2D platChar;
	public static Platformer2DUserControl platController;


	public AudioSource burnedAudio;
	public AudioSource soakedAudio;
	public AudioSource muckedAudio;
	public AudioSource gustedAudio;

	private float maxHeight = .7f;
	private float minHeight = .1f;

	private float shiftCooldown = .75f;

	private float[] yAbove = { .9f, .2f, .1f, 0 };
	private float[] yEqual = { .3f, .2f, .1f, 0 };
	private float[] yBelow = { .9f, .8f, .7f, 0 };

	#region Core Attributes
	public bool invulnerable;
	public float invulFrames;
	private float invulLength = .75f;
	private float fallDamage = 10;

	public float[] score;
	private float maxHealth = 100;
	public float health = 100;

	public float speed;
	private float maxSpeed = 15;
	public float MaxSpeed { get { return maxSpeed; } }
	private float minSpeed = 10;

	public AdvancedTimer shiftTimer;

	public int statusEffect = 0;
	public float statusLength = 0.0f;
	public bool burning = false;
	public bool soaked = false;
	public bool mucked = false;
	public bool gusted = false;

	public bool AdvanceRight = false;

	public ForceBlink blinkController;

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
	public Spawner[] Spawners;
	public PlaneMechanics[] PlaneMechanics;
	public CameraKick[] Kicks;
	public Text[] ScoreDisplay;
	public GameObject[,] Lanes;
	public int WorldIndex;
	public int lane;
	#endregion

	#region UI Elements
	//public Text statusUI;
	public Text scoreUI;
	public Slider healthSlider;
	//public Slider speedSlider;
	#endregion

	#region Particle Control
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
	#endregion

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

		shiftTimer = ScriptableObject.CreateInstance<AdvancedTimer>();
		shiftTimer.Init(shiftCooldown,0, true, 0,0, false, false);

		healthSlider.maxValue = maxHealth;
		healthSlider.value = health;

		//speedSlider.maxValue = maxSpeed;
		//speedSlider.minValue = minSpeed;
		//speedSlider.value = speed;

		speed = (maxSpeed + minSpeed) / 2;
		//speed = minSpeed;

		Lanes = new GameObject[4,4];

		Kicks = new CameraKick[4];
		ScoreDisplay = new Text[4];
		for(int i = 0; i < Cameras.Length; i++)
		{
			Kicks[i] = Cameras[i].GetComponent<CameraKick>();

			ScoreDisplay[i] = Cameras[i].transform.FindChild("Camera Canvas").FindChild("Panel").FindChild("Score Text").GetComponent<Text>();
		}

		PhaseShift(Random.Range(0, 3));
		SetCameras();

	}

	public PlaneMechanics GetPlaneMechanics(int planeIndex = -1)
	{
		if (planeIndex == -1)
		{
			//Debug.Log(PlaneMechanics.Length);
			return PlaneMechanics[WorldIndex];
		}
		if (planeIndex > 3)
		{
			Debug.LogError("Invalid Plane Mechanic request: " + planeIndex + "\n");
		}
		return PlaneMechanics[planeIndex];
	}

	void Update()
	{
		GetInput();

		for (int i = 0; i < ScoreDisplay.Length; i++)
		{
			ScoreDisplay[i].text = "" + (int)score[i];
		}

		#region UpdateSpeed
		//speedSlider.value = speed;
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

		#region Status Effects
		statusLength -= Time.deltaTime;
		if (statusEffect != 0 && statusLength <= 0)
		{
			ChangeStatusEffect(0);
		}

		if (statusEffect == 1 && !burning)
		{
			StartCoroutine(BurnDamage());
		}
		if (statusEffect == 2 && !soaked)
		{
			StartCoroutine(SoakedHeal());
		}
		if (statusEffect == 3 && !mucked)
		{
			StartCoroutine(MuckDown());
		}
		if (statusEffect == 4 && !gusted)
		{
			StartCoroutine(GustUp());
		}
		#endregion

		#region Advancement Progression
		if (transform.position.x >= Cameras[WorldIndex].transform.position.x + 5)
		{
			//Debug.Log("Advancing Right\n");
			AdvanceRight = true;
		}
		else
		{
			//Debug.Log("Advancing Right\n");
			AdvanceRight = false;
		}
		#endregion

		shiftTimer.CheckTimer();
		shiftTimer.UpdateTimer(Time.deltaTime);
		//Debug.Log("W Index:" + WorldIndex + "\n");
		score[WorldIndex] += Time.deltaTime;
		
	}

	public void GainScore(float value, int index)
	{
		score[index] += value;
	}

	public void GetInput()
	{
#if !UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
#endif

		#region L/R Lane Shifting
		if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
		{
			if (!shiftTimer.Running)
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
		}
		if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
		{
			if (!shiftTimer.Running)
			{
				PhaseShift((WorldIndex + 1) % 4);
			}
		}
		#endregion

		#region Speed Adjustment Effect Changing
		/*if (Input.GetKey(KeyCode.LeftArrow))
		{
			AdjustSpeed(-2.5f * Time.deltaTime);
		}

		if (Input.GetKey(KeyCode.RightArrow))
		{
			AdjustSpeed(2.5f * Time.deltaTime);
		}*/
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
		//if (Input.GetKeyDown(KeyCode.U))
		//{
		//	PhaseShift(0);
		//}
		//if (Input.GetKeyDown(KeyCode.I))
		//{
		//	PhaseShift(1);
		//}
		//if (Input.GetKeyDown(KeyCode.O))
		//{
		//	PhaseShift(2);
		//}
		//if (Input.GetKeyDown(KeyCode.P))
		//{
		//	PhaseShift(3);
		//}
		#endregion

		//if (Input.GetKeyDown(KeyCode.Tab))
		//{
		//	AdjustHealth(-maxHealth / 100);
		//}
		//if (Input.GetKeyDown(KeyCode.LeftControl))
		//{
		//	score[0] += Random.Range(0, 40);
		//	score[1] += Random.Range(0, 40);
		//	score[2] += Random.Range(0, 40);
		//	score[3] += Random.Range(0, 40);
		//	AdjustHealth(-105);
		//}

		//if (Input.GetKeyDown(KeyCode.Return))
		//{
		//	Cameras[WorldIndex].GetComponent<CameraKick>().KickCameraVariation(Vector3.right, 1.0f);
		//}
	}

	public void SetCameras()
	{
		for (int i = 0; i < Cameras.Length; i++)
		{
			if (i == WorldIndex)
			{
				Cameras[i].rect = new Rect(Cameras[i].rect.x, yEqual[i], Cameras[i].rect.width, maxHeight);
				Cameras[i].orthographicSize = 10;
			}
			else
			{
				Cameras[i].orthographicSize = 5;
			}

			if (i > WorldIndex)
			{
				Cameras[i].rect = new Rect(Cameras[i].rect.x, yAbove[i], Cameras[i].rect.width, minHeight);
			}
			else if (i < WorldIndex)
			{
				Cameras[i].rect = new Rect(Cameras[i].rect.x, yBelow[i], Cameras[i].rect.width, minHeight);
			}
		}
	}

	public void KickPlayerCamera(Vector3 dir, float magnitude, float radVariation = -1, bool allCameras = false)
	{
		if (allCameras)
		{
			for (int i = 0; i < 4; i++)
			{
				KickCamera(dir, magnitude, i, radVariation);
			}
		}
		else
		{
			KickCamera(dir, magnitude, WorldIndex, radVariation);
		}
	}

	private void KickCamera(Vector3 dir, float magnitude, int index, float radVariation = -1)
	{
		Kicks[index].KickCameraVariation(dir, magnitude, 0, 0, radVariation);
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

	public void EndStatusAudio()
	{
		if (burnedAudio)
		{
			burnedAudio.Stop();
		}
		if (soakedAudio)
		{
			soakedAudio.Stop();
		}
		if (muckedAudio)
		{
			muckedAudio.Stop();
		}
		if (gustedAudio)
		{
			gustedAudio.Stop();
		}
	}

	public void ChangeStatusEffect(int newStatus)
	{
		DisableAllParticles();

		if (newStatus == 0)
		{
			//statusUI.text = "";
		}
		else
		{
			statusLength = 8.0f;
		}
		if (newStatus == 1)
		{
			burnedParticle.SetActive(true);
			//statusUI.text = "Burning";
			burnedAudio = AudioManager.Instance.MakeSource("burnedAudio");
			burnedAudio.Play();
			burning = false;
		}
		if (newStatus == 2)
		{
			soakedParticle.SetActive(true);
			//statusUI.text = "Soaked";
			soakedAudio = AudioManager.Instance.MakeSource("soakedAudio");
			soakedAudio.Play();
			soaked = false;
		}
		if (newStatus == 3)
		{
			muckedParticle.SetActive(true);
			//statusUI.text = "Mucked";
			muckedAudio = AudioManager.Instance.MakeSource("muckedAudio");
			muckedAudio.Play();
			mucked = false;
			//speed = minSpeed;

			platChar.flingDirection = new Vector2(Random.Range(-6.0f, 6.0f), -4);
			platChar.flingPlayer = true;
		}
		if (newStatus == 4)
		{
			gustedParticle.SetActive(true);
			//statusUI.text = "Gusted";
			gustedAudio = AudioManager.Instance.MakeSource("gustedAudio");
			gustedAudio.Play();
			gusted = false;
			//speed = maxSpeed;

			platChar.flingDirection = new Vector2(Random.Range(-6.0f, 6.0f), 4);
			platChar.flingPlayer = true;
		}

		//Debug.Log("Status: " + statusEffect + " to " + newStatus +"\n");
		statusEffect = newStatus;

	}

	public void AdjustHealth(float healthAdj, int damageType = -1)
	{
		float kickAmount = 0;
		if (healthAdj < 0)
		{
			blinkController.ExecuteBlink(.01f);

			if (healthAdj < -3)
			{
				StartCoroutine("SlowMotion", .25f);
			}

			kickAmount = healthAdj < -5 ? 1 : .45f;
		}

		//The Soaked status effect damage prevention.
		if (!invulnerable)
		{
			//Debug.Log("Adjusting health by \t" + healthAdj + "\n");
			
			if (kickAmount > 0)
			{
				Cameras[WorldIndex].GetComponent<CameraKick>().KickCameraVariation(Vector3.right, kickAmount);
			}

			health += healthAdj;
			if (health > maxHealth)
			{
				health = maxHealth;
			}
			healthSlider.value = health;

			if (health <= 0)
			{
				Debug.Log("Death!\tScore:\n\t" + (int)score[0] + ", " + (int)score[1] + ", " + (int)score[2] + ", " + (int)score[3] + "\n");

				StartCoroutine("PlayerDeath");

				//Application.LoadLevel(Application.loadedLevel);
			}
		}
		else if(invulnerable)
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
		shiftTimer.Reset(true);
		if (targetPlane >= 0 && targetPlane < 4)
		{
			AudioManager.Instance.MakeSource("PhaseShift").Play();

			transform.SetParent(null);

			Vector3 relativePos = Cameras[WorldIndex].transform.position - transform.position;
			//Debug.DrawLine(Cameras[targetPlane].transform.position, Cameras[targetPlane].transform.position + relativePos, Color.black, 15.0f);
			Vector3 destination = Cameras[targetPlane].transform.position - relativePos;
			transform.position = destination;

			//platChar.m_Rigidbody2D.AddForce(Vector2.right * 200f, ForceMode2D.Impulse);

			WorldIndex = targetPlane;

			if (statusEffect != 0)
			{
				ChangeStatusEffect(0);
			}

			//When the player shifts, they are temporarily invulnerable.
			SetInvulnerability(.5f);

			SetCameras();

			StartCoroutine("MicroPause", .1f);

			Debug.Log(targetPlane + " setting gscale to " + GetPlaneMechanics().gravityScale + "\n" + GetPlaneMechanics().name);
			platChar.mRigidbody2D.gravityScale = GetPlaneMechanics().gravityScale;
		}
	}

	#region Coroutines
	IEnumerator BurnDamage()
	{
		burning = true;
		//AudioManager.Instance.MakeSource("Burn").Play();
		yield return new WaitForSeconds(.50f);

		if (statusEffect == 1)
		{
			AdjustHealth(-.5f);
			burning = false;
		}
	}

	IEnumerator SoakedHeal()
	{
		soaked = true;
		yield return new WaitForSeconds(.50f);

		if (statusEffect == 2)
		{
			//Debug.Log("Healing\n");
			AdjustHealth(.5f);
			soaked = false;
		}
	}

	IEnumerator MuckDown()
	{
		mucked = true;
		//AudioManager.Instance.MakeSource("Burn").Play();
		yield return new WaitForSeconds(Random.Range(.65f, 1.25f));

		//Debug.Log("Mucking down\n");
		if (statusEffect == 3)
		{
			platChar.flingDirection = new Vector2(Random.Range(-5.0f, 0.0f), -2);
			platChar.flingPlayer = true;
			mucked = false;
		}
	}

	IEnumerator GustUp()
	{
		gusted = true;
		//AudioManager.Instance.MakeSource("Burn").Play();
		yield return new WaitForSeconds(Random.Range(.85f, 1.75f));
		
		//Debug.Log("Gusting Up\n");
		if (statusEffect == 4)
		{
			platChar.flingDirection = new Vector2(Random.Range(0.0f, 5.0f), 3);
			platChar.flingPlayer = true;
			gusted = false;
		}
	}

	public IEnumerator PlayerDeath()
	{
		PlayerPrefs.SetInt("FireScore", (int)score[0]);
		PlayerPrefs.SetInt("WaterScore", (int)score[1]);
		PlayerPrefs.SetInt("EarthScore", (int)score[2]);
		PlayerPrefs.SetInt("AirScore", (int)score[3]);

		AudioManager.Instance.maxMusicVol = 0;
		float adjusted = 0.01f;
		Time.timeScale = adjusted;
		yield return new WaitForSeconds(3.5f * adjusted);
		Time.timeScale = 1f;

		AudioManager.Instance.maxMusicVol = 1;
		Application.LoadLevel(Application.loadedLevel + 1);
	}

	public IEnumerator MicroPause(float time)
	{
		float adjusted = 0.01f;
		Time.timeScale = adjusted;
		yield return new WaitForSeconds(time * adjusted);
		Time.timeScale = 1f;
	}

	public IEnumerator SlowMotion(float time)
	{
		float adjusted = 0.50f;
		Time.timeScale = adjusted;
		yield return new WaitForSeconds(time * adjusted);
		Time.timeScale = 1f;
	}
	#endregion
}