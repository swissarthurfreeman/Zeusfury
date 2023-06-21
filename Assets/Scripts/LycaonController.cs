using System;
using System.IO;
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
	[SerializeField]
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
		gm = GameObject.Find("[GameManager]").GetComponent<GameManager>();
		manager = GameObject.Find("MainMenu").GetComponent<MenuManager>();
    }

    // Update is called once per frame
	private void Update() {
		JumpAndGravity();
		Move();
		Powerups();
		if(health <= 0 && !gm.lycaonDead)
			Die();
	}

	void Powerups() {
		
	}

	void EndAreaReached() {
		gm.lycaonWon = true;
	}

	[SerializeField]
	private bool groundedPlayer = false;

	// overloads OnTriggerEnter method of CharacterController 
	// (which is of type collider)
	void OnControllerColliderHit(ControllerColliderHit hit) {
		if(!groundedPlayer && hit.gameObject.CompareTag("Floor")) {
			groundedPlayer = true;
			LycaonBodyAnimator.SetBool("Jump_b", false);
			LycaonBodyAnimator.SetBool("Grounded", true);
		} else if(hit.gameObject.CompareTag("Zeus")) {	// if Zeus caught up and touched player
			Die();
		} else if(hit.gameObject.CompareTag("Tartarus")) {
			EndAreaReached();
		}
	}

	private GameManager gm;
	private MenuManager manager;

	private void Die() {
		LycaonBodyAnimator.SetBool("Death_b", true);
		LycaonBodyAnimator.SetInteger("DeathType_int", 2);
		gm.lycaonDead = true;
		enabled = false;				// disables update() of this script
		StartCoroutine(DeathCoroutine());
	}

	IEnumerator DeathCoroutine() {
        yield return new WaitForSeconds(2);	// adds delay of two seconds before end screen.
		Time.timeScale = 0;	
		Scene s = SceneManager.GetActiveScene();	// TODO : save data here
		SceneManager.LoadScene(s.name);
	}

	private void JumpAndGravity() {
		//if(groundedPlayer && verticalVelocity < 0)
		//	verticalVelocity = 0.0f;

        // allow jump as long as the player is on the ground
		if (Input.GetKeyDown(KeyCode.Space) && groundedPlayer) {
			LycaonBodyAnimator.SetBool("Jump_b", true);
			verticalVelocity = Mathf.Sqrt(jumpHeight * 2 * gravityValue);
			groundedPlayer = false;
        }	

		if(verticalVelocity < -10 && !groundedPlayer) {	// if falling
			LycaonBodyAnimator.SetBool("Jump_b", false);
			LycaonBodyAnimator.SetBool("Grounded", false);
		}

        _controller.Move(Vector3.up * verticalVelocity * Time.deltaTime);	// call .Move() once only
		if(verticalVelocity > -10)
			verticalVelocity -= gravityValue * Time.deltaTime;	// apply gravity always, to let us track down ramps properly
	}

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

	public float health = 100.0f;
	public void TakeDamage(float lightningDistance, float lightningDamageMaxDist) {	// called from ZeusController on Lightning strike.
		health -= (float) Math.Pow(lightningDamageMaxDist/lightningDistance, 4.0f);
		Debug.Log("Lycaon Health = " + health.ToString());
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

	public void SetSpeed(){
		string filePath = Path.Combine(Application.dataPath, "value.txt");
		if (File.Exists(filePath))
		{
			try
			{
				using (StreamReader reader = new StreamReader(filePath))
				{
					string firstLine = reader.ReadLine();
					if (float.TryParse(firstLine, out float value))
					{
						Debug.Log(value);
						moveSpeed = 4.0f * value;
					}
					else
					{
						Debug.Log("La première ligne ne contient pas un numéro valide.");
					}
				}
			}
			catch (System.Exception ex)
			{
				Debug.Log("collision: read again");
			}
		}
		else
		{
			Debug.Log("Le fichier n'existe pas.");
		}
	}
}
