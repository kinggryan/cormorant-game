using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

	public float scrollSpeed;
	public Renderer waveRenderer;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		// Scroll the waves
		var newOffset = waveRenderer.material.GetTextureOffset ("_MainTex");
		newOffset.y += scrollSpeed * Time.deltaTime;
		waveRenderer.material.SetTextureOffset ("_MainTex", newOffset);
	}
}
