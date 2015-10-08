using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ForceBlink : MonoBehaviour
{
	public Image blinkScreen;
	public Color blinkColor;
	private float alpha = 1;

	void Start()
	{
		blinkScreen.gameObject.SetActive(false);
	}

	public void ExecuteBlink(float duration = .1f)
	{
		StartCoroutine("MicroBlink", duration);
	}

	public IEnumerator MicroBlink(float time)
	{
		blinkScreen.gameObject.SetActive(true);
		blinkScreen.color = new Color(blinkColor.r, blinkColor.g, blinkColor.b, alpha);

		yield return new WaitForSeconds(time);
		blinkScreen.gameObject.SetActive(false);
	}
}