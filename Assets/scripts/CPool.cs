using UnityEngine;
using System.Collections;

public class CPool : MonoBehaviour {
	/*
	 * Pool:
	 * Create a set of stuff, on awake.
	 * It can give out, and recycle back GameObjects.
	 */
	
	// Stuff set through inspector
	public int amount;
	public GameObject thing;
	
	// My privates!
	private int i; // Wildcard iterator. Because I can.
	private GameObject[] stuff;
	private int available;
	
	// -----------------------------------
	
	void Awake(){
		stuff = new GameObject[amount]; // Create the array.
		available = 0;
		
		for(i = 0; i < stuff.Length; i++){ // Create the stuff inside the array.
			stuff[i] = GameObject.Instantiate(thing) as GameObject;
			stuff[i].transform.parent = this.transform;
			stuff[i].name = thing.name + " " + i;
			stuff[i].SetActiveRecursively(false);
			available++;
		}
	}
	
	// -----------------------------------
	
	public GameObject GetObject(){
		if(available > 0){// If there are available
			GameObject go = stuff[available-1]; // take one out
			go.SetActiveRecursively(true); // activate it
			available--;
			return go; // give it away.
		}
		else{
			Debug.LogWarning("No more objects available!");
			return null;
		}
	}
	
	// -----------------------------------
	
	public void StoreObject(GameObject obj){
		obj.SetActiveRecursively(false); // Deactivate the object
		stuff[available] = obj; // and store it.
		available++;
	}
	
	// -----------------------------------
	
	public int GetAvailableObjects(){
		return available;
	}
	
}
