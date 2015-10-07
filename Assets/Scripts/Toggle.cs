using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour
{
	public enum Element { Fire, Water, Earth, Air };
	public Element affiliation;
	public Renderer rend;
	private GameObject toggledVisual;

	public static bool[] obstacleEnabled = { true, true, true, true };

	void Start()
	{
		toggledVisual = Resources.Load<GameObject>("Toggled");
		if (affiliation == Element.Fire)
		{
			rend.material.color = new Color(8f, .25f, .0f);
		}
		if (affiliation == Element.Water)
		{
			rend.material.color = new Color(.2f, .2f, .8f);
		}
		if (affiliation == Element.Earth)
		{
			rend.material.color = new Color(.6f, .6f, .2f);
		}
		if (affiliation == Element.Air)
		{
			rend.material.color = new Color(.2f, .7f, .5f);
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (enabled && other.tag == "Player")
		{
			GameObject go = GameObject.Instantiate(toggledVisual, transform.position, Quaternion.identity) as GameObject;

			go.transform.SetParent(transform);
			Destroy(go, 3.0f);

			if (!obstacleEnabled[(int)affiliation])
			{
				AudioManager.Instance.MakeSource("Reload").Play();
			}
			else
			{
				AudioManager.Instance.MakeSource("Bang").Play();
			}

			Runner.Inst.KickPlayerCamera(Vector3.up, .7f);
			//Runner.Inst.Kicks[Runner.Inst.WorldIndex].KickCamera(Vector3.up * .1f);

			obstacleEnabled[(int)affiliation] = !obstacleEnabled[(int)affiliation];
			enabled = false;
			rend.enabled = false;
		}
	}
}
