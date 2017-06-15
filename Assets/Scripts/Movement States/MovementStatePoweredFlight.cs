using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStatePoweredFlight : MovementState {

	// Use this for initialization
	public MovementStatePoweredFlight() {
		lateralTurnAcceleration = 60f;
		lateralTurnMaxSpeed = 30f;
		upwardTurnAcceleration = 1f;
		upwardTurnSteepestAngle = 25f;
		downwardTurnAcceleration = 1f;
		downwardTurnSteepestAngle = 25f;
		verticalTurnMaxSpeed = 45f;

		forwardAcceleration = 50f;
		forwardMaxSpeed = 15f;
		forwardMinSpeed = 5f;
		maxLateralDrag = 10f;
		gravity = 0f;
	}

	protected override MovementState TransitionToState() {
		if (GetDiveButtonPressed ()) {
			var newState = new MovementStateUnpoweredFlight ();
			return UpdateNewState (newState);
		} else if (underwater) {
			var newState = new MovementStatePoweredSwim ();
			return UpdateNewState (newState);
		}
		return null;
	}

	protected override void UpdateVerticalRotation() {
		base.UpdateVerticalRotation ();

		// Now update so that our local Y axis is aligned to up.
		var targetRotation = Quaternion.LookRotation(playerTransform.forward,Vector3.Cross(playerTransform.forward,Vector3.ProjectOnPlane(playerTransform.right,Vector3.up)));
		var newRotation = Quaternion.Slerp (playerTransform.rotation, targetRotation, 10 * Time.deltaTime);
		playerTransform.rotation = newRotation;
	}
}
