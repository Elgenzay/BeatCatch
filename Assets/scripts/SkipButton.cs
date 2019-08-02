using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkipButton : MonoBehaviour {

	private Button buttonScript;
	private float acceleration;

	void Start () {
		buttonScript = this.GetComponent<Button> ();
		acceleration = 0f;
	}

	void FixedUpdate () {
		if (!buttonScript.interactable) {
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y + acceleration, this.transform.position.z);
			acceleration += 0.5f;
			if (acceleration > 25f) {
				this.gameObject.SetActive (false);
			}
		}
	}
}
