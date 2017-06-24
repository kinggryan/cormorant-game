using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	MovementState state = new MovementStatePoweredFlight();

	public PlayerAnimationManager animationManager;
	public PlayerCameraController cameraController;
	public AnimationCurve verticalMovementCurve;	//!< The curve of vertical movement

	public GameObject waterSplashPrefab;

    [HideInInspector]
	public bool underwater;

	// Use this for initialization
	void Start () {
		state.playerTransform = transform;
		state.verticalMovementCurve = verticalMovementCurve;
	}
	
	// Update is called once per frame
	void Update () {
		// Get input vector
		// Give input to your state
		var newState = state.Update(underwater);
		if (newState != null) {
			Debug.Log ("Transitioning to " + newState);
			state = newState;
		}

		animationManager.UpdateMovementParams (state.GetCurrentLateralTurnSpeed (), state.GetCurrentVerticalTurnSpeed(), 0, underwater, false);
		cameraController.UpdateMovementParameters (state.GetCurrentVelocity ().magnitude / 15, 0);
	}

	void OnTriggerEnter(Collider trigger) {
        // When entering trigger, send that to the movement state
        var fish = trigger.GetComponent<Fish>();
        if (fish) {
            // Destroy the fish
            GameObject.Instantiate(fish.deathEffectPrefab, fish.transform.position, fish.transform.rotation);
			GameObject.Destroy(trigger.gameObject);
        }
        else {
			underwater = true;
			CreateSplashEffect (state.GetCurrentVelocity().magnitude/50f);
		}
	}

	void OnTriggerExit(Collider trigger) {
		if (trigger.GetComponent<Fish> ()) {
			// Do nothing
		} else {
			underwater = false;
			CreateSplashEffect (state.GetCurrentVelocity().magnitude/50f);
		}
	}

	void CreateSplashEffect(float speedScaling) {
		var waterSplashObj = GameObject.Instantiate (waterSplashPrefab, transform.position, Quaternion.AngleAxis(-90,Vector3.right));
		var waterSplashSystem = waterSplashObj.GetComponent<ParticleSystem> ();
		var emission = waterSplashSystem.emission;
//		Debug.Log ("Scaling " + emission.rateOverTimeMultiplier);
		emission.rateOverTimeMultiplier *= speedScaling;
	}
}
