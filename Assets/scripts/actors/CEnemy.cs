using UnityEngine;
using System.Collections;

public class CEnemy : MonoBehaviour {
	
	public int enemyType;
	public float dissapearDelay;
	private float tempT;
	private Vector3 tempMove;
	private int matrX;
	private int matrY;
	private bool marked; // being deleted
	
	
	// -----------------------------------
	
	void Awake(){
		
	}
	
	// ----------
	
	void Start () {
		marked = false;
	}
	
	// ----------
	
	void Update () {
	
	}
	
	// ----------
	
	void OnCollisionEnter(Collision coll){
		if(coll.gameObject.tag == "user"){
			this.transform.parent.GetComponent<CEnemyManager>().SequentialDestroy(this.matrX, this.matrY);
		}
	}
	
	// -----------------------------------
	
	public void SetEnemyType(int t){
		this.enemyType = t;
	}
	
	// ----------
	
	public void Move(float move){
		tempMove = this.transform.position;
		tempMove.x += move;
		this.transform.position = tempMove;
	}
	
	// ----------
	
	public void Shoot(GameObject ammo){
		ammo.transform.position = this.transform.position;
		ammo.SetActiveRecursively(true);
	}
	
	// ----------
	
	public void DestroyMe(){
		this.marked = true;
		
		CSpriteManager cs = MonoBehaviour.FindObjectOfType(typeof(CSpriteManager)) as CSpriteManager;
		this.renderer.material = cs.SwitchToExplosion(this.enemyType);
		Invoke("Disappear", dissapearDelay);
	}
	
	private void Disappear(){
		this.gameObject.SetActiveRecursively(false);
	}
	
	// ----------
	
	public void SetLogicCoordinates(int x, int y){
		this.matrX = x;
		this.matrY = y;
	}
	
	public int GetX(){
		return matrX;
	}
	
	public int GetY(){
		return matrY;
	}
	
	// ----------
	
	public bool GetMarked(){
		return marked;
	}
}
