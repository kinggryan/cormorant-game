using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStatePoweredFlight : MovementState {

	float verticalTurnAcceleration = 45f;
	float verticalTurnClampForce = 180f;

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

	public override float GetCurrentVerticalTurnSpeed() {
		return currentVerticalTurnSpeed;
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

	protected override void UpdateTurningWithInput(Vector3 inputVector) {
		// We basically ignore the vertical component of the base's input and replace it with our own.
		base.UpdateTurningWithInput (inputVector);
		var targetVerticalTurnSpeed = -inputVector.y * verticalTurnMaxSpeed;
		currentVerticalTurnSpeed = Mathf.MoveTowards (currentVerticalTurnSpeed, targetVerticalTurnSpeed, Time.deltaTime * verticalTurnAcceleration);
	}

	protected override void UpdateVerticalRotation() {
		if (!((currentVerticalTurnSpeed < 0 && Vector3.Angle (playerTransform.forward, Vector3.up) > 90 - upwardTurnSteepestAngle) || (currentVerticalTurnSpeed > 0 && Vector3.Angle (playerTransform.forward, Vector3.down) > 90 - downwardTurnSteepestAngle)))
			currentVerticalTurnSpeed = Mathf.MoveTowards (currentVerticalTurnSpeed, 0, verticalTurnClampForce * Time.deltaTime);

		playerTransform.Rotate (currentVerticalTurnSpeed * Vector3.right * Time.deltaTime, Space.Self);
		// Now update so that our local Y axis is aligned to up.
		var targetRotation = Quaternion.LookRotation(playerTransform.forward,Vector3.Cross(playerTransform.forward,Vector3.ProjectOnPlane(playerTransform.right,Vector3.up)));
		var newRotation = Quaternion.Slerp (playerTransform.rotation, targetRotation, 10 * Time.deltaTime);
		playerTransform.rotation = newRotation;
	}
}
