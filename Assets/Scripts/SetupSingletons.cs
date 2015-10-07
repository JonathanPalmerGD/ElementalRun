using UnityEngine;
using System.Collections;

public class SetupSingletons : MonoBehaviour 
{
	public int SceneDifficulty = 1;

	void Awake()
	{
		AudioManager.Instance.Awake();

		AudioSource track = AudioManager.Instance.MakeSource("yeGods");

		track.loop = true;
		track.Play();

		AudioManager.Instance.AddMusicTrack(track, true);
		//GameManager.Instance.BeginGameMusic();

		Destroy(gameObject);
	}
	
	void Update() 
	{
	
	}
}
