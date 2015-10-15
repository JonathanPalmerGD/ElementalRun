using UnityEngine;
using System.Collections;

public class Obstacle : MonoBehaviour
{
	public enum Element {Fire, Water, Earth, Air};
	public Element affiliation;
	public static float[] damageAmt = { 3.0f, 12.0f, 8.0f, 6.0f };
	public static float[] greaterDamageAmt = { 10.0f, 15.0f, 12.0f, 10.0f };
	public enum ObstacleType { Normal, ForceSwitch };
	public ObstacleType obType = ObstacleType.Normal;
	public SpriteRenderer sRend;
	public Sprite DisabledImage;
	public Sprite ActiveImage;
	private GameObject triggerParticle;
	public Collider2D col;
	private Color normal;
	private Color disabled;
	private AdvancedTimer resetTimer;

	public void Start ()
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

		resetTimer = ScriptableObject.CreateInstance<AdvancedTimer>();
		resetTimer.Init(.75f);
		resetTimer.ContinuousReset = false;

		DisabledImage = Resources.Load<Sprite>(spriteFileName + "Disabled");
		ActiveImage = Resources.Load<Sprite>(spriteFileName);
		
		sRend.sprite = ActiveImage;

		//normal = sRend.material.color;
		//disabled = new Color(normal.r, normal.g, normal.b, .3f);
	}

	void Update()
	{
		resetTimer.UpdateTimer(Time.deltaTime);

		resetTimer.CheckTimer();

		if (Toggle.obstacleEnabled[(int)affiliation] || obType == ObstacleType.ForceSwitch)
		{
			col.enabled = true;
			//sRend.material.color = normal;
			sRend.sprite = ActiveImage;
		}
		else
		{
			col.enabled = false;
			//sRend.material.color = disabled;
			sRend.sprite = DisabledImage;
		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		//If we're enabled, and we hit the player and the reset timer isn't running (which says we've hit the player recently)
		if (enabled && other.tag == "Player" && !resetTimer.Running)
		{
			GameObject go = GameObject.Instantiate(triggerParticle, transform.position, Quaternion.identity) as GameObject;

			go.transform.SetParent(transform);
			Destroy(go, 3.0f);

			

			if (obType == ObstacleType.Normal)
			{
				Runner.Inst.AdjustHealth(-damageAmt[(int)affiliation]);
				Runner.Inst.ChangeStatusEffect((int)affiliation + 1);

				enabled = false;
				sRend.enabled = false;				
			}
			else
			{
				Runner.Inst.AdjustHealth(-greaterDamageAmt[(int)affiliation]);
				Runner.Inst.ChangeStatusEffect((int)affiliation + 1);

				resetTimer.Reset(true);
			}
		}
	}
}
