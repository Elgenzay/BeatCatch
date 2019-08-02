using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	private float deathTime;

	void Start(){
		deathTime = Time.time + 1f;
	}

	void FixedUpdate () {
		if (deathTime < Time.time) {
			Destroy (this.gameObject);
		}
	}
}
