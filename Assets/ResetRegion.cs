using UnityEngine;
using System.Collections;

public class ResetRegion : MonoBehaviour
{

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other is CircleCollider2D)
		{
			Debug.Log("Fell\n");
		}
	}
}
