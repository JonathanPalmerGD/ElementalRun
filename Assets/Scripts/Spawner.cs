using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
	public GameObject platformPrefab;
	public GameObject togglePrefab;
	public GameObject obstaclePrefab;
	public int worldIndex;

	public static bool fireEnabled = true;
	public static bool waterEnabled = true;
	public static bool earthEnabled = true;
	public static bool airEnabled = true;

	public GameObject ResetPoint;
	public GameObject spawnPosition;

	public List<GameObject> platforms;
	public List<Toggle> toggles;
	public List<Obstacle> obstacles;

	public float platformTimer = 0;
	private float platformFreq = .75f;
	public float obstacleTimer = 0;
	private float obstacleFreq = 4.5f;
	public float toggleTimer = 0;
	private float toggleFreq = 3.0f;

	void Start() 
	{
		platformPrefab = Resources.Load<GameObject>("Objects/Platform");
		togglePrefab = Resources.Load<GameObject>("Objects/Toggle");
		obstaclePrefab = Resources.Load<GameObject>("Objects/Obstacle");

		platforms = new List<GameObject>();
		toggles = new List<Toggle>();
		obstacles = new List<Obstacle>();

		toggleTimer = toggleFreq;
		obstacleTimer = obstacleFreq;
		//Create base platforms to use?

		StarterPlatforms();
	}

	private void StarterPlatforms()
	{
		GameObject go;
		for (int i = 0; i < 7; i++)
		{
			go = GameObject.Instantiate(platformPrefab, spawnPosition.transform.position + Vector3.left * (75 - i * 10), Quaternion.identity) as GameObject;
			platforms.Add(go);
		}
	}

	private void CreatePlatform()
	{
		float randHeight = Random.Range(-.5f, 6.0f);
		GameObject go = GameObject.Instantiate(platformPrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;

		if (randHeight < 3)
		{
			go.transform.Rotate(Vector3.forward, Random.Range(-15, 15));
		}
		else
		{
			go.transform.Rotate(Vector3.forward, Random.Range(0, 15));
		}

		platforms.Add(go);
	}

	private void CreateToggle()
	{
		float randHeight = Random.Range(0, 10);
		GameObject go = GameObject.Instantiate(togglePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		Toggle tog = go.GetComponent<Toggle>();

		int random = Random.Range(0, 4);

		tog.affiliation = (Toggle.Element)random;

		toggles.Add(tog);
	}

	private void CreateObstacle()
	{
		float randHeight = Random.Range(0, 10);
		GameObject go = GameObject.Instantiate(obstaclePrefab, spawnPosition.transform.position + (Vector3.up * randHeight), Quaternion.identity) as GameObject;
		Obstacle obst = go.GetComponent<Obstacle>();

		int random = Random.Range(0, 4);

		obst.affiliation = (Obstacle.Element)random;

		obstacles.Add(obst);
	}

	private void MovePlatforms()
	{
		for (int i = 0; i < platforms.Count; i++)
		{
			//Do we want different speed obstacles?

			platforms[i].transform.position -= Vector3.right * Time.deltaTime * Runner.Inst.speed;

			if (platforms[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = platforms[i];

				//Remove this one
				platforms.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don't skip an obstacle.
				i--;
			}
		}
	}

	private void MoveToggles()
	{
		for (int i = 0; i < toggles.Count; i++)
		{
			//Do we want different speed obstacles?

			toggles[i].transform.position -= Vector3.right * Time.deltaTime * Runner.Inst.speed;

			if (toggles[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = toggles[i].gameObject;

				//Remove this one
				toggles.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don't skip an obstacle.
				i--;
			}
		}
	}

	private void MoveObstacles()
	{
		for (int i = 0; i < obstacles.Count; i++)
		{
			//Do we want different speed obstacles?

			obstacles[i].transform.position -= Vector3.right * Time.deltaTime * Runner.Inst.speed;

			if (obstacles[i].transform.position.x < ResetPoint.transform.position.x)
			{
				GameObject go = obstacles[i].gameObject;

				//Remove this one
				obstacles.RemoveAt(i);

				GameObject.Destroy(go);

				//Decrement by one to make sure we don't skip an obstacle.
				i--;
			}
		}
	}

	void Update()
	{
		if (Runner.Inst.WorldIndex == worldIndex)
		{
			platformTimer -= Time.deltaTime;
			if (platformTimer <= 0)
			{
				platformTimer = platformFreq / Runner.Inst.speed * 20;// +Random.Range(-0.2f, 0.7f);
				CreatePlatform();
			}

			toggleTimer -= Time.deltaTime;
			if (toggleTimer <= 0)
			{
				toggleTimer = toggleFreq + Random.Range(-2.1f, 2.1f);
				CreateToggle();
			}

			obstacleTimer -= Time.deltaTime;
			if (obstacleTimer <= 0)
			{
				obstacleTimer = obstacleFreq + Random.Range(-2.1f, 2.1f);
				CreateObstacle();
			}

			MovePlatforms();
			MoveToggles();
			MoveObstacles();
		}
	}
}
