using UnityEngine;
using System.Collections;

public class CameraKick : MonoBehaviour 
{
	public Vector3 intendedPosition;
	public Vector3 kickedPosition;
	public float kickMin = .6f;
	public float kickMax = 1.2f;
	public float angleVariation = 20;
	private float recoverDur = .05f;
	public bool recoiling = false;
	public bool kicking = false;
	public AdvancedTimer kickTimer;

	void Start() 
	{
		intendedPosition = transform.position;
		kickTimer = ScriptableObject.CreateInstance<AdvancedTimer>();

		kickTimer.Init(recoverDur);
		kickTimer.Stop();
		kickTimer.ContinuousReset = false;
	}
	
	void Update() 
	{
		kickTimer.UpdateTimer(Time.deltaTime);

		//if (Input.GetKeyDown(KeyCode.Return))
		//{
		//	KickCamera(Vector3.right, angleVariation);
		//}

		if (kicking)
		{
			if (recoiling)
			{
				if (kickTimer.Running)
				{
					float timerPercentage = (1 - kickTimer.counter / kickTimer.countFromValue);
					//Debug.Log("Recoiling\nTimer running: " + timerPercentage);
					transform.position = Vector3.Lerp(intendedPosition, kickedPosition, timerPercentage);
				}
				if(kickTimer.CheckTimer())
				{
					transform.position = kickedPosition;

					//Debug.Log("Recoil Complete\nTimer running: " + kickTimer.counter + "\t" + kickTimer.CheckTimer());
					recoiling = false;
					kickTimer.Reset(true);
					kickTimer.Start();
				}
			}

			if(!recoiling)
			{
				if (kickTimer.Running)
				{
					float timerPercentage = (1 - kickTimer.counter / kickTimer.countFromValue);
					transform.position = Vector3.Lerp(kickedPosition, intendedPosition, timerPercentage);
				}
				if(kickTimer.CheckTimer())
				{
					transform.position = intendedPosition;

					kicking = false;
				}
			}
		}
	}
	
	public void KickCamera(Vector3 direction, float radianVariation = -1)
	{
		if (radianVariation < 0)
		{
			radianVariation = angleVariation;
		}

		recoiling = true;
		kicking = true;

		kickTimer.Start();

		float angleAmount = Random.Range(-radianVariation, radianVariation);
		//Debug.Log("Angle Amount\n\t" + angleAmount);
		Vector3 variedDir = Quaternion.AngleAxis(angleAmount, Vector3.forward) * direction;

		kickedPosition = intendedPosition + variedDir.normalized * Random.Range(kickMin, kickMax);
		
	}
}
