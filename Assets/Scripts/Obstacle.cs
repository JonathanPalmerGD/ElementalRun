using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
	public enum Element {Fire, Water, Earth, Air};
	public Element affiliation;
	public static float[] damageAmt = {3.0f, 12.0f, 8.0f, 6.0f};
	public Renderer rend;

	void Start ()
	{
		if (affiliation == Element.Fire)
		{
			rend.material.color = new Color(8f, .45f, .0f);
		}
		if (affiliation == Element.Water)
		{
			rend.material.color = new Color(0, .2f, .6f);
		}
		if (affiliation == Element.Earth)
		{
			rend.material.color = new Color(.3f, .4f, .1f);
		}
		if (affiliation == Element.Air)
		{
			rend.material.color = new Color(0, .5f, .5f);
		}
	}

	void Update()
	{
		//if (Input.GetKeyDown(KeyCode.C))
		//{	
		//	Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
		//	Runner.Inst.ChangeStatusEffect((int)affiliation + 1);	
		//}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player")
		{
			Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
			Runner.Inst.ChangeStatusEffect((int)affiliation + 1);
		}
	}
}
