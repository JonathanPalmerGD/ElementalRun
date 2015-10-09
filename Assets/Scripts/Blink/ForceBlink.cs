using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ForceBlink : MonoBehaviour
{
	public Image blinkScreen;
	public Color blinkColor;
	public float alpha = 1f;

	void Start()
	{
		blinkScreen.enabled = false;
	}

	public void ExecuteBlink(float duration = .1f)
	{
		StartCoroutine("MicroBlink", duration);
	}

	public IEnumerator MicroBlink(float time)
	{
		blinkScreen.enabled = true;
		blinkScreen.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);

		yield return new WaitForSeconds(time);
		blinkScreen.enabled = false;
	}
}