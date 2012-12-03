using UnityEngine;
using System.Collections;

public class CWeapon : MonoBehaviour {
	
	/*
	 * Weapon:
	 * It has a Bullet Pool that gives bullets to the weapon.
	 * It as a modifiable fire speed (what it's set is actually the delay between shots.)
	 * It monitors the Fire1 key.
	 */
	
	public CPool bulletPool;
	
	private float timerLoadWpn;
	private bool shooting;
	private float fireSpeed;
	private bool loadedWeapon;
	static private int shots; // Total shots fired. Just for stats.
	static private int goodHits;
	static private float accuracy;
	
	// -----------------------------------------------------
	
	void Start () {
		shooting = false;
		fireSpeed = 0.5f;
		timerLoadWpn = 0;
		
		CWeapon.shots = 0;
		CWeapon.goodHits = 0;
		CWeapon.accuracy = 0.0f;
		
		this.transform.position = this.transform.parent.position;
	}
	
	// -----------------------------------------------------
	
	void Update () {
		
		// Get if Fire is being pressed
		if(Input.GetButtonDown("Fire1")){
			shooting = true;
		}
		if(Input.GetButtonUp("Fire1")){
			shooting = false;
		}
		
		// Check if weapon is ready to fire
		timerLoadWpn += Time.deltaTime;
		if(timerLoadWpn > fireSpeed){
			loadedWeapon = true;
			timerLoadWpn = 0;
		}
		
		// If the gun is loaded and the button is being pressed, FIRE AWAY
		if(shooting == true && loadedWeapon == true && Time.timeScale != 0.0f){
			// Fire
			if(Fire()){ // If it could fire, play sound, count the shot and unload
				 this.audio.Play();
				CWeapon.shots++;
				loadedWeapon = false;
			}
		}
	} // end update
	
	// -----------------------------------------------------
	
	private bool Fire(){
		GameObject obj = null;
		obj = bulletPool.GetObject();
		if(obj != null){ // If the bullet could be created, return true.
			// Set bullet's position and rotation (irrelevant in SpaceInvaders' case)
			obj.transform.position = this.transform.position;
//			obj.transform.rotation = this.transform.rotation;
			return true;
		}
		else return false;
	}
	
	// -----------------------------------------------------
	
	public void SetFireDelay(float spd){
		this.fireSpeed = spd;
	}
	
	// ----------
	
	public float GetFireDelay(){
		return this.fireSpeed;
	}
	
	// ----------
	
	static public int GetShotsFired(){
		return CWeapon.shots;
	}
	
	// ----------
	
	static public int GetHits(){
		return CWeapon.goodHits;
	}
	
	static public float GetAccuracy(){
		return CWeapon.accuracy;
	}
	
	static public void ReportHit(){
		CWeapon.goodHits++;
		CWeapon.accuracy = (CWeapon.goodHits * 100) / CWeapon.shots;
	}
	
}
