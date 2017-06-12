using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	MovementState state = new MovementStatePoweredFlight();

	public AnimationCurve verticalMovementCurve;	//!< The curve of vertical movement

	// Use this for initialization
	void Start () {
		state.playerTransform = transform;
		state.verticalMovementCurve = verticalMovementCurve;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input vector
		// Give input to your state
		var newState = state.Update();
		if (newState != null) {
			state = newState;
		}
	}


}
