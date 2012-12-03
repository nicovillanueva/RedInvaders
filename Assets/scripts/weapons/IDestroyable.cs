using UnityEngine;
using System.Collections;

public interface IDestroyable {
	
	// Just to make sure that stuff can be recycled.
	void DestroyMe();
	
}
