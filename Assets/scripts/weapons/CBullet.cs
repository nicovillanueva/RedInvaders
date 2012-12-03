using UnityEngine;
using System.Collections;

public class CBullet : MonoBehaviour, IDestroyable {
	/*
	 * Bullet:
	 * They know the BulletPool only to get recycled.
	 * They can be set to move up or down.
	 * They just, well, move.
	 */
	
	private Vector3 dir;
	private Vector3 pos;
	public float speed = 250.0f;
	public bool goesUp;
	
	public CPool bp;
	
	private float tempT;
	public float imgFlipTime = 0.5f;
	
	// ------------------------------
	
	void Awake () 
    {
		
		// Too hardcoded for my taste, but it's what comes to mind:
		if(this.tag == "enemy"){
			bp = GameObject.FindGameObjectWithTag("enemyPool").GetComponent<CPool>();
		}
		else if(this.tag == "user"){
			bp = GameObject.FindGameObjectWithTag("userPool").GetComponent<CPool>();
		}
		
		if(goesUp){
			dir = this.transform.up;
		}
		else{
			dir = this.transform.up * -1;
		}
		dir.z = 0.0f;
		dir.Normalize();
	}
	
	// ----------
	
	void Start(){
	}
	
	// ----------
	
	void Update () {
		
		tempT += Time.deltaTime;
		if(tempT > imgFlipTime){
			tempT = 0;
			
			// Not very powerful, but functional way of flipping the image.
			Vector3 temp;
			temp = this.transform.localScale;
			temp.x *= -1;
			this.transform.localScale = temp;
		}
		
		
        pos = this.transform.position;
		
        pos += speed * dir * Time.deltaTime;

        if (pos.y < -(Screen.height/2))
			DestroyMe();
		
        else if (pos.y > Screen.height)
			DestroyMe();
		
        this.transform.position = pos;
	}
	
	// ----------
	
	void OnCollisionEnter(Collision coll){
		if(coll.gameObject.tag != this.tag){
			if(goesUp){ // If it goes up, means that it was a user's shot
				CWeapon.ReportHit();
			}
			
			DestroyMe();
		}
	}
	
	// ------------------------------
		
	// Recycle this bullet back into the pool.
	public void DestroyMe(){
		bp.StoreObject(this.gameObject);
	}
	
}
