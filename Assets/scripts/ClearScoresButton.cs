using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ClearScoresButton : MonoBehaviour, IPointerDownHandler {

	public Button progressIndicator;

	private bool isPressed;
	private float pressProgress;

	public void OnPointerDown(PointerEventData eventData){
		isPressed = true;
	}

	public void Release(){
		isPressed = false;
		pressProgress = 1f;
		progressIndicator.image.fillAmount = 1f;
	}

	void Start(){
		pressProgress = 1f;
	}

	void FixedUpdate(){
		if (isPressed) {
			bool buttonHeld = false;
			if (Application.isMobilePlatform) {
				if (Input.touchCount != 0) {
					buttonHeld = true;
				}
			} else if (Input.GetKey (KeyCode.Mouse0)) {
				buttonHeld = true;
			}
			if (buttonHeld) {
				pressProgress -= 0.025f;
				progressIndicator.image.fillAmount = pressProgress;
				if (pressProgress <= 0f) {
					Master.control.ClearScores ();
					Master.control.SceneSongSelect ();
				}
			} else {
				Release ();
				isPressed = false;
			}
		} else {
			Release ();
		}
	}
}
