using UnityEngine;
using System.Collections;

public class CGUI : MonoBehaviour {

	private Rect livesRect;
	private Rect scoreRect;
	private Rect statsRect;
	
	private float scrWid;
	private float scrHei;
	private float boxHei;
	private float boxWid;
	
	public float widthPercent;
	public float heightPercent;
	
	public float refreshDelay;
	private float tempT;
	
	private int remainingEnemies;
	
	private float timer;
	private string minutes;
	private string seconds;
	
	void Awake(){
		scrWid = Screen.width;
		scrHei = Screen.height;
		//
		boxWid = (widthPercent*scrWid)/100;
		boxHei = (heightPercent*scrWid)/100;
		//
		statsRect = new Rect(0,0,boxWid,boxHei);
		scoreRect = new Rect((scrWid-boxWid),(scrHei-(boxHei/2)),boxWid,boxHei/4);
		livesRect = new Rect((scrWid-boxWid),(scrHei-(boxHei/4)),boxWid,boxHei/4);
	}
	
	void Start(){
		
	}
	
	void Update(){
		// Delay the updating, so we aren't calling the method all the time.
		tempT += Time.deltaTime;
		if(tempT > refreshDelay){
			this.remainingEnemies = Managers.Game.GetRemainingEnemies();
			tempT = 0.0f;
		}
		
		timer += Time.deltaTime;
		minutes = Mathf.Floor(timer/60).ToString("00");
		seconds = (timer % 60).ToString("00");
	}
	
	// ----------
		
	void OnGUI(){
		if(!Managers.Game.paused){
			// GUI while the game is running
			GUI.Box(statsRect, "Time: " + minutes + ":" + seconds + 
							   "\n\nRemaining: " + this.remainingEnemies +
							   "\nShots fired: " + CWeapon.GetShotsFired() + 
							   "\nAccuracy: " + CWeapon.GetAccuracy() + "%" // The accuracy is refreshed on each confirmed hit.
				);
			GUI.Box(scoreRect,"Score: " + Managers.Game.GetScore());
			GUI.Box(livesRect,"Lives: " + CShip.GetLives());
		}
		else if(Managers.Game.paused){
			// GUI while the game is paused.
			if(GUI.Button(new Rect((scrWid/2)-(boxWid/2), (scrHei/2)-(boxHei/2), boxWid, boxHei/3), "Continue")){
				Managers.Game.PauseSwitch();
			}
			if(GUI.Button(new Rect((scrWid/2)-(boxWid/2), (scrHei/2)-(boxHei/6)+5, boxWid, boxHei/3), "Close game")){
				Application.Quit();
			}
		}
	}
}
