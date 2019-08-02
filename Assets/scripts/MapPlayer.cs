using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class MapPlayer : MonoBehaviour {

	public AudioClip goCart;
	public AudioClip chinaTown;

	public GameObject lightEffect;
	public GameObject lowHpEffect;

	public Paddle paddleScript;

	public GameObject dimBG;
	public GameObject paddle;
	public GameObject leftWall;
	public GameObject rightWall;
	public GameObject skipButton;
	public Button skipButtonScript;

	public GameObject beatTechnical;
	public GameObject beatGreen;
	public GameObject beatYellow;
	public GameObject beatWhite;
	public GameObject beatPurple;
	public GameObject beatBlue;
	public GameObject beatOrange;

	public GameObject winMenuParent;
	public GameObject pbTextObject;
	public Text endScoreText;
	public Text endAccText;

	public GameObject pauseMenuParent;
	public GameObject resumeButton;
	public GameObject scoreTextObject;
	public GameObject accuracyTextObject;

	public bool isPlaying;

	public GameObject mapLoadError;

	public beat[] map;

	public float speedMult;

	public float hp;
	public float score;
	public float maxScore;
	public float audioStartTime;
	public bool isDead;

	public AudioSource songSource;

	private float accuracy;

	private float prevRawTime;

	private enum size{
		normal, small, big
	};
	private size currentSize;

	private Text accText;
	private Text scoreText;
	private Light lowHpLight;
	private Light flashLight;

	private Beat prevBlue;
	private Beat prevBlue2;

	private static float leadingDistance = 3f;

	private float targetSpeedMult;

	private int noteCounter;
	private float deathScreenTimeStamp;

	private bool isCorrectSize;
	private bool isCorrectSpeed;

	private bool skipButtonDisabled;

	private float scaleSpeed;
	private float accelerationSpeed;

	public float songTimeAvg;

	public void Pause(){
		Time.timeScale = 0f;
		songSource.Pause ();
		pauseMenuParent.SetActive (true);
	}

	public void Resume(){
		pauseMenuParent.SetActive (false);
		Time.timeScale = 1f;
		songSource.volume = Master.control.volume;
		songSource.Play ();
	}

	public void Retry(){
		Master.control.SceneInGame();
	}

	public void Back(){
		Master.control.SceneSongSelect ();
	}

	public void UpdateStats(){
		if (maxScore != 0f) {
			accuracy = (score / maxScore) * 100f;
			accText.text = (accuracy).ToString ("F2") + "%";
		} else {
			accText.text = "";
		}
		scoreText.text = score.ToString ("N0");
	}

	public void UpdateHP(){
		if (!isDead) {
			if (hp >= 0f) {
				leftWall.transform.localScale = new Vector3 (1f, hp, 1f);
				rightWall.transform.localScale = new Vector3 (1f, hp, 1f);
				lowHpLight.intensity = (1f - hp) * .25f;
			} else {
				leftWall.transform.localScale = new Vector3 (1f, 0f, 1f);
				rightWall.transform.localScale = new Vector3 (1f, 0f, 1f);
				lowHpLight.intensity = .25f;
				Death ();
			}
		}
	}

	void Start () {
		Camera.main.aspect = 480f / 800f;
		skipButtonScript = skipButton.GetComponent<Button> ();
		accText = accuracyTextObject.GetComponent<Text> ();
		scoreText = scoreTextObject.GetComponent<Text> ();
		lowHpLight = lowHpEffect.GetComponent<Light> ();
		flashLight = lightEffect.GetComponent<Light> ();
		audioStartTime = -1f;
		currentSize = size.normal;
		isCorrectSize = true;
		isCorrectSpeed = true;
		skipButtonDisabled = false;
		deathScreenTimeStamp = 0f;
		isPlaying = false;
		speedMult = 1f;
		hp = 1f;
		noteCounter = 0;
		score = 0f;
		maxScore = 0f;
		speedMult = 1f;
		targetSpeedMult = 1f;
		scaleSpeed = 1f;
		accelerationSpeed = 1f;
		songTimeAvg = 0f;
		LoadMap ();
		UpdateStats ();
	}

	public void PaddleSize (short newSize, float newScaleSpeed){
		isCorrectSize = false;
		scaleSpeed = newScaleSpeed * 3f;
		switch (newSize) {
		case -3:
			currentSize = size.small;
			break;
		case -2:
			currentSize = size.big;
			break;
		case -1:
			currentSize = size.normal;
			break;
		}
	}

	public void SpeedMult(short newSpeedMult, float newAccelerationSpeed){
		isCorrectSpeed = false;
		accelerationSpeed = newAccelerationSpeed * 0.01f;
		switch (newSpeedMult) {
		case -4:
			targetSpeedMult = 0.5f;
			break;
		case -5:
			targetSpeedMult = 1f;
			break;
		case -6:
			targetSpeedMult = 1.5f;
			break;
		}
	}

	void FixedUpdate(){
		UpdateHP ();
		float rawTime = (songSource.timeSamples * (1f / songSource.clip.frequency));
		if (rawTime <= prevRawTime) {
			rawTime = rawTime + (Time.deltaTime * songSource.pitch);
		}
		prevRawTime = rawTime;
		float calcSTA = ((songTimeAvg + (Time.deltaTime * songSource.pitch)) + rawTime) / 2f;
		if (calcSTA <= rawTime) {
			songTimeAvg = calcSTA;
		}
		//songTimeAvg = rawTime;
		if (!isCorrectSize) {
			UpdateSize ();
		}
		if (!isCorrectSpeed) {
			UpdateSpeedMult ();
		}
		if (isPlaying && noteCounter + 1 <= map.Length) {
			if (noteCounter == 0 && songSource.time < map[1].time - 3f && !skipButtonDisabled && map[1].time > 10f) {
				skipButton.SetActive (true);
			} else if (skipButton.activeSelf && skipButtonScript.interactable) {
				skipButtonScript.interactable = false;
				skipButtonDisabled = true;
			}
			while (noteCounter + 1 <= map.Length && songSource.time + (17f / leadingDistance) > map [noteCounter].time) {
				GameObject instantiatedBeat = null;
				switch (map [noteCounter].type) {
				case 1:
					instantiatedBeat = Instantiate (beatTechnical);
					isPlaying = false;
					break;
				case 2:
					instantiatedBeat = Instantiate (beatGreen);
					break;
				case 3:
					instantiatedBeat = Instantiate (beatYellow);
					break;
				case 4:
					instantiatedBeat = Instantiate (beatWhite);
					break;
				case 5:
					instantiatedBeat = Instantiate (beatPurple);
					break;
				case 6:
					instantiatedBeat = Instantiate (beatBlue);
					if (prevBlue == null) {
						prevBlue = instantiatedBeat.GetComponent<Beat> ();
					} else {
						prevBlue.blueEnd = instantiatedBeat.transform;
						prevBlue = null;
						instantiatedBeat.GetComponent<Beat> ().isBlueEnd = true;
					}
					break;
				case 7:
					instantiatedBeat = Instantiate (beatOrange);
					break;
				default:
					instantiatedBeat = Instantiate (beatTechnical);
					break;
				}

				if (instantiatedBeat != null) {
					Beat beatScript = instantiatedBeat.GetComponent<Beat> ();
					beatScript.mapPlayer = this.gameObject;
					beatScript.time = map [noteCounter].time - ((Master.control.offset - 2f) * 0.05f);
					switch (map[noteCounter].type) {
					case 1:
						beatScript.type = 1;
						break;
					case 5:
						beatScript.paddle = paddle;
						break;
					case 7:
						beatScript.place = map [noteCounter].place;
						break;
					default:
						beatScript.type = map[noteCounter].type;
						beatScript.place = map [noteCounter].place;
						break;
					}
					instantiatedBeat.transform.position = new Vector3 ((map[noteCounter].place * 6f) - 3f, 100f, 0f);
				}
				noteCounter++;
			}
		}
		if (isDead && deathScreenTimeStamp > Time.time) {
			if (songSource.pitch > 0f && songSource.pitch - 0.025f >= 0f) {
				songSource.pitch -= 0.025f;
			} else {
				songSource.Pause ();
			}
			if (Camera.main.transform.position.y < 11f) {
				Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + 0.025f, Camera.main.transform.position.z);
			}
		} else if (!isDead && hp < 1) {
			hp += 0.001f;
		}
		if (flashLight.intensity > 0f) {
			flashLight.intensity -= 0.05f;
		}
		if (deathScreenTimeStamp != 0 && deathScreenTimeStamp < Time.time) {
			resumeButton.SetActive (false);
			Pause ();
		}
	}

	private void Death (){
		GameObject[] allBeats =  GameObject.FindGameObjectsWithTag("Beat");

		for (int y = 0; y < allBeats.Length; y++) {
			if (allBeats [y].transform.position.y > 15f) {
				Destroy (allBeats [y]);
			}
		}

		paddleScript.isDead = true;
		isDead = true;
		isPlaying = false;
		deathScreenTimeStamp = Time.time + 2f;
	}

	public void MapFinish(){
		winMenuParent.SetActive(true);
		Time.timeScale = 0f;
		isPlaying = false;
		bool pb = false;
		if (Master.control.SaveScore (accuracy, score)) {
			pb = true;
		}
		accuracyTextObject.SetActive (false);
		scoreTextObject.SetActive (false);
		endScoreText.text = score.ToString ("N0");
		endAccText.text = accuracy.ToString ("F2") + "%";
		if (pb) {
			pbTextObject.SetActive (true);
		}
		//print (maxScore);
	}

	public void SkipIntro(){
		skipButtonScript.interactable = false;
		skipButton.SetActive(false);
		Invoke ("SkipEnable", 0.1f);
		songSource.time = map [1].time - 3f;
		songTimeAvg = songSource.time;
		skipButtonDisabled = true;
	}

	private void SkipEnable(){
		skipButton.SetActive(true);
	}

	public void LoadMap(){
		string mapFileName = "";
		switch (Master.control.song){
		case 1:
			mapFileName = "gocart";
			break;
		case 2:
			mapFileName = "chinatown";
			break;
		}

		string dbPath = "";
		if (Application.platform == RuntimePlatform.Android){
			string oriPath = System.IO.Path.Combine(Application.streamingAssetsPath, mapFileName + ".dat");
			WWW reader = new WWW(oriPath);
			while ( ! reader.isDone) {}

			string realPath = Application.persistentDataPath + "/" + mapFileName;
			System.IO.File.WriteAllBytes(realPath, reader.bytes);

			dbPath = realPath;
		} else {
			dbPath = System.IO.Path.Combine(Application.streamingAssetsPath, mapFileName + ".dat");
		}

		if (File.Exists (dbPath)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (dbPath, FileMode.Open);
			MapData data = (MapData)bf.Deserialize (file);
			map = data.map;

			bf.Serialize (file, data);
			file.Close ();

			switch (Master.control.song) {
			case 1:
				songSource.clip = goCart;
				break;
			case 2:
				songSource.clip = chinaTown;
				break;
			}
			songSource.Play();
			songSource.volume = Master.control.volume;
			if (Master.control.haste) {
				songSource.pitch = 1.4f;
			}
			audioStartTime = Time.time;
			isPlaying = true;
			paddle = Instantiate (paddle);
			if (Master.control.dim) {
				dimBG.SetActive (true);
				paddle.GetComponent<SpriteRenderer> ().enabled = false;
				accuracyTextObject.SetActive (false);
				scoreTextObject.SetActive (false);
			}
			paddleScript = paddle.GetComponent<Paddle> ();
		} else {
			mapLoadError.SetActive (true);
		}
	}

	public void RandFlash(){
		float rand = Random.Range (0.01f,0.075f);
		if (flashLight.intensity < rand) {
			flashLight.intensity = rand;
		}
	}

	public void MediumFlash(){
		if (flashLight.intensity < 0.1f) {
			flashLight.intensity = 0.1f;
		}
	}

	public void BigFlash(){
		if (flashLight.intensity < 0.5f) {
			flashLight.intensity = 0.5f;
		}
	}

	private void UpdateSize(){
		if (currentSize == size.normal && paddle.transform.localScale.x != 100f) {
			if (paddle.transform.localScale.x > 100f) {
				if (paddle.transform.localScale.x - scaleSpeed <= 100f) {
					paddle.transform.localScale = new Vector3 (100f, paddle.transform.localScale.y, paddle.transform.localScale.z);
				} else {
					paddle.transform.localScale = new Vector3 (paddle.transform.localScale.x - scaleSpeed, paddle.transform.localScale.y, paddle.transform.localScale.z);
				}
			} else if (paddle.transform.localScale.x < 100f) {
				if (paddle.transform.localScale.x + scaleSpeed >= 100f) {
					paddle.transform.localScale = new Vector3 (100f, paddle.transform.localScale.y, paddle.transform.localScale.z);
				} else {
					paddle.transform.localScale = new Vector3 (paddle.transform.localScale.x + scaleSpeed, paddle.transform.localScale.y, paddle.transform.localScale.z);
				}
			}
			paddle.transform.localScale = new Vector3 (paddle.transform.localScale.x, paddle.transform.localScale.y, paddle.transform.localScale.z);
		} else if (currentSize == size.small && paddle.transform.localScale.x > 50f) {
			if (paddle.transform.localScale.x - scaleSpeed <= 50f) {
				paddle.transform.localScale = new Vector3 (50f, paddle.transform.localScale.y, paddle.transform.localScale.z);
			} else {
				paddle.transform.localScale = new Vector3 (paddle.transform.localScale.x - scaleSpeed, paddle.transform.localScale.y, paddle.transform.localScale.z);
			}
		} else if (currentSize == size.big && paddle.transform.localScale.x < 150f) {
			if (paddle.transform.localScale.x + scaleSpeed >= 150f) {
				paddle.transform.localScale = new Vector3 (150f, paddle.transform.localScale.y, paddle.transform.localScale.z);
			} else {
				paddle.transform.localScale = new Vector3 (paddle.transform.localScale.x + scaleSpeed, paddle.transform.localScale.y, paddle.transform.localScale.z);
			}
		} else {
			isCorrectSize = true;
		}
	}

	private void UpdateSpeedMult(){
		if (speedMult < targetSpeedMult) {
			if (speedMult + accelerationSpeed >= targetSpeedMult) {
				speedMult = targetSpeedMult;
			} else {
				speedMult += +accelerationSpeed;
			}
		} else if (speedMult > targetSpeedMult) {
			if (speedMult - accelerationSpeed <= targetSpeedMult) {
				speedMult = targetSpeedMult;
			} else {
				speedMult -= accelerationSpeed;
			}
		} else {
			isCorrectSpeed = true;
		}
	}

}