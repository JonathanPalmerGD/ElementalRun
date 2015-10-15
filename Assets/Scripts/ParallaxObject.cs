using UnityEngine;
using System.Collections;

public class ParallaxObject : MonoBehaviour
{
	public enum Element {Fire, Water, Earth, Air};
	public Element affiliation;
	public SpriteRenderer sRend;
	public Sprite ActiveImage;
	public float zBack;
	//private GameObject triggerParticle;

	public void Start ()
	{
		//triggerParticle = Resources.Load<GameObject>("Danger");
		//string spriteFileName = "";
		//if (affiliation == Element.Fire)
		//{
		//	spriteFileName = "RedBox";
		//}
		//if (affiliation == Element.Water)
		//{
		//	spriteFileName = "BlueBox";
		//}
		//if (affiliation == Element.Earth)
		//{
		//	spriteFileName = "BrownBox";
		//}
		//if (affiliation == Element.Air)
		//{
		//	spriteFileName = "GreenBox";
		//}

		//ActiveImage = Resources.Load<Sprite>(spriteFileName);
		
		//sRend.sprite = ActiveImage;
	}
}
