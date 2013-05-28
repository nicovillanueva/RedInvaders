using UnityEngine;
using System.Collections;

public class CShip : MonoBehaviour, IActor {
	/*
	 * Ship:
	 * User-controlled ship.
	 * It monitors keypresses, and moves by itself.
	 * It has a Weapon as a child, which is the one that actually fires.
	 * By itself, it just moves, and has a set of lives.
	 */
	
	public float moveSpeed;
	public float horizMovement;
	public Vector2 pos;
	
	private Vector3 initPos;
	static private int lives;
	private float leftLimit;
	private float rightLimit;
	
//	private CGameManager gamemgr;
	
	// ------------------------------
	
	void Awake(){
		initPos.x = 0.0f;
		initPos.y = -250.0f;
		initPos.z = 20.0f;
		//
		lives = 3;
		//
		leftLimit = (Screen.width/2 - this.transform.localScale.x/2) * -1;
		rightLimit = Screen.width/2 - this.transform.localScale.x/2;
	}
	
	// ----------
	
	void Start () {
		this.transform.position = initPos;
//		gamemgr = MonoBehaviour.FindObjectOfType(typeof(CGameManager)) as CGameManager;
	}
	
	// ----------
	
	void Update () {
		horizMovement = Input.GetAxisRaw("Horizontal");
		
		if(horizMovement != 0.0f){
			Move(horizMovement);
		}
	}

	// ----------
	
	void OnCollisionEnter(Collision coll){
//		Debug.Log(this.name + " hit obj with tag: " + coll.gameObject.tag);
		if(coll.gameObject.tag == "enemy"){
//			DestroyMe();
			this.audio.Play();
//			Managers.Audio.PlaySound(AudioManager.Sounds.explosion, this.transform.position);
			Managers.Game.Respawn(lives);
		}
	}
	
	// ------------------------------
	
	// Not implemented. The Weapon does all the actual shooting.
	public void Shoot(GameObject ammo){}
	
	// ----------
	
	static public int GetLives(){
		return CShip.lives;
	}
	
	// ----------
	
	static public void SetLives(int l){
		CShip.lives = l;
	}
	
	// ----------
	
	public void Move(float move){
		// Check if the Ship is within the screen limits.
		if(this.transform.position.x >= leftLimit && this.transform.position.x <= rightLimit){
			// Move around.
			pos = this.transform.position;
			pos.x += moveSpeed * horizMovement * Time.deltaTime;
			this.transform.position = pos;
		}
		else{
			// Going out of the screen.
			Debug.Log("Off limits. Pos: " + this.transform.position.x);
			if(this.transform.position.x <= leftLimit){
				// Shove the Ship back into the screen limits
				pos = this.transform.position;
				pos.x = leftLimit + 1;
				this.transform.position = pos;
			}
			else if(this.transform.position.x >= rightLimit){
				pos = this.transform.position;
				pos.x = rightLimit - 1;
				this.transform.position = pos;
			}
			else{ // Something happened. Just reset it.
				Debug.LogWarning("Something happened. Resetting.");
				ResetPosition();
			}
		}
	}
	
	// ----------
	
	public void DestroyMe(){
		this.gameObject.SetActiveRecursively(false);
	}
	
	// ----------
	
	public void ResetPosition(){
		this.transform.position = initPos;
	}
	
}
