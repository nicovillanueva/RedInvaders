using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CEnemyManager : MonoBehaviour {
	
	private enum Movements{
		side,
		down
	}
	public enum Directions{
		right,
		left
	}
	
	// Also used as initial movement. Set in inspector.
	public Directions currentDirection;
	
	public byte[,] LogicEnemyMatrix = null;
	public GameObject[,] RealEnemyMatrix = null;
	
	// Enemy to be used (the skin/renderer.material is set later.)
	public GameObject Enemy;
	
	// Set the matrixes (Real and Logic) height/length through inspector.
	public int matrixWidth;
	public int matrixHeight;
	
	// Width used to calculate when to move down. Height is not currently used.
	private float totalWidth;
//	private float totalHeight;
	
	// Timers to control movement and firing.
	public float movementDelay;
	public float fireDelay;
	private float tempTimerMove;
	private float tempTimerFire;
	private float defaultMovDelay;
	private float defaultFirDelay;
	
	// Used by the GameManager
	private int aliveEnemies = 0;
	
	// Ammo to be loaded to the pool for the enemies to fire.
	public GameObject enemysAmmo;
	public CPool enemyBullets;
	
	// To set each enemy's skin
//	private CSpriteManager spr;
	
	// List of enemies who are able to fire (the lowest of each column)
	private List<GameObject> readyToFire;
	
	// Score configurations
	public int row0Score;
	public int row1Score;
	public int row2Score;
	public int row3Score;
	public float exponentialScore; // The higher, the most points give chaining enemy kills.
	
	// IMPLEMENT <<<<<<<<<
	private Vector3 initialPosition;
	
	// -----------------------------------------------------
	
	void Awake(){
		// Create thee matrixes
		LogicEnemyMatrix = new byte[matrixWidth,matrixHeight];
		RealEnemyMatrix = new GameObject[matrixWidth,matrixHeight];
		
		readyToFire = new List<GameObject>();
		
		enemyBullets.thing = enemysAmmo;
		enemyBullets.amount = matrixWidth; // Only the lowermost will fire, so, why create more.
	}
	
	// ----------
	
	void Start () {
		// There can only be ONE SpriteManager on-scene.
//		spr = GameObject.FindObjectOfType(typeof(CSpriteManager)) as CSpriteManager;
		
		InitiateMatrixes();
		
		// Get the total size (in pixels) of the whole Real Matrix, to check boundaries when moving.
//		totalHeight = (Enemy.transform.localScale.y * matrixHeight) + ((Enemy.transform.localScale.y / 2) * (matrixHeight-1));
		totalWidth = ((Enemy.transform.localScale.x * matrixWidth) + (Enemy.transform.localScale.x * (matrixWidth-1)));
		
		// Set their types (according to LogicMatrix) and their positions.
		SetTypesAndCoords();
		SetPositions();
		
		// Store default delays for resetting.
		defaultFirDelay = fireDelay;
		defaultMovDelay = movementDelay;
	}
	
	// ----------
	
	void Update () {
		if(Input.GetKeyUp(KeyCode.D)){
			MoveDown();
		}
		if(Input.GetKeyUp(KeyCode.E)){
			GameObject bullet = enemyBullets.GetObject();
			if(bullet != null){
				int rand = Random.Range(0,matrixWidth-1);
				RealEnemyMatrix[rand,0].GetComponent<CEnemy>().Shoot(bullet);
			}
		}
		
		// If the game is not paused, time everything.
		if(Managers.Game.paused == false){
			// Timed moving
			tempTimerMove += Time.deltaTime;
			if(tempTimerMove > movementDelay){
				tempTimerMove = 0.0f;
				MoveAll();
			}
			
			// Timed enemy firing
			tempTimerFire += Time.deltaTime;
			if(tempTimerFire > fireDelay){
				GameObject bullet = enemyBullets.GetObject();
				if(bullet != null){
					int r = Random.Range(0,(readyToFire.Count-1));
					readyToFire[r].GetComponent<CEnemy>().Shoot(bullet);
					tempTimerFire = 0.0f;
				}
			}
		}
	}
	
	// -----------------------------------------------------
	
	public void ResetEnemies(bool resetDelays){
		// Destroy all enemies.
		foreach(GameObject go in RealEnemyMatrix){
			Destroy(go);
		}
		aliveEnemies = 0;
		
		// Reset Delays.
		if(resetDelays){
			fireDelay = defaultFirDelay;
			movementDelay = defaultMovDelay;
		}
		// Re-create all matrixes, RTF list and stuff.
		LogicEnemyMatrix = new byte[matrixWidth,matrixHeight];
		RealEnemyMatrix = new GameObject[matrixWidth,matrixHeight];
		readyToFire = new List<GameObject>();
		
		InitiateMatrixes();
		SetTypesAndCoords();
		SetPositions();
	}
	
	// ----------
	
	private void InitiateMatrixes(){
		// Randomly fill up logic matrix with numbers between 1 and 4 (inclusive x 2)
		LogicEnemyMatrix = RandomFillMatrix(LogicEnemyMatrix, 1, 4);
		
		// Instantiate enemies in the Real matrix, set their names and activate them.
		for(int i = 0; i < matrixWidth; i++){
			for(int e = 0; e < matrixHeight; e++){
				if(RealEnemyMatrix[i,e] == null){ // If they weren't enabled, instantiate, name and parent them. Otherwise, just enable them.
					RealEnemyMatrix[i,e] = Instantiate(Enemy) as GameObject;
					RealEnemyMatrix[i,e].name = Enemy.name + " " + i + " " + e;
					RealEnemyMatrix[i,e].transform.parent = this.transform; // Add them as the manager's children, only to have them organized.
					if(e == 0){
						readyToFire.Add(RealEnemyMatrix[i,0]); // Set who will firstly be able to shoot (bottom row).
					}
				}
				RealEnemyMatrix[i,e].SetActiveRecursively(true); // Default state (activated)
				
				aliveEnemies++;
			}
		}
		Managers.Game.UpdateRemainingEnemies(aliveEnemies);
	}
	
	// ----------
	
	// Fill a logic matrix with ints between a min and a max.
	private byte[,] RandomFillMatrix(byte[,] m, int min, int max){
		int rand;
		for(int i = 0; i < matrixWidth; i++){
			for(int e = 0; e < matrixHeight; e++){
				//Debug.Log("Setting: [" + i + "," + e + "]");
				rand = Random.Range(min,max+1);
				m[i,e] = (byte)rand;
			}
		}
		
		return m;
	}
	
	// ----------
	
	// Set the enemies' types and matrix coordinates according to the Logic matrix integers.	
	public void SetTypesAndCoords(){
		for(int i = 0; i < matrixWidth; i++){
			for(int e = 0; e < matrixHeight; e++){
				RealEnemyMatrix[i,e].GetComponent<CEnemy>().SetEnemyType(LogicEnemyMatrix[i,e]); // Set enemy's internal (enemy)type variable
//				spr.SetEnemySprite(ref RealEnemyMatrix[i,e],LogicEnemyMatrix[i,e]); // Set sprite according to type
				Managers.SpriteMgr.SetEnemySprite(ref RealEnemyMatrix[i,e],LogicEnemyMatrix[i,e]);
				RealEnemyMatrix[i,e].GetComponent<CEnemy>().SetLogicCoordinates(i,e); // Match internal enemy coordinates, with the Logical ones
			}
		}
	}
	
	// ----------
	
	// Set positions according to the enemies sizes.
	private void SetPositions(){
		Vector2 tempPos;
		Vector2 initPos;
		
		initPos.x = totalWidth/2 * -1;
		initPos.y = (((25*Screen.height) / 100) - Screen.height/2) * -1;
		tempPos = initPos;
		int i,e;
		i = e = 0;
		
		for(i = 0; i < matrixWidth; i++){
			for(e = 0; e < matrixHeight; e++){
				RealEnemyMatrix[i,e].transform.position = tempPos;
				tempPos.y += (RealEnemyMatrix[i,e].transform.localScale.y + (RealEnemyMatrix[i,e].transform.localScale.y / 2));
			}
			tempPos.y = initPos.y;
			tempPos.x += RealEnemyMatrix[i,0].transform.localScale.x * 2;
		}
	}
	
	// ----------
	
	private void MoveAll(){
		float movement = 0;
		if(currentDirection == Directions.right){
			// Check the right-most enemy's position
			if(RealEnemyMatrix[matrixWidth-1,0].transform.position.x + (Enemy.transform.localScale.x * 2) > Screen.width/2){
				// It will go out of the screen, so, better go down, and go the opposite side.
				MoveDown();
				currentDirection = Directions.left;
			}
			else{
				movement = Enemy.transform.localScale.x;
			}
		}
		else{ // They should go left.
			// Check the left-most enemy's position
			if(RealEnemyMatrix[0,0].transform.position.x - (Enemy.transform.localScale.x * 2) < -(Screen.width/2)){
				// It will go out of the screen, so, better go down, and go the opposite side.
				MoveDown();
				currentDirection = Directions.right;
			}
			else{
				movement = Enemy.transform.localScale.x * -1;
			}
		}
		
		for(int i = 0; i < matrixWidth; i++){
			for(int e = 0; e < matrixHeight; e++){
				RealEnemyMatrix[i,e].GetComponent<CEnemy>().Move(movement); // call each enemy's Move()
//				spr.SwitchSpriteState(ref RealEnemyMatrix[i,e]); // switch the sprite image
				Managers.SpriteMgr.SwitchSpriteState(ref RealEnemyMatrix[i,e]);
			}
		}
	}
	
	// ----------
	
	// Move each Enemy down by one enemy height.
	// Checks if the enemies got to the bottom of the screen. If so, it resets them.
	private void MoveDown(){
		if(RealEnemyMatrix[0,0].transform.position.y + (Enemy.transform.localScale.y * 2) <= -(Screen.height/2)){
			ResetEnemies(false);
			return;
		}
		
		Vector3 tempPos = Vector3.zero;
		for(int i = 0; i < matrixWidth; i++){
			for(int e = 0; e < matrixHeight; e++){
				tempPos = RealEnemyMatrix[i,e].transform.position;
				tempPos.y -= Enemy.transform.localScale.y;
				RealEnemyMatrix[i,e].transform.position = tempPos;
			}
		}
	}
	
	// ----------
	
	// Someone was hit. Start destroying dudes according to color/type.
	public void SequentialDestroy(int x, int y){
		Debug.Log(x + " " + y + " was hit. It's of type: " + LogicEnemyMatrix[x,y]);
		int typeToDestroy = LogicEnemyMatrix[x,y];
		int tempX;
		int tempY;
		
		List<GameObject> currentlyChecking = new List<GameObject>(); // Batch of objects being checked at the moment.
		List<GameObject> pendingChecking = new List<GameObject>(); // Objects that will be checked as soon as currentlyChecking finishes (as lists cannot be modified while iterated.)
		List<GameObject> toKill = new List<GameObject>(); // Checked and ready to be removed.
		
		pendingChecking.Add(RealEnemyMatrix[x,y]); // Add the first hit enemy. For it's surroundings to be checked.
		
		// While there are things to be checked.
		while(pendingChecking.Count != 0){
			
			// Move from Pending to Currently
			Debug.Log("Moving lists");
			
			currentlyChecking.Clear(); // Make sure that the contents are new.
			
			// Make a deep copy of the list.
			foreach(GameObject go in pendingChecking){
				currentlyChecking.Add(go);
			}
			pendingChecking.Clear();
			

			foreach(GameObject obj in currentlyChecking){ // Check around each element in ToCheck
				// Convenience
				tempX = obj.GetComponent<CEnemy>().GetX();
				tempY = obj.GetComponent<CEnemy>().GetY();
				
				// The current enemy will be removed after all of this.
				Debug.Log("Adding to kill list (from currentlyChecking): " + tempX + ":" + tempY);
				toKill.Add(RealEnemyMatrix[tempX,tempY]);
				
				// Surroundings checkings. Extensive and redundant implementation that must be optimized.
				// LEFT
				
				try{
					// Check left of hit enemy.
					if(LogicEnemyMatrix[tempX - 1, tempY] == typeToDestroy){ // If the enemy to the LEFT is of the type we look for...
						Debug.Log("Type " + typeToDestroy + " found (left). Adding: " + (tempX-1) + "," + tempY);
						if(!toKill.Contains(RealEnemyMatrix[tempX-1,tempY]) && RealEnemyMatrix[tempX-1,tempY].GetComponent<CEnemy>().GetMarked() == false){ // ... and it's not in the kill list yet (which would mean
							pendingChecking.Add(RealEnemyMatrix[tempX-1,tempY]); //  that the surroundings were already checked) add it to the Pending list.
						}
						else{
							Debug.Log("Already added, ignoring"); // Already checked and marked to be removed.
						}
					}
					else{
						Debug.Log("Not the same, not adding"); // Not the kind we look for.
					}
				}
				catch(System.Exception e){
					Debug.LogWarning("Caught exception: " + e); // We are checking outside the matrix.
				}
				
				/*
				 * Rinse and repeat for all other directions
				 */
				// RIGHT
				
				try{	
					// Check right of hit enemy.
					if(LogicEnemyMatrix[tempX + 1, tempY] == typeToDestroy){
						Debug.Log("Type " + typeToDestroy + " found (right). Adding: " + (tempX+1) + "," + tempY);
						if(!toKill.Contains(RealEnemyMatrix[tempX+1,tempY]) && RealEnemyMatrix[tempX+1,tempY].GetComponent<CEnemy>().GetMarked() == false){
							pendingChecking.Add(RealEnemyMatrix[tempX+1,tempY]);
						}
						else{
							Debug.Log("Already added, ignoring");
						}
					}
					else{
						Debug.Log("Not the same, not adding");	
					}
					Debug.Log("Finished checking.");
				}
				catch(System.Exception e){
					Debug.LogWarning("Caught exception: " + e);
				}
				
				// UP
				
				try{
					// Check up of hit enemy.
					if(LogicEnemyMatrix[tempX, tempY + 1] == typeToDestroy){
						Debug.Log("Type " + typeToDestroy + " found (up). Adding: " + tempX + "," + (tempY + 1));
						if(!toKill.Contains(RealEnemyMatrix[tempX,tempY+1]) && RealEnemyMatrix[tempX,tempY+1].GetComponent<CEnemy>().GetMarked() == false){
							pendingChecking.Add(RealEnemyMatrix[tempX,tempY+1]);
						}
						else{
							Debug.Log("Already added, ignoring");
						}
					}
					else{
						Debug.Log("Not the same, not adding");
					}
				}
				catch(System.Exception e){
					Debug.LogWarning("Caught exception: " + e);
				}
				
				// DOWN
				
				try{
					// Check down of hit enemy.
					if(LogicEnemyMatrix[tempX, tempY - 1] == typeToDestroy){
						Debug.Log("Type " + typeToDestroy + " found (down). Adding: " + tempX + "," + (tempY - 1));
						if(!toKill.Contains(RealEnemyMatrix[tempX,tempY-1]) && RealEnemyMatrix[tempX,tempY-1].GetComponent<CEnemy>().GetMarked() == false){
							pendingChecking.Add(RealEnemyMatrix[tempX,tempY-1]);
						}
						else{
							Debug.Log("Already added, ignoring");
						}
					}
					else{
						Debug.Log("Not the same, not adding");
					}
				}
				catch(System.Exception e){
					Debug.LogWarning("Caught exception: " + e);
				}
			}
		}
		
		float chainKills = 0.0f;
				
		// Everything was checked, now destroy all elements in the toKill list.
		foreach(GameObject go in toKill){
			
			chainKills += 1.0f;
			
			// Up the score, according to in which row the enemy was.
			switch(go.GetComponent<CEnemy>().GetY()){
			case 0:
				Managers.Game.ModifyScore(row0Score);
				break;
			case 1:
				Managers.Game.ModifyScore(row1Score);
				break;
			case 2:
				Managers.Game.ModifyScore(row2Score);
				break;
			case 3:
				Managers.Game.ModifyScore(row3Score);
				break;
			default:
				Managers.Game.ModifyScore(1);
				break;
			}
			
			Debug.Log("Killing: " + go.GetComponent<CEnemy>().GetX() + " " + go.GetComponent<CEnemy>().GetY());
			go.GetComponent<CEnemy>().DestroyMe(); // Tell the enemy to destroy itself.
			aliveEnemies--;
			
			// If the enemy was enabled to fire, delegate the firing responsability to the one (living) above
			if(readyToFire.Contains(go)){
				Debug.Log("Removing from RTF list");
				readyToFire.Remove(go); // Not ready to fire anymore...
				try{
					int n = 1;
					tempX = go.GetComponent<CEnemy>().GetX();
					tempY = go.GetComponent<CEnemy>().GetY();
					
					while(RealEnemyMatrix[tempX,(tempY+n)].gameObject.active == false){
						Debug.Log("Going up");
						n++;
					}
					Debug.Log("Adding to RTF: " + go.GetComponent<CEnemy>().GetX() + "," +(go.GetComponent<CEnemy>().GetY() + n));
					readyToFire.Add(RealEnemyMatrix[go.GetComponent<CEnemy>().GetX(),(go.GetComponent<CEnemy>().GetY() + n)]); // ... but the ALIVE guy above him IS now able to fire.
				}
				catch(System.Exception e){
					// There's no one above.
					Debug.Log("That was the last one of the column: " + e);
				}
			}
		}
		
		// Clear kill list
		toKill.Clear();
		
		// Reward long chains.
		int extraScore = (int)(Mathf.Pow(exponentialScore, chainKills));
		Managers.Game.ModifyScore(extraScore);
		
		// Update the remaining count in the GameManager
		Managers.Game.UpdateRemainingEnemies(aliveEnemies);
	}
	
	// ----------
	
}
