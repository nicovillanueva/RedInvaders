using UnityEngine;
//using System.Collections;

	[RequireComponent(typeof(CGameManager))]
	[RequireComponent(typeof(CEnemyManager))]
	[RequireComponent(typeof(CSpriteManager))]
//	[RequireComponent(typeof(AudioManager))]

public class Managers : MonoBehaviour {
	
	private static CGameManager gameMgr;
	public static CGameManager Game{
		get { return gameMgr; }
	}
	
	private static CEnemyManager enemyMgr;
	public static CEnemyManager EnemyMgr{
		get { return enemyMgr; }
	}
	
	private static CSpriteManager spriteMgr;
	public static CSpriteManager SpriteMgr{
		get { return spriteMgr; }
	}
	
//	private static AudioManager audioMgr;
//	public static AudioManager Audio{
//		get { return audioMgr; }
//	}
	
	void Awake () {
		gameMgr = GetComponent<CGameManager>();
		enemyMgr = GetComponent<CEnemyManager>();
		spriteMgr = GetComponent<CSpriteManager>();
//		audioMgr = GetComponent<AudioManager>();
		
		DontDestroyOnLoad(gameObject);
	}
	
}
