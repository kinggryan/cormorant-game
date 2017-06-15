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
	protected float verticalTurnMaxSpeed;		//!< The max rate of vertical turning

	protected float forwardAcceleration;		//!< The speed used to calculate the desired forward speed.
	protected float forwardMaxSpeed;
	protected float forwardMinSpeed;
	protected float maxLateralDrag;				//!< THe maximum drag applied laterally, based on the difference between intended movement direction and actual movement direction. This is based on the dot product of the two vectors.
	protected float forwardDrag;
	protected float gravity;					//!< The acceleration along the y axis.

	protected bool underwater;

	public virtual MovementState Update(bool underwater) {
		this.underwater = underwater;
		var input = GetInputVector();
		UpdateTurningWithInput (input);
		UpdateRotation ();
		UpdateVelocity ();
		Move ();
		return TransitionToState();
	}

	protected virtual void UpdateTurningWithInput(Vector3 inputVector) {
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
		UpdateLateralRotation ();
		UpdateVerticalRotation ();
	}

	protected virtual void UpdateLateralRotation() {
		// Rotate laterally
		playerTransform.Rotate (currentLocalLateralTurnSpeed * Vector3.up * Time.deltaTime, Space.World);
	}

	protected virtual void UpdateVerticalRotation() {
		// Only rotate up to the steepest allowed angles for vertical rotation
		var originalAngle = 90 - Vector3.Angle(playerTransform.forward,Vector3.up);
		var newVerticalAngle = currentVerticalAngleXAxis > 0 ? verticalMovementCurve.Evaluate(currentVerticalAngleXAxis)*upwardTurnSteepestAngle : verticalMovementCurve.Evaluate(-currentVerticalAngleXAxis)*-downwardTurnSteepestAngle;
		var amountToRotate = Mathf.Clamp(newVerticalAngle - originalAngle,-verticalTurnMaxSpeed*Time.deltaTime,verticalTurnMaxSpeed*Time.deltaTime);
		playerTransform.Rotate (-amountToRotate * Vector3.right, Space.Self);
	}

	protected virtual void UpdateVelocity() {
		var originalVelocity = currentVelocity;

//		Debug.Log ("Player transform forward: " + playerTransform.forward);
		Debug.Log("Forward acceleration " + forwardAcceleration);
		currentVelocity = currentVelocity + forwardAcceleration * playerTransform.forward * Time.deltaTime;
		currentVelocity += gravity * Vector3.down * Time.deltaTime;

		// Apply lateral drag
//		float amountOfLateralDrag = Vector3.Dot (currentVelocity, playerTransform.forward);
//		var directionOfLateralDrag = Vector3.Project (currentVelocity, playerTransform.forward);
//		directionOfLateralDrag.y = 0;
//		directionOfLateralDrag.Normalize ();
//		Debug.Log ("Pre-drag " + currentVelocity);
//		currentVelocity += amountOfLateralDrag * maxLateralDrag * Time.deltaTime * directionOfLateralDrag;
//		Debug.Log ("Post-drag " + currentVelocity);

		// If above max speed, apply max drag
		if (currentVelocity.magnitude > forwardMaxSpeed) {
			currentVelocity = forwardMaxSpeed * currentVelocity.normalized;
		}

		if (currentVelocity.magnitude > forwardMinSpeed) {
			// Apply backward drag
			var amountOfForwardDrag = currentVelocity.magnitude/forwardMaxSpeed;
			currentVelocity = Vector3.MoveTowards (currentVelocity, Vector3.zero, amountOfForwardDrag * forwardDrag * Time.deltaTime);
			Debug.Log ("Slowing this amount " + (amountOfForwardDrag * forwardDrag * Time.deltaTime));
		}
	}

	void Move() {
		Debug.Log ("VEL " + currentVelocity + " mag " + currentVelocity.magnitude);
		playerTransform.position += currentVelocity*Time.deltaTime;
	}

	protected virtual MovementState TransitionToState() {
		return null;
	} 

	protected MovementState UpdateNewState(MovementState newState) {
		newState.playerTransform = playerTransform;
		newState.verticalMovementCurve = verticalMovementCurve;
		newState.InheritMovementProperties (currentVelocity);
		return newState;
	}

	public void InheritMovementProperties(Vector3 currentVelocity) {
		this.currentVelocity = currentVelocity;
	}

	// Input Getters

	Vector3 GetInputVector() {
		var inputVector = new Vector3 (Input.GetAxis ("Horizontal"), Input.GetAxis ("Vertical"), 0);
		return inputVector;
	}

	protected bool GetDiveButtonPressed() {
		return Input.GetButtonDown("Jump");
	}

	bool GetDiveButtonHeld() {
		return Input.GetButton("Jump");
	}

	protected bool GetDiveButtonReleased() {
		return Input.GetButtonUp("Jump");
	}


}
