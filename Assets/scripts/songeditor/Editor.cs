using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class Editor : MonoBehaviour {

	public float zoom;

	public GameObject eraser;
	public GameObject savedText;

	public GameObject placingTypeInputField;
	public GameObject timeInputField;
	public GameObject placeInputField;
	public GameObject typeInputField;

	public GameObject bpmInputField;
	public GameObject offsetInputField;
	public GameObject snapPlaceToggle;

	public Material standardMaterial;
	public Material greenMaterial;
	public Material yellowMaterial;
	public Material whiteMaterial;
	public Material purpleMaterial;
	public Material blueMaterial;
	public Material orangeMaterial;

	public GameObject currentTimeText;
	public GameObject currentPlayBackText;
	public GameObject selectedBeat;
	public GameObject editorBeat;
	public GameObject playBackSpeedSlider;
	public GameObject song;

	public bool isDraggingBeat;

	private bool isPlaying;
	private float playBackSpeed;
	private float saveTextTimeStamp;

	private beat[] map;

	public void ChangePlayBack(float newval){
		playBackSpeed = newval;
		song.GetComponent<AudioSource> ().pitch = playBackSpeed;
		currentPlayBackText.GetComponent<Text> ().text = "x " + playBackSpeed.ToString ("F2");
	}

	public void ChangeVolume(float newval){
		song.GetComponent<AudioSource> ().volume = newval;
	}

	public void UpdateBeatMenu(){
		if (selectedBeat != null) {
			timeInputField.GetComponent<InputField> ().text = selectedBeat.GetComponent<EditorBeat> ().time.ToString ();
			placeInputField.GetComponent<InputField> ().text = selectedBeat.GetComponent<EditorBeat> ().place.ToString ();
			typeInputField.GetComponent<InputField> ().text = selectedBeat.GetComponent<EditorBeat> ().type.ToString ();
		} else {
			timeInputField.GetComponent<InputField> ().text = "0";
			placeInputField.GetComponent<InputField> ().text = "0";
			typeInputField.GetComponent<InputField> ().text = "0";
		}
	}

	public void ListenStart(){
		song.GetComponent<AudioSource> ().time = float.Parse (currentTimeText.GetComponent<Text> ().text) - 0.01f;
		song.GetComponent<AudioSource> ().Play ();
		Invoke ("ListenEnd", 0.05f);
	}
	public void ListenEnd(){
		song.GetComponent<AudioSource> ().Stop ();
		song.GetComponent<AudioSource> ().time = float.Parse (currentTimeText.GetComponent<Text> ().text);
	}

	public void ApplyButton(){
		if (selectedBeat != null) {
			selectedBeat.GetComponent<EditorBeat> ().time = float.Parse (timeInputField.GetComponent<InputField> ().text);
			selectedBeat.GetComponent<EditorBeat> ().place = float.Parse (placeInputField.GetComponent<InputField> ().text);
			selectedBeat.GetComponent<EditorBeat> ().type = short.Parse (typeInputField.GetComponent<InputField> ().text);

			selectedBeat.transform.position = new Vector3 (Camera.main.ScreenToWorldPoint (new Vector3(Screen.width * selectedBeat.GetComponent<EditorBeat> ().place, 0f, 0f)).x, selectedBeat.GetComponent<EditorBeat> ().time * zoom + 6.6f, selectedBeat.transform.position.z);
			selectedBeat.GetComponent<EditorBeat> ().UpdateBeat ();
		}
	}

	public void DeleteButton(){
		Destroy (selectedBeat);
		selectedBeat = null;
		UpdateBeatMenu ();
	}

	public void Play(){
		if (isPlaying) {
			song.GetComponent<AudioSource> ().Stop ();
			isPlaying = false;
		} else if (Camera.main.transform.position.y >= 10f){
			song.GetComponent<AudioSource> ().Play ();
			song.GetComponent<AudioSource> ().pitch = playBackSpeed;
			song.GetComponent<AudioSource> ().time = (Camera.main.transform.position.y - 10f) / zoom;
			isPlaying = true;
		}
	}

	public void SaveMap(){
		GameObject[] allBeats =  GameObject.FindGameObjectsWithTag("Beat");
		map = new beat[allBeats.Length];
		float testTime = 0f;
		int currentBeat = 0;
		while (currentBeat + 1 <= map.Length) {
			for (int y = 0; y < allBeats.Length; y++) {
				if (allBeats [y] != null && allBeats [y].GetComponent<EditorBeat> ().time < testTime) {
					if (allBeats [y].GetComponent<EditorBeat> ().place >= 0f && allBeats [y].GetComponent<EditorBeat> ().place <= 1f) {
						map [currentBeat] = new beat {
							time = allBeats [y].GetComponent<EditorBeat> ().time,
							place = allBeats [y].GetComponent<EditorBeat> ().place,
							type = allBeats [y].GetComponent<EditorBeat> ().type
						};
					} else {
						map [currentBeat] = new beat {
							time = allBeats [y].GetComponent<EditorBeat> ().time,
							place = 0f,
							type = allBeats [y].GetComponent<EditorBeat> ().type
						};
						print ("Error: Beat placement out of range. Time: " + allBeats [y].GetComponent<EditorBeat> ().time.ToString ());
					}
					allBeats [y] = null;
					currentBeat++;
				}
			}
			testTime += 0.01f;

		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (Application.persistentDataPath + "/editor/map.dat", FileMode.Open);

		MapData data = new MapData ();
		data.map = map;

		bf.Serialize (file, data);
		file.Close ();
		savedText.SetActive (true);
		saveTextTimeStamp = Time.time + 2f;
	}

	public void LoadMap(){
		if (File.Exists (Application.persistentDataPath + "/editor/map.dat")) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (Application.persistentDataPath + "/editor/map.dat", FileMode.Open);
			MapData data = (MapData)bf.Deserialize (file);
			map = data.map;

			bf.Serialize (file, data);
			file.Close ();

		} else {
			Debug.Log ("Error loading map file");
		}

		for (int x = 0; x < map.Length; x++) {
			var instantiatedBeat = Instantiate (editorBeat);
			instantiatedBeat.GetComponent<EditorBeat> ().editorMaster = this.gameObject;
			instantiatedBeat.GetComponent<EditorBeat> ().time = map [x].time;
			instantiatedBeat.GetComponent<EditorBeat> ().place = map [x].place;
			instantiatedBeat.GetComponent<EditorBeat> ().type = map [x].type;
			instantiatedBeat.transform.position = new Vector3 (Camera.main.ScreenToWorldPoint (new Vector3(Screen.width * instantiatedBeat.GetComponent<EditorBeat> ().place, 0f, 0f)).x, instantiatedBeat.GetComponent<EditorBeat> ().time * zoom + 6.6f, instantiatedBeat.transform.position.z);
			instantiatedBeat.GetComponent<EditorBeat> ().UpdateBeat();
		}
	}

	void Start () {
		Screen.SetResolution(480, 800, false);
		saveTextTimeStamp = 0f;
		playBackSpeed = 1f;
		isDraggingBeat = false;
		isPlaying = false;
		LoadMap ();
	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.Mouse0) && Input.mousePosition.y > 150) {
			RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
			if(hitInfo == false)
			{
				selectedBeat = null;
				UpdateBeatMenu ();
			}
		}
		if (Input.GetKeyDown(KeyCode.Mouse1)) {
			var instantiatedBeat = Instantiate (editorBeat);
			float prevSelBeatTime = 0f;
			if (selectedBeat != null) {
				prevSelBeatTime = selectedBeat.GetComponent<EditorBeat> ().time;
			}
			selectedBeat = instantiatedBeat;
			instantiatedBeat.GetComponent<EditorBeat> ().editorMaster = this.gameObject;
			instantiatedBeat.transform.position = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
			instantiatedBeat.GetComponent<EditorBeat>().type = short.Parse(placingTypeInputField.GetComponent<InputField>().text);
			instantiatedBeat.GetComponent<EditorBeat> ().UpdateBeat ();
			UpdateBeatMenu ();
			if (Input.GetKey (KeyCode.Q) || snapPlaceToggle.GetComponent<Toggle>().isOn) {
				placeInputField.GetComponent<InputField>().text = ((Mathf.Round((Camera.main.WorldToScreenPoint (new Vector3(instantiatedBeat.transform.position.x, 0f, 0f)).x / Screen.width) * 10f)) * 0.1f).ToString();
				ApplyButton ();
			}
			if (Input.GetKey (KeyCode.R) && prevSelBeatTime != 0f) {
				timeInputField.GetComponent<InputField> ().text = prevSelBeatTime.ToString ();
				ApplyButton ();
			}
			if (Input.GetKey (KeyCode.F)) {
				//timeInputField.GetComponent<InputField> ().text = (RoundUp((instantiatedBeat.transform.position.y - 6.6f) / zoom,1f / (float.Parse(bpmInputField.GetComponent<InputField>().text) / 60f ) )).ToString();
				timeInputField.GetComponent<InputField> ().text = (RoundUp((instantiatedBeat.transform.position.y - 6.6f) / zoom, 
					1f / (float.Parse(bpmInputField.GetComponent<InputField>().text) / 60f )) + ( (1f / (float.Parse(bpmInputField.GetComponent<InputField>().text) / 60f )) * float.Parse(offsetInputField.GetComponent<InputField>().text)
				)
				).ToString();
				ApplyButton ();
			}
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			Instantiate (eraser).transform.position = Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, Input.mousePosition.y, -Camera.main.transform.position.z));
		}
		if (Input.GetKey (KeyCode.W)) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + 0.1f, Camera.main.transform.position.z);
			currentTimeText.GetComponent<Text>().text = ((Camera.main.transform.position.y - 10f) / zoom).ToString("F2");
		}
		if (Input.GetKey (KeyCode.S)) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y - 0.1f, Camera.main.transform.position.z);
			currentTimeText.GetComponent<Text>().text = ((Camera.main.transform.position.y - 10f) / zoom).ToString("F2");
		}
		if (Input.GetAxis("Mouse ScrollWheel") != 0f) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * 4f, Camera.main.transform.position.z);
			currentTimeText.GetComponent<Text>().text = ((Camera.main.transform.position.y - 10f) / zoom).ToString("F2");
		}
		if (saveTextTimeStamp != 0f && saveTextTimeStamp < Time.time) {
			savedText.SetActive (false);
			saveTextTimeStamp = 0f;
		}
	}

	private float RoundUp(float numToRound, float multiple)
	{
		if (multiple == 0)
			return numToRound;

		float remainder = numToRound % multiple;
		if (remainder == 0)
			return numToRound;

		return numToRound + multiple - remainder;
	}//thanks, Mark Ransom. i'm lazy

	void FixedUpdate(){
		if (isPlaying) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + playBackSpeed * Time.deltaTime * zoom, Camera.main.transform.position.z);
			currentTimeText.GetComponent<Text>().text = ((Camera.main.transform.position.y - 10f) / zoom).ToString("F2");
			if (float.Parse(currentTimeText.GetComponent<Text> ().text) < 0.0001 && float.Parse(currentTimeText.GetComponent<Text> ().text) > -0.0001) {
				currentTimeText.GetComponent<Text> ().text = "0";
			}
		}
		if (Input.GetKey (KeyCode.Mouse0) && !isDraggingBeat && !isPlaying) {
			Camera.main.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y - Input.GetAxis ("Mouse Y"), Camera.main.transform.position.z);
			currentTimeText.GetComponent<Text>().text = ((Camera.main.transform.position.y - 10f) / zoom).ToString("F2");
			if (float.Parse(currentTimeText.GetComponent<Text> ().text) < 0.0001 && float.Parse(currentTimeText.GetComponent<Text> ().text) > -0.0001) {
				currentTimeText.GetComponent<Text> ().text = "0";
			}
		}
	}
}

[System.Serializable]
class MapData{
	public beat[] map;
}