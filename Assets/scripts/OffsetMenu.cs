using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OffsetMenu : MonoBehaviour {

	public Slider offsetPreviewSlider;
	public Slider offsetSlider;
	public Text offsetText;
	public AudioSource tick;
	public GameObject paddleFlash;

	private float timerCounter;
	private float targetTime;
	private bool played;
	private bool keepVal;

	void OnEnable () {
		tick.volume = Master.control.volume;
		played = false;
		keepVal = false;
		timerCounter = 0f;
		targetTime = .85f + ((Master.control.offset - 2f) * 0.05f);
		offsetSlider.value = Master.control.offset;
		offsetText.text = Master.control.offset.ToString ();
	}

	public void ChangeOffset(float newval){
		Master.control.offset = newval;
		PlayerPrefs.SetFloat ("offset", newval);
		offsetText.text = Master.control.offset.ToString ();
		targetTime = .85f + ((Master.control.offset - 2f) * 0.05f);
	}

	void FixedUpdate(){
		timerCounter += Time.deltaTime;
		if (timerCounter < targetTime) {
			if (timerCounter <= 1f) {
				offsetPreviewSlider.value = timerCounter;
			} else {
				if (keepVal == false) {
					keepVal = true;
					paddleFlash.SetActive (true);
					Invoke ("FlashDisable", 0.05f);
				}
				offsetPreviewSlider.value = timerCounter - 1f;
			}
		} else if (timerCounter > 1f) {
			Tick ();
			timerCounter = 0f;
			played = false;
			if (keepVal == true) {
				timerCounter = offsetPreviewSlider.value;
				keepVal = false;
			} else {
				offsetPreviewSlider.value = 0f;
				paddleFlash.SetActive (true);
				Invoke ("FlashDisable", 0.05f);
			}
		} else {
			offsetPreviewSlider.value = timerCounter;
			Tick ();
		}
	}

	private void Tick(){
		if (!played) {
			played = true;
			tick.Play ();
		}
	}

	private void FlashDisable(){
		paddleFlash.SetActive (false);
	}
}
