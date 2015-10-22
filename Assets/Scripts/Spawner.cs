using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	#region Spawner Attributes
	#region Prefabs
	public GameObject platformPrefab;
	public GameObject platformLongPrefab;
	public GameObject togglePrefab;
	public GameObject obstaclePrefab;
	public GameObject[] forcingObstaclePrefab;
	public GameObject parallaxPrefab;
	#endregion

	public int worldIndex;

	#region World Enabled
	public static bool fireEnabled = true;
	public static bool waterEnabled = true;
	public static bool earthEnabled = true;
	public static bool airEnabled = true;
	#endregion

	#region Reference Objects
	public GameObject ResetPoint;
	public GameObject spawnPosition;
	public GameObject CreationParent;
	#endregion

	#region Spawned World Objects
	public List<GameObject> platforms;
	public List<Toggle> toggles;
	public List<Obstacle> obstacles;
	public List<ParallaxObject> parallax;
	#endregion

	#region Timers & Base Frequencies
	[Header("Timers & Frequencies")]
	public float platformFreq = .75f;
	public float obstacleFreq = 4.5f;
	public float toggleFreq = 3.0f;

	private float platformTimer = 0;
	private float obstacleTimer = 0;
	private float toggleTimer = 0;
	//public float paraTimer = 0;
	//public float paraFreq = 3.0f;
	#endregion

	#region World Speed Modifers
	[Header("Speed Modifiers")]
	public float playerAcceleration = 0;
	public float inactiveWorldTimeAdj = .25f;
	#endregion

	#region Height Generation Variables
	[Header("Height Generation Variables")]
	public float greatestHeight = 9;
	public float lowestHeight = -1.0f;
	public float heightVarMax = 2;
	public float heightVarMin = -2f;

	public float obstacleAboveMax = 3.0f;
	public float obstacleAboveMin = 2f;

	public float toggleAboveMax = 7.5f;
	public float toggleAboveMin = 2f;

	private float currentHeight = 5;
	private float startingHeight = 5;
	#endregion

	#region
	[Header("Tilt Ranges")]
	public float tiltMax = 15f;
	public float tiltMin = -15f;

	public float tiltDeadPos = 5;
	public float tiltDeadNeg = -5;
	#endregion

	#region Object Type Frequencies
	[Header("Frequency Variables")]
	public int maxPlatformBound = 30;
	public int flatPlatformBound = 10; //0 to 9
	public int tiltPlatformBound = 20; //10 to 20
	public int longPlatformBound = 27; //20 to 27
	public int gapPlatformBound = 30; // 27-30

	public int maxObstacleBound = 30;
	public int normalObstacleBound = 28;

	private int forceSwitchCounter;
	public int obstacleBoundIncrement = 2;
	#endregion

	#region Platform Time Adjustments
	[Header("Time Parameters")]
	public float platformDelayMultiplier = 20;
	public float longPlatformDelayMultiplier = 24;
	public float platformTimeMin = -.25f;
	public float platformTimeMax = .25f;
	public float longPlatformTimeMin = 2.0f;
	public float LongPlatformTimeMax = 2.75f;

	public float toggleTimeMin = -2.1f;
	public float toggleTimeMax = 2.1f;

	public float obstacleTimeMin = -2.1f;
	public float obstacleTimeMax = 2.1f;
	#endregion
	#endregion

	#region Start
	void Start() 
	{
		platformPrefab = Resources.Load<GameObject>("Objects/Platform");
		platformLongPrefab = Resources.Load<GameObject>("Objects/PlatformLong");
		togglePrefab = Resources.Load<GameObject>("Objects/Toggle");
		obstaclePrefab = Resources.Load<GameObject>("Objects/Obstacle");
		parallaxPrefab = Resources.Load<GameObject>("Objects/Parallax");

		forcingObstaclePrefab = new GameObject[4];

		forcingObstaclePrefab[0] = Resources.Load<GameObject>("Objects/FireForce");
		forcingObstaclePrefab[1] = Resources.Load<GameObject>("Objects/WaterForce");
		forcingObstaclePrefab[2] = Resources.Load<GameObject>("Objects/EarthForce");
		forcingObstaclePrefab[3] = Resources.Load<GameObject>("Objects/AirForce");

		platforms = new List<GameObject>();
		toggles = new List<Toggle>();
		obstacles = new List<Obstacle>();
		parallax = new List<ParallaxObject>();

		toggleTimer = toggleFreq;
		obstacleTimer = obstacleFreq;
		//paraTimer = paraFreq;
		//Create base platforms to use?
		
		CreationParent = new GameObject();
		CreationParent.name = name + " folder";

		StarterPlatforms();
	}
	private void StarterPlatforms()
	{
		GameObject go;
		for (int i = 0; i < 2; i++)
		{
			go = GameObject.Instantiate(platformPrefab, spawnPosition.transform.position + Vector3.left * (100 - i * 10), Quaternion.identity) as GameObject;

			go.transform.SetParent(CreationParent.transform);

			platforms.Add(go);
		}
		for (int i = 2; i < 10; i++)
		{
			if (i % 2 == 0)
			{
				CreatePlatform(platformPrefab, false, (100 - i * 10));
			}
		}
	}
	#endregion

	private void AdjustSpawningHeight()
	{
		float upperBound, lowerBound = 0;
		if (currentHeight + heightVarMax > greatestHeight)
		{
			upperBound = greatestHeight - currentHeight;
		}
		else
		{
			upperBound = heightVarMax;
		}
		if (currentHeight + heightVarMin < lowestHeight)
		{
			lowerBound = currentHeight + lowestHeight;
		}
		else
		{
			lowerBound = heightVarMin;
		}

		float heightVariation = Random.Range(lowerBound, upperBound);

		currentHeight += heightVariation;

		//Debug.Log("New height is: " + currentHeight + "\n");
	}

	#region Creating Objects: Platforms, Toggles, Obstacles
	private GameObject CreatePlatform(GameObject prefab, bool tilt = true, float leftOffset = 0)
	{
		AdjustSpawningHeight();

		float randHeight = 0;

		Vector3 verOffset = Vector3.up * randHeight;
		verOffset = Vector3.up * (currentHeight);
		Vector3 horOffset = Vector3.left * leftOffset;

		GameObject go = GameObject.Instantiate(prefab, spawnPosition.transform.position + verOffset + horOffset, Quaternion.identity) as GameObject;

		go.transform.SetParent(CreationParent.transform);

		if (tilt)
		{
			float tiltAmt = Random.Range(tiltMin, tiltMax);
			if(tiltAmt < tiltDeadPos && tiltAmt > 0)
			{
				tiltAmt = tiltDeadPos;
			}
			else if(tiltAmt > tiltDeadNeg && tiltAmt < 0)
			{
				tiltAmt = tiltDeadNeg;
			}
			go.transform.Rotate(Vector3.forward, tiltAmt);
		}

		platforms.Add(go);

		return go;
	}

	private void CreateToggle()
	{
		float randHeight = 0;
		randHeight = Random.Range(toggleAboveMin, toggleAboveMax) + currentHeight;
		GameObject go = GameObject.Instantiate(togglePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		Toggle tog = go.GetComponent<Toggle>();

		int random = Random.Range(0, 4);

		tog.affiliation = (Toggle.Element)random;

		toggles.Add(tog);
	}

	private void CreateObstacle()
	{
		float randHeight = 0;
		randHeight = Random.Range(obstacleAboveMin, obstacleAboveMax) + currentHeight;

		GameObject go = GameObject.Instantiate(obstaclePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		Obstacle obst = go.GetComponent<Obstacle>();

		int random = Random.Range(0, 4);

		obst.affiliation = (Obstacle.Element)random;

		obstacles.Add(obst);
	}

	private void CreateForcingObstacle()
	{
		if (worldIndex != 3)
		{
			GameObject go = GameObject.Instantiate(forcingObstaclePrefab[worldIndex], spawnPosition.transform.position, Quaternion.identity) as GameObject;
			go.transform.SetParent(CreationParent.transform);
			Obstacle obst = go.GetComponent<Obstacle>();

			obst.obType = Obstacle.ObstacleType.ForceSwitch;

			obst.affiliation = (Obstacle.Element)worldIndex;

			obstacles.Add(obst);
		}
		else
		{
			AdjustPlatformClock(1);
		}
	}

	private void CreateParallaxObject(float timeAdjustment)
	{
		float randHeight = Random.Range(0, 15);
		float randDepth = Random.Range(1, 2);
		GameObject go = GameObject.Instantiate(parallaxPrefab, spawnPosition.transform.position + (Vector3.up * randHeight) + Vector3.forward * randDepth, Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		go.transform.localScale = new Vector3(8, 8, 8);
		ParallaxObject para = go.GetComponent<ParallaxObject>();

		para.zBack = randDepth;

		//para.affiliation = (Obstacle.Element)random;

		parallax.Add(para);
	}
	#endregion

	#region Move Spawned Objects
	private void MovePlatforms(float timeAdjustment)
	{
		for (int i = 0; i < platforms.Count; i++)
		{
			//Do we want different speed obstacles?

			platforms[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + playerAcceleration);

			if (platforms[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = platforms[i];

				//Remove this one
				platforms.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don'timerPercentage skip an obstacle.
				i--;
			}
		}
	}

	private void MoveToggles(float timeAdjustment)
	{
		for (int i = 0; i < toggles.Count; i++)
		{
			//Do we want different speed obstacles?

			toggles[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + playerAcceleration);

			if (toggles[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = toggles[i].gameObject;

				//Remove this one
				toggles.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don'timerPercentage skip an obstacle.
				i--;
			}
		}
	}

	private void MoveObstacles(float timeAdjustment)
	{
		for (int i = 0; i < obstacles.Count; i++)
		{
			//Do we want different speed obstacles?

			obstacles[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + playerAcceleration);

			if (obstacles[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = obstacles[i].gameObject;

				//Remove this one
				obstacles.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don'timerPercentage skip an obstacle.
				i--;
			}
		}
	}

	private void MoveParallax(float timeAdjustment)
	{
		for (int i = 0; i < parallax.Count; i++)
		{
			float rateAdj = parallax[i].zBack * 5;
			parallax[i].transform.position -= Vector3.right * timeAdjustment * rateAdj;
		}
	}
	#endregion

	public void AdjustPlatformClock(int platformCase)
	{
		if (platformCase == 0)
		{
			platformTimer = platformFreq / Runner.Inst.speed * platformDelayMultiplier + Random.Range(platformTimeMin, platformTimeMax);
		}
		else if (platformCase == 1)
		{
			platformTimer = platformFreq / Runner.Inst.speed * longPlatformDelayMultiplier + Random.Range(longPlatformTimeMin, LongPlatformTimeMax);
		}
		else if (platformCase == 2)
		{
			platformTimer = platformFreq / Runner.Inst.speed * platformDelayMultiplier + Random.Range(platformTimeMin, platformTimeMax);
		}
	}

	private void UpdateWorld(float timeAdjustment)
	{
		playerAcceleration += timeAdjustment / 10;

		#region Platform Creation
		platformTimer -= timeAdjustment;
		if (platformTimer <= 0)
		{
			GameObject whichPlatform = platformPrefab;
			int randCase = Random.Range(0, maxPlatformBound);
			if (randCase <= flatPlatformBound)
			{
				whichPlatform = platformPrefab;
				AdjustPlatformClock(0);
				CreatePlatform(whichPlatform, false);
			}
			else if (randCase <= tiltPlatformBound)
			{
				whichPlatform = platformPrefab;
				AdjustPlatformClock(0);

				CreatePlatform(whichPlatform, true);
			}
			else if (randCase <= longPlatformBound)
			{
				whichPlatform = platformLongPrefab;
				AdjustPlatformClock(1);

				CreatePlatform(whichPlatform, false);
			}
			else if(randCase <= gapPlatformBound)
			{
				AdjustPlatformClock(2);

			}
		}
		#endregion

		#region Toggle Creation
		toggleTimer -= timeAdjustment;
		if (toggleTimer <= 0)
		{
			toggleTimer = toggleFreq + Random.Range(toggleTimeMin, toggleTimeMax);
			CreateToggle();
		}
		#endregion

		#region Obstacle Creation
		obstacleTimer -= timeAdjustment;
		if (obstacleTimer <= 0)
		{
			obstacleTimer = obstacleFreq + Random.Range(obstacleTimeMin, obstacleTimeMax);

			int randCase = Random.Range(0, maxObstacleBound);
			randCase += (worldIndex == Runner.Inst.WorldIndex) ? forceSwitchCounter : 0;
			//Debug.Log(randCase + "\n");
			if (randCase < normalObstacleBound)
			{
				forceSwitchCounter += obstacleBoundIncrement;
				CreateObstacle();
			}
			else
			{
				forceSwitchCounter = 0;
				CreateForcingObstacle();
			}
		}
		#endregion

		#region Parallax Creation
		//paraTimer -= timeAdjustment;
		//if (paraTimer <= 0)
		//{
			//paraTimer = paraFreq + Random.Range(-2.1f, 2.1f);
			//CreateParallaxObject(timeAdjustment);
		//}
		#endregion

		//if (Runner.Inst.AdvanceRight)
		//{
		MovePlatforms(timeAdjustment);
		MoveToggles(timeAdjustment);
		MoveObstacles(timeAdjustment);
		//MoveParallax(timeAdjustment);
		//}
	}

	void Update()
	{
		if (Runner.Inst.WorldIndex == worldIndex)
		{
			UpdateWorld(Time.deltaTime);
		}
		else
		{
			UpdateWorld(Time.deltaTime * inactiveWorldTimeAdj);
		}
	}
}
