using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

[System.Serializable]
public struct beat {
	public float time;
	public float place;
	public short type;
}

public class Master : MonoBehaviour {

	public int songAmount;

	public static Master control;

	public short song;

	public float volume;
	public float offset;
	public float multiplier = 1f;

	public bool particles;
	public bool flash;
	public bool lighting;

	public bool haste = false;
	public bool dim = false;

	public void SceneMainMenu(){
		SceneManager.LoadScene ("MainMenu");
		Time.timeScale = 1f;
	}

	public void SceneSongSelect(){
		SceneManager.LoadScene ("SongSelect");
		Time.timeScale = 1f;
	}

	public void SceneInGame(){
		SceneManager.LoadScene ("InGame");
		Time.timeScale = 1f;
	}

	void Awake(){
		//Screen.SetResolution(480, 800, false);

		Application.targetFrameRate = 60;
		if (PlayerPrefs.HasKey("volume")) {
			volume = PlayerPrefs.GetFloat ("volume");
		} else {
			PlayerPrefs.SetFloat ("volume", 1f);
			volume = 1f;
		}
		if (PlayerPrefs.HasKey("offset")) {
			offset = PlayerPrefs.GetFloat ("offset");
		} else {
			PlayerPrefs.SetFloat ("offset", 0f);
			offset = 0f;//0 for android
		}

		if (PlayerPrefs.HasKey("flash")) {
			if (PlayerPrefs.GetInt ("flash") == 0) {
				flash = false;
			} else {
				flash = true;
			}
		} else {
			PlayerPrefs.SetInt ("flash", 1);
			flash = true;
		}
		if (PlayerPrefs.HasKey("particles")) {
			if (PlayerPrefs.GetInt ("particles") == 0) {
				particles = false;
			} else {
				particles = true;
			}
		} else {
			PlayerPrefs.SetInt ("particles", 1);
			particles = true;
		}
		if (PlayerPrefs.HasKey("lighting")) {
			if (PlayerPrefs.GetInt ("lighting") == 0) {
				lighting = false;
			} else {
				lighting = true;
			}
		} else {
			PlayerPrefs.SetInt ("lighting", 0);
			lighting = false;
		}

		if (control == null) {
			DontDestroyOnLoad (this.gameObject);
			control = this;
		} else if (control != this) {
			Destroy (this.gameObject);
		}
	}

	public bool SaveScore(float accuracy, float score){
		if (score > PlayerPrefs.GetFloat (song.ToString () + "s")) {
			PlayerPrefs.SetFloat (song.ToString () + "a", accuracy);
			PlayerPrefs.SetFloat (song.ToString () + "s", score);
			return true;
		} else {
			return false;
		}
	}

	public void ClearScores(){
		int counter = 1;
		while (counter <= songAmount) {
			PlayerPrefs.DeleteKey (counter.ToString () + "a");
			PlayerPrefs.DeleteKey (counter.ToString () + "s");
			counter++;
		}
	}
}
