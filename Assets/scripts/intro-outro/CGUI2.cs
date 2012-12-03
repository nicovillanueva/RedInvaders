using UnityEngine;
using System.Collections;

public class CGUI2 : MonoBehaviour {
	
	private Rect bg;
	private Rect logo;
	private Rect start;
	private Rect quit;
	
	public Texture logoTex;
	
	public float bgWid;
	public float bgHei;
	public float btnWid;
	public float btnHei;
	
	private float scrWid;
	private float scrHei;
	
	void Awake(){
		
		scrWid = Screen.width;
		scrHei = Screen.height;
		
		btnHei = bgHei/3;
		bg =    new Rect((scrWid/2-bgWid/2),(scrHei/2-bgHei/2),bgWid,bgHei);
		logo =  new Rect((scrWid/2-bgWid/2),(scrHei/2-bgHei/2),btnWid,btnHei);
		start = new Rect((scrWid/2-bgWid/2),(scrHei/2-bgHei/2)+btnHei+20,btnWid,btnHei-20);
		quit =  new Rect((scrWid/2-bgWid/2),(scrHei/2-bgHei/2)+btnHei*2+20,btnWid,btnHei/2);
		
		Debug.Log(logo.height + " " + logo.width);
	}
	
	void Update(){
		this.transform.Rotate(Vector3.up * Time.deltaTime);
	}
	
	void OnGUI(){
		GUI.Box(bg, ""); // Background
		if(Application.loadedLevel == 0){
			GUI.Box(logo, logoTex);
		}
		
		// If this is the intro, show "Start!"
		if(Application.loadedLevel == 0){
			if(GUI.Button(start,"Start!"))
				Application.LoadLevel(1);
		}
		
		// if this is the Retry, show "Retry!"
		else if(Application.loadedLevel == 2){
			if(GUI.Button(start,"Retry!"))
				Application.LoadLevel(1);
		}
		
		if(GUI.Button(quit,"Quit game"))
			Application.Quit();
	}
}
