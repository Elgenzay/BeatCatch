using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour {

	public Toggle flashToggle;
	public Toggle particlesToggle;
	public Toggle lightingToggle;

	public GameObject lightingWarning;

	void Start(){
		flashToggle.isOn = Master.control.flash;
		particlesToggle.isOn = Master.control.particles;
		lightingToggle.isOn = Master.control.lighting;
		if (Master.control.lighting) {
			lightingWarning.SetActive (true);
		}
	}

	public void Flash(bool newval){
		if (newval) {
			PlayerPrefs.SetInt ("flash", 1);
		} else {
			PlayerPrefs.SetInt ("flash", 0);
		}
		Master.control.flash = newval;
	}

	public void Particles(bool newval){
		if (newval) {
			PlayerPrefs.SetInt ("particles", 1);
		} else {
			PlayerPrefs.SetInt ("particles", 0);
		}
		Master.control.particles = newval;
	}

	public void Lighting(bool newval){
		if (newval) {
			PlayerPrefs.SetInt ("lighting", 1);
			lightingWarning.SetActive (true);
		} else {
			PlayerPrefs.SetInt ("lighting", 0);
			lightingWarning.SetActive (false);
		}
		Master.control.lighting = newval;
	}

}
