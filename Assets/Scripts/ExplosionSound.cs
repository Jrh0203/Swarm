using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionSound : MonoBehaviour {

	AudioSource sound;
	// Use this for initialization
	void Start () {
		sound = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (!sound.isPlaying) {
			Destroy(gameObject);
		}
	}
}
