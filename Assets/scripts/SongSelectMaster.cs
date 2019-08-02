using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongSelectMaster : MonoBehaviour {

	public GameObject canvasParent;
	public GameObject settingsMenuParent;
	public GameObject modsMenuParent;

	private bool transitioning;
	private float transitionSpeed = 0f;

	public void Back(){
		Master.control.SceneMainMenu ();
	}

	// Use this for initialization
	void Start () {
		Camera.main.aspect = 480f / 800f;
	}

	public void Settings(){
		if (settingsMenuParent.activeSelf) {
			settingsMenuParent.SetActive (false);
		} else {
			settingsMenuParent.SetActive (true);
		}
	}

	public void Mods(){
		if (modsMenuParent.activeSelf) {
			modsMenuParent.SetActive (false);
		} else {
			modsMenuParent.SetActive (true);
		}
	}

	public void ClearScores(){
		Master.control.ClearScores ();
		Master.control.SceneSongSelect ();
	}

	public void GameScene(){
		transitioning = true;
	}

	void FixedUpdate(){
		if (transitioning) {
			transitionSpeed += 4f;
			canvasParent.transform.position = new Vector3 ((canvasParent.transform.position.x - transitionSpeed), canvasParent.transform.position.y, canvasParent.transform.position.z);
			if (canvasParent.transform.position.x < -500f) {
				Master.control.SceneInGame ();
			}
		}
	}

}