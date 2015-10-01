using UnityEngine;
using System.Collections;

public class ResetRegion : MonoBehaviour
{

	void Update()
	{
		if (Runner.Inst.transform.position.y < transform.position.y)
		{
			Runner.Inst.FellBelow();
		}
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other is BoxCollider2D)
		{
			Runner.Inst.FellBelow();
			//Debug.Log("Fell\n");
		}
	}
}
