using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementState : System.Object {
	public Transform playerTransform;
	public AnimationCurve verticalMovementCurve;

	// Current Movement variables
	protected Vector3 currentVelocity = Vector3.zero;
	protected float currentLocalLateralTurnSpeed;	//!< The turning around the local y axis.
	protected float currentVerticalAngleXAxis;		//!< The turn speed around the local x axis

	// Movement Control Properties
	protected float lateralTurnAcceleration;	//!< How fast the lateral turn angle changes if movement is fully held in that direction
	protected float lateralTurnMaxSpeed;		//!< How fast we can possibly turn laterally
	protected float upwardTurnAcceleration;
	protected float upwardTurnSteepestAngle;	//!< The steepest angle you can ascend at
	protected float downwardTurnAcceleration;
	protected float downwardTurnSteepestAngle;	//!< THe steepest angle you can descend at

	protected float forwardAcceleration;		//!< The speed used to calculate the desired forward speed.
	protected float forwardMaxSpeed;
	protected float forwardMinSpeed;
	protected float returnToMaxSpeedAcceleration;	//!< The rate at which the player will return to the max speed if exceeding it.
	protected float returnToMinSpeedAcecleration;	//!< The rate at which the player will return to the min speed if below it.
	protected float forwardDrag;				//!< The amount to reduce the forward speed per second, until reaching min speed.
	protected float maxLateralDrag;				//!< THe maximum drag applied laterally, based on the difference between intended movement direction and actual movement direction. This is based on the dot product of the two vectors.
	protected float gravity;					//!< The acceleration along the y axis.

	public virtual MovementState Update() {
		var input = GetInputVector();
		UpdateTurningWithInput (input);
		UpdateRotation ();
		UpdateVelocity ();
		Move ();
		return null;
	}

	void UpdateTurningWithInput(Vector3 inputVector) {
		// Do lateral turning
		var targetLateralTurnSpeed = inputVector.x * lateralTurnMaxSpeed;
		currentLocalLateralTurnSpeed = Mathf.MoveTowards (currentLocalLateralTurnSpeed, targetLateralTurnSpeed, lateralTurnAcceleration * Time.deltaTime);

		// Do upward turning
		var accelerationToUse = 0f;
		if (inputVector.y > 0) {
			accelerationToUse = upwardTurnAcceleration;
		} else {
			accelerationToUse = downwardTurnAcceleration;
		}	
		currentVerticalAngleXAxis = Mathf.MoveTowards (currentVerticalAngleXAxis, inputVector.y, accelerationToUse*Time.deltaTime);
	}

	void UpdateRotation() {
		// Rotate laterally
		playerTransform.Rotate (currentLocalLateralTurnSpeed * Vector3.up * Time.deltaTime, Space.World);

		// Only rotate up to the steepest allowed angles for vertical rotation
		var originalAngle = 90 - Vector3.Angle(playerTransform.forward,Vector3.up);
		var newVerticalAngle = currentVerticalAngleXAxis > 0 ? verticalMovementCurve.Evaluate(currentVerticalAngleXAxis)*upwardTurnSteepestAngle : verticalMovementCurve.Evaluate(-currentVerticalAngleXAxis)*-downwardTurnSteepestAngle;
		Debug.Log ("New vert angle " + newVerticalAngle + " og angle " + originalAngle);
		var amountToRotate = newVerticalAngle - originalAngle;
		playerTransform.Rotate (-amountToRotate * Vector3.right, Space.Self);
	}

	void UpdateVelocity() {
		var originalVelocity = currentVelocity;
//		Debug.Log ("Cur vel " + currentVelocity + " forward max spd " + forwardMaxSpeed);
//		if (currentVelocity.magnitude < forwardMaxSpeed) {
		currentVelocity = currentVelocity + forwardAcceleration * playerTransform.forward * Time.deltaTime;
//		}
//		currentVelocity += gravity * Vector3.down * Time.deltaTime;
//
//		// Apply lateral drag
//		float amountOfLateralDrag = Vector3.Dot (currentVelocity, playerTransform.forward);
//		var directionOfLateralDrag = Vector3.Project (currentVelocity, playerTransform.forward);
//		directionOfLateralDrag.y = 0;
//		directionOfLateralDrag.Normalize ();
//		currentVelocity += amountOfLateralDrag * maxLateralDrag * Time.deltaTime * directionOfLateralDrag;

		// If above max speed, apply max drag
//		var forwardDragToApply = 0f;
//		if (currentVelocity.magnitude > forwardMaxSpeed) {
//			forwardDragToApply = returnToMaxSpeedAcceleration;
//		} else if (currentVelocity.magnitude > forwardMinSpeed) {
//			forwardDragToApply = forwardDrag;
//		} else if (currentVelocity.magnitude < forwardMinSpeed) {
//			forwardDragToApply = -returnToMinSpeedAcecleration;
//		}
//		Debug.Log ("Forward drag to apply " + forwardDragToApply);
//		currentVelocity -= forwardDragToApply * Time.deltaTime * currentVelocity.normalized;
		if (currentVelocity.magnitude > forwardMaxSpeed) {
			currentVelocity = forwardMaxSpeed * currentVelocity.normalized;
		}
	}

	void Move() {
		playerTransform.position += currentVelocity*Time.deltaTime;
	}

	// Input Getters

	Vector3 GetInputVector() {
		var inputVector = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0);
		return inputVector;
	}

	bool GetDiveButtonPressed() {
		return Input.GetButtonDown("jump");
	}

	bool GetDiveButtonHeld() {
		return Input.GetButton("jump");
	}

	bool GetDiveButtonReleased() {
		return Input.GetButtonUp("jump");
	}
}
