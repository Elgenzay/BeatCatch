using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour {
	
	public GameObject dataMenuParent;
	public GameObject volumeMenuParent;
	public GameObject offsetMenuParent;
	public GameObject graphicsMenuParent;

	public Button vol;
	public Button sync;
	public Button gfx;
	public Button data;

	void OnEnable(){
		VolumeMenu ();
	}

	private void CloseMenus(){
		dataMenuParent.SetActive (false);
		volumeMenuParent.SetActive (false);
		offsetMenuParent.SetActive (false);
		graphicsMenuParent.SetActive (false);
		vol.interactable = true;
		sync.interactable = true;
		gfx.interactable = true;
		data.interactable = true;
	}
		
	public void VolumeMenu(){
		CloseMenus ();
		volumeMenuParent.SetActive (true);
		vol.interactable = false;
	}

	public void OffsetMenu(){
		CloseMenus ();
		offsetMenuParent.SetActive (true);
		sync.interactable = false;
	}

	public void GraphicsMenu(){
		CloseMenus ();
		graphicsMenuParent.SetActive (true);
		gfx.interactable = false;
	}

	public void DataMenu(){
		CloseMenus ();
		dataMenuParent.SetActive (true);
		data.interactable = false;
	}

}
