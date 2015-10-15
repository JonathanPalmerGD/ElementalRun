using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	public GameObject platformPrefab;
	public GameObject platformLongPrefab;
	public GameObject togglePrefab;
	public GameObject obstaclePrefab;
	public GameObject[] forcingObstaclePrefab;
	public GameObject parallaxPrefab;
	public int worldIndex;

	public static bool fireEnabled = true;
	public static bool waterEnabled = true;
	public static bool earthEnabled = true;
	public static bool airEnabled = true;

	public GameObject ResetPoint;
	public GameObject spawnPosition;
	public GameObject CreationParent;

	public List<GameObject> platforms;
	public List<Toggle> toggles;
	public List<Obstacle> obstacles;
	public List<ParallaxObject> parallax;

	public float speedModifier = 0;

	public float platformTimer = 0;
	private float platformFreq = .75f;
	public float obstacleTimer = 0;
	private float obstacleFreq = 4.5f;
	public float toggleTimer = 0;
	private float toggleFreq = 3.0f;
	public float paraTimer = 0;
	private float paraFreq = 3.0f;

	private int counter;

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
		paraTimer = paraFreq;
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

	private GameObject CreatePlatform(GameObject prefab, bool tilt = true, float leftOffset = 0)
	{
		float randHeight = Random.Range(-1.5f, 9.0f);

		Vector3 verOffset = Vector3.up * randHeight;
		Vector3 horOffset = Vector3.left * leftOffset;

		GameObject go = GameObject.Instantiate(prefab, spawnPosition.transform.position + verOffset + horOffset, Quaternion.identity) as GameObject;

		go.transform.SetParent(CreationParent.transform);

		if (tilt)
		{
			if (randHeight < 3)
			{
				go.transform.Rotate(Vector3.forward, Random.Range(-15, 15));
			}
			else
			{
				go.transform.Rotate(Vector3.forward, Random.Range(0, 15));
			}
		}

		platforms.Add(go);

		return go;
	}

	private void CreateToggle()
	{
		float randHeight = Random.Range(0, 10);
		GameObject go = GameObject.Instantiate(togglePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		Toggle tog = go.GetComponent<Toggle>();

		int random = Random.Range(0, 4);

		tog.affiliation = (Toggle.Element)random;

		toggles.Add(tog);
	}

	private void CreateObstacle()
	{
		float randHeight = Random.Range(0, 10);
		GameObject go = GameObject.Instantiate(obstaclePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		Obstacle obst = go.GetComponent<Obstacle>();

		int random = Random.Range(0, 4);

		obst.affiliation = (Obstacle.Element)random;

		obstacles.Add(obst);
	}

	private void CreateForcingObstacle()
	{
		GameObject go = GameObject.Instantiate(forcingObstaclePrefab[worldIndex], spawnPosition.transform.position, Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		Obstacle obst = go.GetComponent<Obstacle>();

		obst.obType = Obstacle.ObstacleType.ForceSwitch;

		obst.affiliation = (Obstacle.Element)worldIndex;

		obstacles.Add(obst);
	}

	private void CreateParallaxObject(float timeAdjustment)
	{
		float randHeight = Random.Range(0, 15);
		float randDepth = Random.Range(1, 5);
		GameObject go = GameObject.Instantiate(parallaxPrefab, spawnPosition.transform.position + (Vector3.up * randHeight) + Vector3.forward * randDepth, Quaternion.identity) as GameObject;
		go.transform.SetParent(CreationParent.transform);
		go.transform.localScale = new Vector3(8, 8, 8);
		ParallaxObject para = go.GetComponent<ParallaxObject>();

		para.zBack = randDepth;

		//para.affiliation = (Obstacle.Element)random;

		parallax.Add(para);
	}

	private void MovePlatforms(float timeAdjustment)
	{
		for (int i = 0; i < platforms.Count; i++)
		{
			//Do we want different speed obstacles?

			platforms[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + speedModifier);

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

			toggles[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + speedModifier);

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

			obstacles[i].transform.position -= Vector3.right * timeAdjustment * (Runner.Inst.speed + speedModifier);

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

	private void UpdateWorld(float timeAdjustment)
	{
		speedModifier += timeAdjustment / 10;

		#region Platform Creation
		platformTimer -= timeAdjustment;
		if (platformTimer <= 0)
		{
			GameObject whichPlatform = platformPrefab;
			int randCase = Random.Range(0, 30);
			if (randCase < 24)
			{
				whichPlatform = platformPrefab;
				platformTimer = platformFreq / Runner.Inst.speed * 20 + Random.Range(-0.25f, 0.25f);
				CreatePlatform(whichPlatform, true);
			}
			else if (randCase < 27)
			{
				whichPlatform = platformLongPrefab;
				platformTimer = platformFreq / Runner.Inst.speed * 24 + Random.Range(1.25f, 2.05f);
				CreatePlatform(whichPlatform, false);
			}
			else
			{
				platformTimer = platformFreq / Runner.Inst.speed * 20 + Random.Range(-0.25f, 0.25f);
			}

			//platformTimer = platformFreq / Runner.Inst.speed * 18 + Random.Range(-0.25f, 0.25f);
		}
		#endregion

		#region Toggle Creation
		toggleTimer -= timeAdjustment;
		if (toggleTimer <= 0)
		{
			toggleTimer = toggleFreq + Random.Range(-2.1f, 2.1f);
			CreateToggle();
		}
		#endregion

		#region Obstacle Creation
		obstacleTimer -= timeAdjustment;
		if (obstacleTimer <= 0)
		{
			obstacleTimer = obstacleFreq + Random.Range(-2.1f, 2.1f);

			int randCase = Random.Range(0, 30);
			randCase += (worldIndex == Runner.Inst.WorldIndex) ? counter : 0;
			//Debug.Log(randCase + "\n");
			if (randCase < 28)
			{
				counter += 2;
				CreateObstacle();
			}
			else
			{
				counter = 0;
				CreateForcingObstacle();
			}
		}
		#endregion

		#region Parallax Creation
		paraTimer -= timeAdjustment;
		if (paraTimer <= 0)
		{
			paraTimer = paraFreq + Random.Range(-2.1f, 2.1f);
			//CreateParallaxObject(timeAdjustment);
		}
		#endregion

		//if (Runner.Inst.AdvanceRight)
		//{
		MovePlatforms(timeAdjustment);
		MoveToggles(timeAdjustment);
		MoveObstacles(timeAdjustment);
		MoveParallax(timeAdjustment);
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
			UpdateWorld(Time.deltaTime * .25f);
		}
	}
}
