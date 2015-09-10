using UnityEngine;
using System.Collections;

public class Toggle : MonoBehaviour
{
	public enum Element { Fire, Water, Earth, Air };
	public Element affiliation;
	public Renderer rend;

	public static bool[] obstacleEnabled = { true, true, true, true };

	void Start()
	{
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
		if (other.tag == "Player")
		{
			obstacleEnabled[(int)affiliation] = !obstacleEnabled[(int)affiliation];
			enabled = false;
			rend.enabled = false;
		}
	}
}
