using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour {

	public Slider volumeSlider;
	public Text volumeText;

	void Start () {
		volumeSlider.value = Master.control.volume * 20f;
		volumeText.text = (Master.control.volume * 100f).ToString () + "%";
	}

	public void ChangeVolume(float newval){
		float newvolume = newval * .05f;
		Master.control.volume = newvolume;
		PlayerPrefs.SetFloat ("volume", newvolume);
		volumeText.text = (Master.control.volume * 100f).ToString () + "%";
	}

}
