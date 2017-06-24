using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish : MonoBehaviour {

	class MovementState : System.Object {

		protected Transform transform;

		protected float minSpeed;
		protected float maxSpeed;
		protected float minTurnAngle;
		protected float maxTurnAngle;
		protected float minTurnSpeed;
		protected float maxTurnSpeed;
		protected float minChangeMovementTime;
		protected float maxChangeMovementTime;

		float currentSpeed;
		float currentTurnAngleRemaining;
		float currentTurnSpeed;
		float changeSpeedTimeRemaining;

		public MovementState(Transform transform) {
			this.transform = transform;
			FinishTurn();
		}

		public virtual MovementState Update() {
			// Turn
			transform.Rotate(Vector3.up,Time.deltaTime*currentTurnSpeed,Space.Self);
			var originalSign = Mathf.Sign (currentTurnAngleRemaining);
			currentTurnAngleRemaining -= currentTurnSpeed * Time.deltaTime;

			// If the sign switched, it means we should stop turning
			if (!Mathf.Approximately(currentTurnSpeed,0) && Mathf.Sign (currentTurnAngleRemaining) * originalSign < 0) {
				FinishTurn ();
			}

			// Determine if we finished moving straight
			if (changeSpeedTimeRemaining >= 0) {
				changeSpeedTimeRemaining -= Time.deltaTime;
				if (changeSpeedTimeRemaining < 0) {
					StartTurn ();
				}
			}

			// Move
			transform.position += transform.forward*currentSpeed*Time.deltaTime;
			Debug.Log ("Current speed: " + currentSpeed);

			return null;
		}

		void StartTurn() {
			var sign = (Random.value < 0.5f ? 1 : -1);
			currentTurnAngleRemaining = sign * Random.Range (minTurnAngle, maxTurnAngle);
			currentTurnSpeed = sign * Random.Range (minTurnSpeed, maxTurnSpeed);
			currentSpeed = Random.Range (minSpeed, maxSpeed);
		}

		void FinishTurn() {
			// Go straight
			currentSpeed = Random.Range(minSpeed,maxSpeed);
			currentTurnSpeed = 0;
			changeSpeedTimeRemaining = Random.Range (minChangeMovementTime, maxChangeMovementTime);
		}

        protected float GetDistanceToPlayer()
        {
            var player = Object.FindObjectOfType<PlayerController>();
            return Vector3.Distance(transform.position, player.transform.position);
        }

        protected bool GetPlayerIsUnderwater()
        {
            var player = Object.FindObjectOfType<PlayerController>();
            return player.underwater;
        }
	}

	class MovementStateDefault : MovementState {
        float scaredDuration = 0f;

		public MovementStateDefault(Transform transform) : base(transform) {
			minSpeed = 5f;
			maxSpeed = 8f;
			minTurnAngle = 25f;
			maxTurnAngle = 180f;
			minTurnSpeed = 45f;
			maxTurnSpeed = 180f;
			minChangeMovementTime = 4f;
			maxChangeMovementTime = 10f;
		}

        public override MovementState Update()
        {
            base.Update();
            // Determine if player is far enough away
            if (GetDistanceToPlayer() < 15f && GetPlayerIsUnderwater())
            {
                scaredDuration += Time.deltaTime;
                if (scaredDuration > 0.75f)
                    return new MovementStateScared(transform);
            }
            
            return null;
        }
    }

    class MovementStateScared : MovementState
    {
        public MovementStateScared(Transform transform) : base(transform)
        {
            minSpeed = 10f;
            maxSpeed = 15f;
            minTurnAngle = 25f;
            maxTurnAngle = 180f;
            minTurnSpeed = 120f;
            maxTurnSpeed = 360f;
            minChangeMovementTime = 10f;
            maxChangeMovementTime = 15f;
        }

        public override MovementState Update()
        {
            base.Update();
            // Determine if player is far enough away
            if(GetDistanceToPlayer() < 15f && GetPlayerIsUnderwater())
            {
                return null;
            } else
            {
                return new MovementStateDefault(transform);
            }
        }
    }

    public GameObject deathEffectPrefab;
	MovementState state;

	// Use this for initialization
	void Start () {
		state = new MovementStateDefault(transform);
	}
	
	// Update is called once per frame
	void Update () {
		var newState = state.Update ();
		if (newState != null)
			state = newState;
	}
}
