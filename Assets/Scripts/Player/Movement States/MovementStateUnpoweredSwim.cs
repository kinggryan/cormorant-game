﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateUnpoweredSwim : MovementState {

	float verticalTurnAcceleration = 90f;

	// Use this for initialization
	public MovementStateUnpoweredSwim() {
		lateralTurnAcceleration = 120f;
		lateralTurnMaxSpeed = 60f;
		upwardTurnAcceleration = 120f;
		upwardTurnSteepestAngle = 90f;
		downwardTurnAcceleration = 120f;
		downwardTurnSteepestAngle = 90f;
		verticalTurnMaxSpeed = 90f;

		forwardAcceleration = 50f;
		forwardMaxSpeed = 100f;
		forwardMinSpeed = 5f;
		maxLateralDrag = 15f;
		forwardDrag = 2f;
		gravity = 0f;
	}

	public override float GetCurrentVerticalTurnSpeed() {
		return currentVerticalTurnSpeed;
	}

	protected override MovementState TransitionToState() {
		if (GetDiveButtonReleased ()) {
			var newState = new MovementStatePoweredSwim ();
			return UpdateNewState (newState);
		} else if (!underwater) {
			var newState = new MovementStateUnpoweredFlight ();
			return UpdateNewState (newState);
		}
		return null;
	}

	protected override void UpdateVerticalRotation() {
		// Based on our replaced updated vertical turning
		playerTransform.Rotate (currentVerticalTurnSpeed * Vector3.right * Time.deltaTime, Space.Self);
	}

	protected override void UpdateTurningWithInput(Vector3 inputVector) {
		// We basically ignore the vertical component of the base's input and replace it with our own.
		base.UpdateTurningWithInput (inputVector);
		var targetVerticalTurnSpeed = -inputVector.y * verticalTurnMaxSpeed;
		currentVerticalTurnSpeed = Mathf.MoveTowards (currentVerticalTurnSpeed, targetVerticalTurnSpeed, Time.deltaTime * verticalTurnAcceleration);
	}


	protected override void UpdateVelocity() {
//		Debug.Log ("Pre rotate: " + currentVelocity + " rotating " + currentVelocity + " towards " + playerTransform.forward*currentVelocity.magnitude + " max turn radians " + (verticalTurnMaxSpeed+lateralTurnMaxSpeed)*0.5f*Mathf.Deg2Rad*Time.deltaTime);
		currentVelocity = Vector3.RotateTowards(currentVelocity,playerTransform.forward*currentVelocity.magnitude,(verticalTurnMaxSpeed+lateralTurnMaxSpeed)*0.5f*Mathf.Deg2Rad*Time.deltaTime,Mathf.Infinity);
//		Debug.Log ("Post rotate: " + currentVelocity);

		// If above max speed, apply max drag
		if (currentVelocity.magnitude > forwardMaxSpeed) {
			currentVelocity = forwardMaxSpeed * currentVelocity.normalized;
		}

		if (currentVelocity.magnitude > forwardMinSpeed) {
			// Apply backward drag
			currentVelocity = Vector3.MoveTowards (currentVelocity, Vector3.zero, forwardDrag * Time.deltaTime);
		}
	}

//	protected override void UpdateLateralRotation() {
//		// Rotate around forward axis
//		playerTransform.Rotate (-currentLocalLateralTurnSpeed * Vector3.forward * Time.deltaTime, Space.Self);
//	}
}
