using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof (PlatformerCharacter2D))]
public class Platformer2DUserControl : MonoBehaviour
{
    private PlatformerCharacter2D m_Character;
	public bool m_Jump;
	public bool m_ForceJump;

    private void Awake()
    {
        m_Character = GetComponent<PlatformerCharacter2D>();
    }

    private void Update()
    {
		if (!m_Jump)
		{
			// Read the jump input in Update so button presses aren'timerPercentage missed.
			m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}
    }

    private void FixedUpdate()
    {
        // Read the inputs.
		bool crouch = false;// Input.GetKey(KeyCode.LeftControl);
		float h = CrossPlatformInputManager.GetAxis("Horizontal");

		//if (Input.GetAxis("Vertical") < -.75f)
		//{
		//	//the layer moving platforms cannot collide with
		//	gameObject.layer = 9;
		//}
		//else
		//{
		//	gameObject.layer = 0; //default layer
		//}

        // Pass all parameters to the character control script.
        m_Character.Move(h, crouch, m_Jump, m_ForceJump);
        m_Jump = false;
		m_ForceJump = false;
    }
}