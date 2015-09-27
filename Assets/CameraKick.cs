using UnityEngine;
using System.Collections;

public class CameraKick : MonoBehaviour 
{
	public Vector3 intendedPosition;
	public float kickMin = 1;
	public float kickMax = 2;
	private float recoilRate = .25f;

	void Start() 
	{
		intendedPosition = transform.position;
	}
	
	void Update() 
	{
		
	}
	
	public void KickCamera(Vector3 direction, float radianVariation)
	{

	}
}
