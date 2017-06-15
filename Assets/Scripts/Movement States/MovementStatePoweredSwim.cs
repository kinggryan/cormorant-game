using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStatePoweredSwim : MovementState {

	float currentVerticalTurnSpeed = 0f;
	float verticalTurnAcceleration = 90f;

	// Use this for initialization
	public MovementStatePoweredSwim() {
		lateralTurnAcceleration = 80f;
		lateralTurnMaxSpeed = 80f;
		upwardTurnAcceleration = 1f;
		upwardTurnSteepestAngle = 80f;
		downwardTurnAcceleration = 1f;
		downwardTurnSteepestAngle = 80f;
		verticalTurnMaxSpeed = 65f;

		forwardAcceleration = 50f;
		forwardMaxSpeed = 10f;
		forwardMinSpeed = 5f;
		maxLateralDrag = 10f;
		gravity = -2f;			//!< Negative gravity so you float upwards
	}

	protected override MovementState TransitionToState() {
		if (GetDiveButtonPressed ()) {
			var newState = new MovementStateUnpoweredSwim ();
			return UpdateNewState (newState);
		} else if (!underwater) {
			var newState = new MovementStatePoweredFlight ();
			return UpdateNewState (newState);
		}
		return null;
	}

	protected override void UpdateTurningWithInput(Vector3 inputVector) {
		// We basically ignore the vertical component of the base's input and replace it with our own.
		base.UpdateTurningWithInput (inputVector);
		var targetVerticalTurnSpeed = -inputVector.y * verticalTurnMaxSpeed;
		currentVerticalTurnSpeed = Mathf.MoveTowards (currentVerticalTurnSpeed, targetVerticalTurnSpeed, Time.deltaTime * verticalTurnAcceleration);
	}

	protected override void UpdateVerticalRotation() {
		playerTransform.Rotate (currentVerticalTurnSpeed * Vector3.right * Time.deltaTime, Space.Self);
		// Now update so that our local Y axis is aligned to up.
		var targetRotation = Quaternion.LookRotation(playerTransform.forward,Vector3.Cross(playerTransform.forward,Vector3.ProjectOnPlane(playerTransform.right,Vector3.up)));
		var newRotation = Quaternion.Slerp (playerTransform.rotation, targetRotation, 10 * Time.deltaTime);
		playerTransform.rotation = newRotation;
	}
}
