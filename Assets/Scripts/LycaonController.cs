using System;
using System.IO;
using UnityEngine.UI;
using System.Collections;
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
		_dashCoolDown = dashCoolDown;
    }

	private void Update() {
		if(Time.timeScale == 0) return;
		_dashCoolDown -= Time.deltaTime;
		JumpAndGravity();
		Move();
		if(health <= 0 && !gm.lycaonDead)
			Die();

		if(transform.position.y < -10)
			Die();

		if(_dashCoolDown > 0 && name == "LycaonBody")
			dashCoolDownBar.value = 1 - _dashCoolDown / dashCoolDown;
		else if(name == "LycaonBody")
			dashCoolDownBar.value = 1;
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
		} else if(hit.gameObject.CompareTag("DuplicatorPowerup")) {
			Debug.Log("Lycaon collided with a potion");
			hit.gameObject.GetComponent<DuplicatorPowerup>().ProcessCollision(gameObject);
		} else if(hit.gameObject.CompareTag("SpeedPowerup")) {
			hit.gameObject.GetComponent<SpeedPowerup>().ProcessCollision(gameObject);
		}
	}

	private GameManager gm;

	private void Die() {
		LycaonBodyAnimator.SetBool("Death_b", true);
		LycaonBodyAnimator.SetInteger("DeathType_int", 2);
		moveSpeed = 0;
		rotateSpeed = 0;
		jumpHeight = 0;
		if(gameObject.name == "LycaonBody") {
			gm.lycaonDead = true;
			StartCoroutine(DeathCoroutine());
		} else {
			StartCoroutine(CleanClone());
		}
	}

	IEnumerator CleanClone() {
		yield return new WaitForSeconds(5);
		Destroy(gameObject);
	}

	IEnumerator DeathCoroutine() {
        yield return new WaitForSeconds(2);	// adds delay of two seconds before end screen.	
		Time.timeScale = 0;	
		Scene s = SceneManager.GetActiveScene();	// TODO : save data here
		SceneManager.LoadScene(s.name);
	}

	private void JumpAndGravity() {
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

	public float dashCoolDown = 1.0f;
	private float _dashCoolDown;
	public float dashBreadth = 10.0f;
	public Slider dashCoolDownBar;

    [Obsolete]
    private void Move() {
		float vertInput = Input.GetAxis("LycaonMove");
		float horiInput = Input.GetAxis("LycaonRotate");

		if(gameObject.name == "LycaonBody")	// clones get rotated by Powerup
			transform.Rotate(Vector3.up, -horiInput * rotateSpeed * Time.deltaTime);
		
		_controller.Move(
			transform.forward * vertInput * ( (moveSpeed) * Time.deltaTime)
		);

		if(_controller.velocity.magnitude > 0) {
			LycaonBodyAnimator.SetBool("Static_b", false);
			LycaonBodyAnimator.SetFloat("Speed_f", 1.0f);
		} else {
			LycaonBodyAnimator.SetBool("Static_b", true);
			LycaonBodyAnimator.SetFloat("Speed_f", 0f);
		}

		if(Input.GetKeyDown(KeyCode.LeftControl) && _dashCoolDown < 0) {	// Dash mechanic
			ParticleSystem start = Instantiate(dashParticleGameObject, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			start.startColor = Color.green;
			StartCoroutine(DestroyParticles(start.gameObject));
			start.Play();

			_controller.Move(transform.forward * dashBreadth);
			ParticleSystem end = Instantiate(dashParticleGameObject, transform.position, transform.rotation).GetComponent<ParticleSystem>();
			end.startColor = Color.black;
			end.Play();
			
			StartCoroutine(DestroyParticles(end.gameObject));
			_dashCoolDown = dashCoolDown;
		}
	}

	IEnumerator DestroyParticles(GameObject game) {
		yield return new WaitForSeconds(3.0f);
		Destroy(game);
	}

	public GameObject dashParticleGameObject;

	public float health = 100.0f;
	public Slider healthBar;
	public void TakeDamage(float lightningDistance, float lightningDamageMaxDist) {	// called from ZeusController on Lightning strike.
		health -= (float) 5 * (lightningDamageMaxDist/lightningDistance);
		if(health < 0 && name == "LycaonBody")
			healthBar.value = 0;
		else if(name == "LycaonBody")
			healthBar.value = health / 100.0f;	// fucking horrendous bug
	}
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

	void LateUpdate() {
		_controller.Move(Vector3.back * Time.deltaTime * gm.gameSpeed);
	}

}
