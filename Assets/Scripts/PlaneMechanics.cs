using UnityEngine;
using System.Collections;

public class PlaneMechanics : MonoBehaviour
{
	public float maxPlaneForwardSpeed = 10;
	public float maxPlaneBackwardSpeed = 25;
	public int jumpsAllowed = 2;
	public float jumpForce = 1200;
	public float airControlPercentage = .7f;

	public float gravityScale = 3;
	public float resetForce = 1200;
}
