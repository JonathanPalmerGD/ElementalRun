using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
	public enum Element {Fire, Water, Earth, Air};
	public Element affiliation;
	public static float[] damageAmt = {3.0f, 12.0f, 8.0f, 6.0f};
	public Renderer rend;
	public Collider2D col;
	private Color normal;
	private Color disabled;

	public 

	void Start ()
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

		normal = rend.material.color;
		disabled = new Color(normal.r, normal.g, normal.b, .3f);
	}

	void Update()
	{
		if (!Toggle.obstacleEnabled[(int)affiliation])
		{
			col.enabled = false;
			rend.material.color = disabled;
		}
		else
		{
			col.enabled = true;
			rend.material.color = normal;
		}
		//if (Input.GetKeyDown(KeyCode.C))
		//{	
		//	Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
		//	Runner.Inst.ChangeStatusEffect((int)affiliation + 1);	
		//}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Player")
		{
			Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
			Runner.Inst.ChangeStatusEffect((int)affiliation + 1);

			//Destroy self?
		}
	}
}
