using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	public enum Sounds{
		fire,
		bgm,
		explosion,
	}
	
	private GameObject emitter;
	private AudioSource source;
	
	public AudioClip fire;
	public AudioClip backgroundMusic;
	public float bgmVolume;
	public AudioClip explosion1;
	public AudioClip explosion2;
	
	public void PlaySound(Sounds sound, Vector3 emitterPos){
		emitter.transform.position = emitterPos;
		
		switch(sound){
			case Sounds.fire:
				source.clip = fire;
				break;
			
			case Sounds.bgm:
				source.clip = backgroundMusic;
				break;
			
			case Sounds.explosion:
				int r = Random.Range(0,10);
				if(r < 5){
					source.clip = explosion1;
				}
				else{
					source.clip = explosion2;
				}
				break;
		}
		
		source.Play();
	}

	public void PlaySound(Sounds sound){
		PlaySound(sound, Camera.main.transform.position);
	}
	
	public void PlayBackground(){
		Camera.main.gameObject.AddComponent<AudioSource>();
		
		Camera.main.gameObject.GetComponent<AudioSource>().clip = backgroundMusic;
		Camera.main.gameObject.GetComponent<AudioSource>().rolloffMode = AudioRolloffMode.Linear;
		Camera.main.gameObject.GetComponent<AudioSource>().volume = bgmVolume;
		Camera.main.gameObject.GetComponent<AudioSource>().Play();
	}
	
	void Awake () {
		emitter = new GameObject("Audio Emitter");
		source = emitter.AddComponent<AudioSource>();
		source.rolloffMode = AudioRolloffMode.Linear;
	}
}
