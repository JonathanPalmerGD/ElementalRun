using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
	public enum Element {Fire, Water, Earth, Air};
	public Element affiliation;
	public static float[] damageAmt = {3.0f, 12.0f, 8.0f, 6.0f};
	public SpriteRenderer sRend;
	public Sprite DisabledImage;
	public Sprite ActiveImage;
	private GameObject triggerParticle;
	public Collider2D col;
	private Color normal;
	private Color disabled;

	public	void Start ()
	{
		triggerParticle = Resources.Load<GameObject>("Danger");
		string spriteFileName = "";
		if (affiliation == Element.Fire)
		{
			spriteFileName = "RedBox";
			//sRend.material.color = new Color(8f, .25f, .0f);
		}
		if (affiliation == Element.Water)
		{
			spriteFileName = "BlueBox";
			//sRend.material.color = new Color(.3f, .3f, .8f);
		}
		if (affiliation == Element.Earth)
		{
			spriteFileName = "BrownBox";
			//sRend.material.color = new Color(.6f, .6f, .2f);
		}
		if (affiliation == Element.Air)
		{
			spriteFileName = "GreenBox";
			//sRend.material.color = new Color(.2f, .7f, .5f);
		}

		DisabledImage = Resources.Load<Sprite>(spriteFileName + "Disabled");
		ActiveImage = Resources.Load<Sprite>(spriteFileName);
		
		sRend.sprite = ActiveImage;

		//normal = sRend.material.color;
		//disabled = new Color(normal.r, normal.g, normal.b, .3f);
	}

	void Update()
	{
		if (!Toggle.obstacleEnabled[(int)affiliation])
		{
			col.enabled = false;
			//sRend.material.color = disabled;
			sRend.sprite = DisabledImage;
		}
		else
		{
			col.enabled = true;
			//sRend.material.color = normal;
			sRend.sprite = ActiveImage;

		}
		//if (Input.GetKeyDown(KeyCode.C))
		//{	
		//	Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
		//	Runner.Inst.ChangeStatusEffect((int)affiliation + 1);	
		//}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (enabled && other.tag == "Player")
		{
			GameObject go = GameObject.Instantiate(triggerParticle, transform.position, Quaternion.identity) as GameObject;

			go.transform.SetParent(transform);
			Destroy(go, 3.0f);

			Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
			Runner.Inst.ChangeStatusEffect((int)affiliation + 1);

			enabled = false;
			sRend.enabled = false;
			//Destroy self?
		}
	}
}
