using UnityEngine;
using System.Collections;

public class CGameManager : MonoBehaviour {
	
	private CWeapon weap = null;
	private CShip ship = null;
	private CSpriteManager spr = null;
	private CEnemyManager enemyMgr = null;
	
	static private int remainingEnemies;
	static private int score;
	
	static public bool paused;
	
	public int defaultLives;
	
	private float tempT;
	public float acceleratingRate; // Every X seconds, enemies move and shoot faster.
	public float acceleratingFactor; // How faster they shoot/move, every X seconds.
	public float delayLimit; // Can't shoot/move faster than this.
	public float initFireDelay;
	
	// ------------------------------
	
	void Awake(){
		weap = MonoBehaviour.FindObjectOfType(typeof(CWeapon)) as CWeapon;
		ship = MonoBehaviour.FindObjectOfType(typeof(CShip)) as CShip;
		spr = MonoBehaviour.FindObjectOfType(typeof(CSpriteManager)) as CSpriteManager;
		enemyMgr = MonoBehaviour.FindObjectOfType(typeof(CEnemyManager)) as CEnemyManager;
		
		CGameManager.paused = false;
	}
	
	// ----------
	
	void Start () {
		ResetGame();
		
		weap.SetFireDelay(initFireDelay); // Arbitrary delay.
		weap.transform.position = ship.transform.position; // Align weapon with ship.
		spr.SetUserSprite(ref ship); // set user's sprite
		
		// background music
		this.audio.Play();
	}
	
	// ----------
	
	void Update () {
		// Pause control.
		if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)){
			PauseSwitch();
		}
		
		if(Input.GetKeyDown(KeyCode.R)){
			enemyMgr.ResetEnemies(true);
			ResetGame();
		}
		
		if(remainingEnemies <= 0){
			enemyMgr.ResetEnemies(false);
			weap.SetFireDelay(weap.GetFireDelay()/2);
		}
		
		// Time the accelerating of enemies.
		tempT += Time.deltaTime;
		if(tempT > acceleratingRate && enemyMgr.fireDelay > delayLimit){
			enemyMgr.fireDelay -= acceleratingFactor;
			enemyMgr.movementDelay -= acceleratingFactor;
			tempT = 0;
		}
	}
	
	// ------------------------------
	
	static public void PauseSwitch(){
		if(paused){
			Time.timeScale = 1.0f; // Go on...
			paused = false;
		}
		else{
			Time.timeScale = 0.0f; // Freeze!
			paused = true;
		}
	}
	
	// ----------
	
	public void ResetGame(){
		ship.ResetPosition();
		CShip.SetLives(defaultLives);
		score = 0;
	}
	
	// ----------
	
	// Reset the ship and enemies (keep score and -1 life)
	public void Respawn(int l){
		if(l > 0){ // Only respawn if there are lives left.
			ship.ResetPosition();
			enemyMgr.ResetEnemies(true);
			CShip.SetLives(CShip.GetLives()-1);
			weap.SetFireDelay(weap.GetFireDelay() + 0.3f);
		}
		else{
			Application.LoadLevel(2);
		}
	}
	
	// ----------
	
	static public int GetScore(){
		return CGameManager.score;
	}
	
	// ----------
	
	static public void ModifyScore(int newscore){
		CGameManager.score += newscore;
	}
	
	// ----------
	
	static public void UpdateRemainingEnemies(int enemyNumber){
		CGameManager.remainingEnemies = enemyNumber;
	}
	
	// ----------
	
	static public int GetRemainingEnemies(){
		return CGameManager.remainingEnemies;
	}
	
	// ----------
	
}
