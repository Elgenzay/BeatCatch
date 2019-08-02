using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour {

	public bool isDead;
	public GameObject flashPaddle;

	private int flashTimer;

	void Start () {
		flashTimer = -1;
		isDead = false;
		this.transform.localScale = new Vector3 (100f, 20f, 1f);
		this.transform.position = new Vector3(0f,6.5f,0f);
	}

	void FixedUpdate(){
		if (!isDead) {
			if (Input.touchCount > 0 && Application.platform == RuntimePlatform.Android) {
				this.transform.position = new Vector3 (Camera.main.ScreenToWorldPoint (new Vector3 (Input.touches [0].position.x, 0f, 0f)).x, 6.5f, 0f);
			} else {
				this.transform.position = new Vector3 (Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, 0f, 0f)).x, 6.5f, 0f);
			}
		} else {
			this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - 0.025f, this.transform.position.z);
		}

		if (flashTimer > 0) {
			flashTimer--;
		} else if (flashTimer == 0) {
			flashTimer = -1;
			flashPaddle.SetActive (false);
		}
	}

	public void Flash(){
		flashPaddle.SetActive (true);
		flashTimer = 3;
	}
}
