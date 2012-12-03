using UnityEngine;
using System.Collections;

public interface IActor {
	
	// Move around the actor
	//void Move(Direction d);
	void Move(float move);
	
	// Fire weapon, either up (player) or downwards (enemy).
	void Shoot(GameObject ammo);
	
	// Destroy and remove from scene.
	// User should call this when having 0 lives.
	void DestroyMe();
	
}
