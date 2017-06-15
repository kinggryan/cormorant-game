using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	MovementState state = new MovementStatePoweredFlight();

	public AnimationCurve verticalMovementCurve;	//!< The curve of vertical movement

	bool underwater;

	// Use this for initialization
	void Start () {
		state.playerTransform = transform;
		state.verticalMovementCurve = verticalMovementCurve;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input vector
		// Give input to your state
		var newState = state.Update(underwater);
		if (newState != null) {
			Debug.Log ("Transitioning to " + newState);
			state = newState;
		}
	}

	void OnTriggerEnter(Collider trigger) {
		// When entering trigger, send that to the movement state
		underwater = true;
		Debug.Log ("Entering water.");
	}

	void OnTriggerExit(Collider trigger) {
		underwater = false;
		Debug.Log ("Exiting water");
	}
}
