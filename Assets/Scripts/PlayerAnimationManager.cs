using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void UpdateMovementParams(float lateralTurnAngle, float verticalTurnAngle, float speed, bool underwater, bool powered) {
		// For now, just use lateral turn angle
		var turnQuat = Quaternion.AngleAxis(0.25f*verticalTurnAngle,Vector3.right)*Quaternion.AngleAxis(-lateralTurnAngle,Vector3.forward);
		var localForward = turnQuat*Vector3.forward;
		var localUp = turnQuat * Vector3.up;
		var localRotation = Quaternion.LookRotation(localForward,localUp);
		transform.localRotation = localRotation;
	}
}
