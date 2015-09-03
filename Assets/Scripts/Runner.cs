using UnityEngine;
using System.Collections;

public class Runner : MonoBehaviour
{
	public GameObject[] Cameras;
	public GameObject[,] Lanes;
	public int lane;
	public bool immune;
	public int WorldIndex;

	void Start () 
	{
		WorldIndex = 0;
		Lanes = new GameObject[4,4];

		for(int i = 0; i < Cameras.Length; i++)
		{
			for(int j = 0; j < 4; j++)
			{
				Lanes[i,j] = Cameras[i].transform.FindChild("Lane Parent").FindChild("Lane ["+j+"]").gameObject;
			}
		}
	}

	void Update() 
	{
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
	}

	public void StepIntoLane(int newLane)
	{
		lane = newLane;
		transform.position = Lanes[WorldIndex, newLane].transform.position - Vector3.forward;
	}

	public void PhaseShift(int targetPlane)
	{
		if (targetPlane >= 0 && targetPlane < 4)
		{
			WorldIndex = targetPlane;

			transform.position = Lanes[WorldIndex, lane].transform.position - Vector3.forward;

		}
	}


}