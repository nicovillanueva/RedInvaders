using UnityEngine;
using System.Collections;

public class CSpriteManager : MonoBehaviour {
	
	public Material mat_blue;
	public Material mat_red;
	public Material mat_yellow;
	public Material mat_green;
	public Material userShip;
	
//	static public Material expl_blue;
//	static public Material expl_red;
//	static public Material expl_yellow;
//	static public Material expl_green;
	
	public Material expl_blue;
	public Material expl_red;
	public Material expl_yellow;
	public Material expl_green;

	// Set the user's sprite. Takes no "types".
	public void SetUserSprite(ref CShip go){
		go.renderer.material = userShip;
	}
	
	// Gets a 'type', from which defines what offset and size to use.
	public void SetEnemySprite(ref GameObject go, byte type){
		switch(type){
		case 1:
			go.renderer.material = mat_blue;
			break;
		case 2:
			go.renderer.material = mat_red;
			break;
		case 3:
			go.renderer.material = mat_green;
			break;
		case 4:
			go.renderer.material = mat_yellow;
			break;
		}
	}
	
	// Flips the scale/inverts the tiling, to go to the other part of the "sprite sheet"
	public void SwitchSpriteState(ref GameObject go){
		Vector2 temp = go.renderer.material.mainTextureScale;
		temp.x *= -1;
		go.renderer.material.mainTextureScale = temp;
	}
	
	// Receive which type the enemy is, and return the appropiate explosion color.
	public Material SwitchToExplosion(int type){
		
		switch(type){
		case 1:
			return this.expl_blue;
		case 2:
			return this.expl_red;
		case 3:
			return this.expl_green;
		case 4:
			return this.expl_yellow;
		}
		return null;
	}
	
}
