using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateUnpoweredFlight : MovementState {

	float rotationSlerpRate = 10f;

	// Use this for initialization
	public MovementStateUnpoweredFlight() {
		lateralTurnAcceleration = 60f;
		lateralTurnMaxSpeed = 30f;
		upwardTurnAcceleration = 0f;
		upwardTurnSteepestAngle = 90f;
		downwardTurnAcceleration = 0f;
		downwardTurnSteepestAngle = 90f;

		forwardAcceleration = 0f;
		forwardMaxSpeed = 100f;
		forwardMinSpeed = 5f;
		maxLateralDrag = 0f;
		gravity = 9f;
	}

	protected override MovementState TransitionToState() {
		if (GetDiveButtonReleased ()) {
			var newState = new MovementStatePoweredFlight ();
			return UpdateNewState (newState);
		} else if (underwater) {
			var newState = new MovementStateUnpoweredSwim ();
			return UpdateNewState (newState);
		}
		return null;
	}

	protected override void UpdateVerticalRotation() {
		var targetRotationLocalUp = Vector3.Cross (currentVelocity, playerTransform.right);
		var targetRotation = Quaternion.LookRotation (currentVelocity, targetRotationLocalUp);
		var currentRotation = Quaternion.Slerp (playerTransform.rotation, targetRotation, rotationSlerpRate * Time.deltaTime);
		playerTransform.rotation = currentRotation;
	}
}
