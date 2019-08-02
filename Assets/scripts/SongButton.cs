using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongButton : MonoBehaviour {

	public float maxScore;

	public SongSelectMaster ssmScript;

	public GameObject accuracyText;
	public GameObject scoreText;
	public GameObject starSprite;

	public Sprite star0;
	public Sprite star1;
	public Sprite star2;
	public Sprite star3;
	public Sprite star3G;

	public short id;

	public void Select(){
		Master.control.song = id;
		ssmScript.GameScene ();
	}

	// Use this for initialization
	void Start () {
		float scorePercent = (PlayerPrefs.GetFloat (id.ToString () + "s") / maxScore) * 100f;
		if (PlayerPrefs.GetFloat(id.ToString() + "s") != 0f) {
			accuracyText.GetComponent<Text> ().text = PlayerPrefs.GetFloat(id.ToString() + "a").ToString("F2") + "%";
			scoreText.GetComponent<Text> ().text = PlayerPrefs.GetFloat(id.ToString() + "s").ToString("N0");
			if (scorePercent >= 100f) {
				starSprite.GetComponent<Image> ().sprite = star3G;
			} else if (scorePercent > 98f) {
				starSprite.GetComponent<Image> ().sprite = star3;
			} else if (scorePercent > 96f) {
				starSprite.GetComponent<Image> ().sprite = star2;
			} else if (scorePercent > 94f) {
				starSprite.GetComponent<Image> ().sprite = star1;
			} else {
				starSprite.GetComponent<Image> ().sprite = star0;
			}
		} else {
			accuracyText.GetComponent<Text> ().text = "Unbeaten";
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
