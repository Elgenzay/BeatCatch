using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorEraser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyUp (KeyCode.D)) {
			Destroy (this.gameObject);
		}
	}

	void FixedUpdate(){
		this.gameObject.transform.position = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
	}

	void OnCollisionEnter2D(Collision2D other){
		if (other.gameObject.name != "rlcollider") {
			Destroy (other.gameObject);
		}
	}
}
