using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{
	public int SceneDifficulty = 1;

	void Awake()
	{
		AudioManager.Instance.Awake();

		//GameManager.Instance.BeginGameMusic();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
