using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLCollider : MonoBehaviour {

	public GameObject flash;

	void OnCollisionEnter2D(Collision2D col){
		if (col.transform.position.y > Camera.main.transform.position.y - 10f) {
			flash.SetActive (true);
			Invoke ("FlashEnd", 0.05f);
		}
	}

	private void FlashEnd(){
		flash.SetActive (false);
	}

}
