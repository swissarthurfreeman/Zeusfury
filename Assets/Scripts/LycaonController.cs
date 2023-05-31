using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LycaonController : MonoBehaviour
{
	[Header("Player")]
	[Tooltip("Move speed of the character in m/s")]
	public float moveSpeed = 4.0f;
	[Tooltip("Rotation speed of the character camera")]
	public float rotateSpeed = 100;    
	private float verticalVelocity;
    private float groundedTimer;        // to allow jumping when going down ramps
    public float jumpHeight = 8.0f;
    public float gravityValue = 9.81f;
	private CharacterController _controller;
	public GameObject LycaonBody;
	private Animator LycaonBodyAnimator;
    // Start is called before the first frame update
    void Start() {
		_controller = GetComponent<CharacterController>();
		GameObject mainPanel = GameObject.Find("MainPanel");
		LycaonBodyAnimator = LycaonBody.GetComponent<Animator>();
    }

    // Update is called once per frame
	private void Update() {
		JumpAndGravity();
		Move();
	}
	private void JumpAndGravity() {
		bool groundedPlayer = _controller.isGrounded;
        if (groundedPlayer)
            groundedTimer = 0.2f;	// cooldown interval to allow reliable jumping even whem coming down ramps

        if (groundedTimer > 0)
            groundedTimer -= Time.deltaTime;
 
        // slam into the ground
        if (groundedPlayer && verticalVelocity < 0)
            verticalVelocity = 0f;	// hit ground
 
        verticalVelocity -= gravityValue * Time.deltaTime;	// apply gravity always, to let us track down ramps properly
 
        // allow jump as long as the player is on the ground
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (groundedTimer > 0) {	// must have been grounded recently to allow jump
                groundedTimer = 0;		// no more until we recontact ground
				LycaonBodyAnimator.SetTrigger("Jump_trig");
                verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravityValue);
            }
        }
        _controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);	// call .Move() once only
	}

	private bool idle = true;
    private void Move() {
		float vertInput = Input.GetAxis("LycaonMove");
		float horiInput = Input.GetAxis("LycaonRotate");

		transform.Rotate(Vector3.up, -horiInput * rotateSpeed * Time.deltaTime);
		
		_controller.Move(
			transform.forward * vertInput * (moveSpeed * Time.deltaTime)
		);

		if(_controller.velocity.magnitude > 0) {
			LycaonBodyAnimator.SetBool("Static_b", false);
			LycaonBodyAnimator.SetFloat("Speed_f", 1.0f);
		} else {
			LycaonBodyAnimator.SetBool("Static_b", true);
			LycaonBodyAnimator.SetFloat("Speed_f", 0f);
		}
	}

	public float HeartRate;
	private int _n_beats = 0;
	private int _n_counts = 0;
	public void Print(int value) {
		_n_counts += 1;

		if(value > 600)
			_n_beats += 1;

		if(_n_counts == 50) {	// 5 secs measure
			_n_counts = 0;
			Debug.Log("BPM = " + _n_beats * 12);
			_n_beats = 0;
		}

	}
}