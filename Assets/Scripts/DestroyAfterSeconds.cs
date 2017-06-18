using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour {

	public float lifespan = 5f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		lifespan -= Time.deltaTime;
		if (lifespan <= 0) {
			GameObject.Destroy (gameObject);
		}
	}
}
