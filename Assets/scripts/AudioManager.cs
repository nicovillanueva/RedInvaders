using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	public enum Sounds{
		fire,
		bgm,
		explosion,
	}
	
	private GameObject emitter;
	private AudioSource sourceOld;

	
	public AudioClip fire;
	public AudioClip backgroundMusic;
	public float bgmVolume;
	public AudioClip explosion1;
	public AudioClip explosion2;
	
	public int sourcesLimit;
	public float defaultVolume = 1.0f;
	public bool linearRolloff = true;
	
	private GameObject[] emitters;
	private AudioSource[] sources;
	
	// ----------
	
	public void PlaySound(Sounds sound, Vector3 emitterPos){
		emitter.transform.position = emitterPos;
		
		switch(sound){
			case Sounds.fire:
				sourceOld.clip = fire;
				break;
			
			case Sounds.bgm:
				sourceOld.clip = backgroundMusic;
				break;
			
			case Sounds.explosion:
				int r = Random.Range(0,10);
				if(r < 5){
					sourceOld.clip = explosion1;
				}
				else{
					sourceOld.clip = explosion2;
				}
				break;
		}
		
		sourceOld.Play();
	}

	public void PlaySound(Sounds sound){
		PlaySound(sound, Camera.main.transform.position);
	}
	
	// ----------
	
	public void PlayBackground(){
		Camera.main.gameObject.AddComponent<AudioSource>();
		
		Camera.main.gameObject.GetComponent<AudioSource>().clip = backgroundMusic;
		Camera.main.gameObject.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		Camera.main.gameObject.GetComponent<AudioSource>().volume = bgmVolume;
		Camera.main.gameObject.GetComponent<AudioSource>().Play();
	}
	
	// ------------------------------
	
	/// <summary>
	/// Creates the Audio emitters their sources.
	/// </summary>
	/// <param name='srcAmount'>
	/// Amount of audio emitters/sources to be created (limit). Simultaneous sounds possible to be played at the same time.
	/// </param>
	
	private void CreateObjects(int srcAmount){
		emitters = new GameObject[srcAmount];
		
		for(int i = 0; i < emitters.Length; i++){
			emitters[i] = new GameObject("Emitter " + i);
			emitters[i].transform.parent = this.transform;
			
			emitters[i].AddComponent<AudioSource>();
			if(linearRolloff){
				emitters[i].GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
			}
			emitters[i].GetComponent<AudioSource>().volume = defaultVolume;
		}
	}
	
	
	void Awake () {
		if(sourcesLimit <= 0){
			sourcesLimit = 1;
		}
		
		CreateObjects(sourcesLimit);
	}
}
