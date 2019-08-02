using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModMenu : MonoBehaviour {

	public Text multText;

	public Image hasteButton;
	public Image dimButton;

	public Sprite hastePressed;
	public Sprite dimPressed;

	private Sprite hasteUnpressed;
	private Sprite dimUnpressed;

	void Start(){
		hasteUnpressed = hasteButton.GetComponent<Image> ().sprite;
		dimUnpressed = dimButton.GetComponent<Image> ().sprite;

		if (Master.control.haste) {
			hasteButton.sprite = hastePressed;
		}
		if (Master.control.dim) {
			dimButton.sprite = dimPressed;
		}
		UpdateMult ();
	}

	private void UpdateMult(){
		float mult = 1f;
		if (Master.control.haste) {
			mult *= 1.04f;
		}
		if (Master.control.dim) {
			mult *= 1.02f;
		}
		Master.control.multiplier = mult;
		multText.text = "x" + mult.ToString ("F2");
	}

	public void Haste(){
		if (Master.control.haste) {
			Master.control.haste = false;
			hasteButton.sprite = hasteUnpressed;
		} else {
			Master.control.haste = true;
			hasteButton.sprite = hastePressed;
		}
		UpdateMult ();
	}

	public void Dim(){
		if (Master.control.dim) {
			Master.control.dim = false;
			dimButton.sprite = dimUnpressed;
		} else {
			Master.control.dim = true;
			dimButton.sprite = dimPressed;
		}
		UpdateMult ();
	}
}
