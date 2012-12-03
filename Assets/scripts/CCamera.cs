using UnityEngine;
using System.Collections;

public class CCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
		this.camera.orthographic = true;
		this.camera.orthographicSize = Screen.height/2;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
