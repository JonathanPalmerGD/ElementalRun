using UnityEngine;
using System.Collections;

public class OffsetScroller : MonoBehaviour 
{
	public Spawner planeSpawner;
	public float xScrollSpeed;
	public float yScrollSpeed;
	public Material scrollingMaterial;
	private Vector2 savedOffset;
	private Renderer rend;

	void Start()
	{
		rend = GetComponent<Renderer>();
		savedOffset = rend.sharedMaterial.GetTextureOffset("_MainTex");
	}

	void Update()
	{
		Vector2 newPosition = new Vector2(Mathf.Repeat(Time.time * xScrollSpeed, 1), Mathf.Repeat(Time.time * yScrollSpeed, 1));
		rend.sharedMaterial.SetTextureOffset("_MainTex", newPosition);
	}

	void OnDisable()
	{
		rend.sharedMaterial.SetTextureOffset("_MainTex", savedOffset);
	}
}
