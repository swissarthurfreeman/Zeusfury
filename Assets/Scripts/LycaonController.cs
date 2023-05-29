using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LycaonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 4.0f;

	[Tooltip("Rotation speed of the character camera")]
	public float rotateSpeed = 100;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Tooltip("The character uses its own gravity value.")]
	public float Gravity = -15.0f;
    
	private CharacterController _controller;

	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;
	private Hybrid8Test _bitalino;
    // Start is called before the first frame update
    void Start() {
		_controller = GetComponent<CharacterController>();
		GameObject mainPanel = GameObject.Find("MainPanel");
		//if(mainPanel != null) {
		//	_bitalino = mainPanel.GetComponent<Hybrid8Test>();
		//}
    }

    // Update is called once per frame
	private void Update() {
		JumpAndGravity();
		Move();
	}

	private void JumpAndGravity() {
		if (Input.GetKeyDown(KeyCode.Space) && _controller.isGrounded)  // Jump
			_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);   // the square root of H * -2 * G = how much velocity needed to reach desired height

		if (Math.Abs(_verticalVelocity) < Math.Abs(_terminalVelocity))
			_verticalVelocity += Gravity * Time.deltaTime;
	}

    private void Move() {
		float vertInput = Input.GetAxis("LycaonMove");
		float horiInput = Input.GetAxis("LycaonRotate");

		transform.Rotate(Vector3.up, -horiInput * rotateSpeed * Time.deltaTime);
		
		_controller.Move(
			transform.forward * vertInput * (MoveSpeed * Time.deltaTime) + 
			new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime
		);
	}
}