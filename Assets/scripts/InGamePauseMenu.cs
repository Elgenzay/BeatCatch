using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGamePauseMenu : MonoBehaviour {

	public GameObject mapPlayer;
	public GameObject volumeMenuParent;
	public GameObject progressText;
	public GameObject progressSlider;

	void OnEnable(){
		progressSlider.GetComponent<Slider> ().value = mapPlayer.GetComponent<MapPlayer> ().songSource.time / mapPlayer.GetComponent<MapPlayer> ().map [mapPlayer.GetComponent<MapPlayer> ().map.Length - 1].time;
		progressText.GetComponent<Text> ().text = " " + ((mapPlayer.GetComponent<MapPlayer> ().songSource.time / mapPlayer.GetComponent<MapPlayer> ().map [mapPlayer.GetComponent<MapPlayer> ().map.Length - 1].time) * 100f).ToString ("F0") + "%";
	}

	public void VolumeMenu(){
		if (volumeMenuParent.activeSelf) {
			volumeMenuParent.SetActive (false);
		} else {
			volumeMenuParent.SetActive (true);
		}
	}
}
