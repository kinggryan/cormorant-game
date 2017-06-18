using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateUnpoweredFlight : MovementState {

	float rotationSlerpRate = 10f;

	// Use this for initialization
	public MovementStateUnpoweredFlight() {
		lateralTurnAcceleration = 60f;
		lateralTurnMaxSpeed = 30f;
		upwardTurnAcceleration = 50f;
		upwardTurnSteepestAngle = 90f;
		downwardTurnAcceleration = 50f;
		downwardTurnSteepestAngle = 90f;

		forwardAcceleration = 0f;
		forwardMaxSpeed = 100f;
		forwardMinSpeed = 5f;
		maxLateralDrag = 0f;
		gravity = 9f;
	}

	public override float GetCurrentVerticalTurnSpeed() {
		return 0f;
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
		var targetRotationLocalForward = currentVelocity;
		// Rotate local forward based on the input
		var amountToTurn = currentVerticalAngleXAxis > 0 ? upwardTurnAcceleration*currentVerticalAngleXAxis : downwardTurnAcceleration*currentVerticalAngleXAxis;
//		Debug.Log ("Amount to turn " + amountToTurn);
		targetRotationLocalForward = Quaternion.AngleAxis (amountToTurn * Time.deltaTime, Vector3.Cross (currentVelocity, Vector3.up))*targetRotationLocalForward;

		var targetRotationLocalUp = Vector3.Cross (targetRotationLocalForward, playerTransform.right);
		var targetRotation = Quaternion.LookRotation (targetRotationLocalForward, targetRotationLocalUp);
		var currentRotation = Quaternion.Slerp (playerTransform.rotation, targetRotation, rotationSlerpRate * Time.deltaTime);
		playerTransform.rotation = currentRotation;
	}
}
