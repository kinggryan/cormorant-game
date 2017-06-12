using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStatePoweredFlight : MovementState {

	// Use this for initialization
	public MovementStatePoweredFlight() {
		lateralTurnAcceleration = 60f;
		lateralTurnMaxSpeed = 30f;
		upwardTurnAcceleration = 1f;
//		upwardTurnMaxSpeed = 30f;
		upwardTurnSteepestAngle = 25f;
		downwardTurnAcceleration = 1f;
//		downwardTurnMaxSpeed = 30f;
		downwardTurnSteepestAngle = 25f;

		forwardAcceleration = 50f;
		forwardMaxSpeed = 15f;
		forwardMinSpeed = 5f;
		returnToMaxSpeedAcceleration = 50f;
		returnToMinSpeedAcecleration = 50f;
		forwardDrag = 25f;
		maxLateralDrag = 50f;
		gravity = 0f;
	}
}
