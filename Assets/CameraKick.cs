using UnityEngine;
using System.Collections;

public class CameraKick : MonoBehaviour 
{
	public Vector3 intendedPosition;
	public Vector3 kickedPosition;
	public float kickMin = .6f;
	public float kickMax = 1.2f;
	public float angleVariation = 20;
	private float recoverDur = .08f;
	public bool recoiling = false;
	public bool kicking = false;
	public AdvancedTimer kickTimer;

	void Start() 
	{
		intendedPosition = transform.position;
		kickTimer = ScriptableObject.CreateInstance<AdvancedTimer>();

		kickTimer.Init(recoverDur, -1, false, 0,0, false);
		kickTimer.Stop();
		kickTimer.ContinuousReset = false;
	}
	
	void Update() 
	{
		kickTimer.UpdateTimer(Time.deltaTime);

		if (Input.GetKeyDown(KeyCode.Return))
		{
			KickCamera(Vector3.right, 2);
		}

		if (kicking)
		{
			if (recoiling)
			{
				if (kickTimer.Running)
				{
					float timerPercentage = (kickTimer.counter / kickTimer.timerTargetValue);
					//int kt = (int)(kickTimer.counter * 100);
					//Debug.Log("Recoiling\nTimer running: " + kickTimer.counter);
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
					float timerPercentage = (kickTimer.counter / kickTimer.timerTargetValue);
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

	public void KickCameraVariation(Vector3 direction, float magnitude = 1, float minVariation = 0, float maxVariation = 0, float angleVariation = -1)
	{
		if (angleVariation < 0)
		{
			angleVariation = 0;
		}

		float angleAmount = Random.Range(-angleVariation, angleVariation);

		recoiling = true;
		kicking = true;

		kickTimer.Start();

		Vector3 variedDir = Quaternion.AngleAxis(angleAmount, Vector3.forward) * direction;

		kickedPosition = intendedPosition + variedDir.normalized * magnitude * Random.Range(kickMin, kickMax);
	}

	public void KickCamera(Vector3 direction, float magnitude = 1, float radianVariation = -1)
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
		Vector3 variedDir = Quaternion.AngleAxis(angleAmount, Vector3.forward) * -direction;

		kickedPosition = intendedPosition + variedDir.normalized * Random.Range(kickMin, kickMax);
	}
}
