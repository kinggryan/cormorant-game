using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashControl : MonoBehaviour {

	public ParticleSystem foamSystem;
	public ParticleSystem ringsSystem;

	public void SetSplashAmount(float amount) {
		var emission = foamSystem.emission;
		emission.rateOverTimeMultiplier *= amount;
	}
}
