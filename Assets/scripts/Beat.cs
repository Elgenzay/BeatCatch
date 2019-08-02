using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Beat : MonoBehaviour {
	public short type;
	public GameObject explosion;
	public GameObject lighting;
	public Material missMat;
	public GameObject childSprite;
	public GameObject orangeTrail;

	[HideInInspector]
	public GameObject mapPlayer;
	public GameObject paddle;

	public Transform blueEnd;
	public bool isBlueEnd = false;

	public float time;
	public float place;

	private float bluePrevTime;

	private float pitch;
	private float clipFrequency;
	private float prevSample;

	private float move;

	private float speed;
	private float worth;

	private Vector3 prevPos;

	private MapPlayer mpscript;

	private SpriteRenderer sr;
	private LineRenderer lr;
	private Material blueMat;

	private bool isMissed = false;
	private bool isTechnical = false;
	private bool isHidden = false;

	private bool blueHasPassed = false;
	private bool blueBeingMissed = false;

	void Start(){
		if (Master.control.lighting && !Master.control.haste && !Master.control.dim && type != 6 && type > 1) {
			lighting.SetActive (true);
		}
		mpscript = mapPlayer.GetComponent<MapPlayer> ();
		clipFrequency = mpscript.songSource.clip.frequency;
		pitch = mpscript.songSource.pitch;
		this.transform.localScale = new Vector3 (15f, 15f, 1f);
		sr = this.GetComponent<SpriteRenderer> ();
		switch (type) {
		//1: Map end
		case 2://green
			speed = 10f;
			worth = 10f;
			break;
		case 3://yellow
			speed = 20f;
			worth = 15f;
			break;
		case 4://white
			speed = 10f;
			worth = 20f;
			break;
		case 5://purple
			speed = 5f;
			worth = 15f;
			break;
		case 6://blue
			speed = 10f;
			worth = 40f;
			lr = this.GetComponent<LineRenderer> ();
			blueMat = this.GetComponent<SpriteRenderer> ().material;
			bluePrevTime = 0f;
			break;
		case 7://orange
			speed = 5f;
			worth = 15f;
			break;
		}
		worth *= Master.control.multiplier;
		if (type <= 1) {
			speed = 10f;
			isTechnical = true;
		}
		this.transform.position = new Vector3 (this.transform.position.x, 6.5f + (time - mpscript.songTimeAvg) * speed , 0f);
		move = speed * mpscript.speedMult;
	}

	void FixedUpdate () {
		if (this.transform.position.y > 15f) {
			move = speed * mpscript.speedMult;
		}//TODO: optimize this
		float sample = mpscript.songSource.timeSamples;
		float smoothTime = mpscript.songTimeAvg;
		float ActualTime = sample * (1f / clipFrequency);
		float newY;
		if (time < (smoothTime + 4f)){
			if (sample != prevSample && !isMissed) {
				//newY = 6.5f + (((time - (smoothTime)) + Time.fixedDeltaTime) * move);
				newY = 6.5f + (((time - (smoothTime)) + Time.fixedDeltaTime) * move);
				prevSample = sample;
			} else {
				newY = this.transform.position.y - (Time.fixedDeltaTime * move * pitch);
			}
			if (Master.control.dim && !isHidden) {
				if (type != 7) {
					if (newY < 12.5f && type > 1) {
						isHidden = true;
						if (childSprite != null) {
							childSprite.SetActive (false);
						} else {
							sr.enabled = false;
						}
					}
				} else {
					if (newY < 9f) {
						isHidden = true;
						sr.enabled = false;
						orangeTrail.GetComponent<ParticleSystem> ().Stop ();
					}
				}
			}
			if (this.transform.position.y < 20f) {
				switch (type) {
				case 5:
					this.transform.position = new Vector3 (-paddle.transform.position.x, this.transform.position.y, this.transform.position.z);
					break;
				case 6:
					if (!isBlueEnd) {
						Vector3 bePos;
						if (blueEnd == null) {
							bePos = new Vector3 (this.transform.position.x, 15f, 0f);
						} else {
							bePos = blueEnd.transform.position;
						}

						bool isHit = false;
						bool justMissed = false;
						if (bePos.y <= 5f) {
							Destroy (blueEnd.gameObject);
							Destroy (this.gameObject);
						}
						if (this.transform.position.y < 6.5f && bePos.y >= 6.5f) {
							blueHasPassed = true;
							RaycastHit2D paddlehit = Physics2D.Linecast (bePos, this.transform.position, 1 << 8);
							if (paddlehit == true) {
								blueBeingMissed = false;
								isHit = true;
								if (Master.control.particles && !Master.control.dim) {
									Instantiate (explosion).transform.position = paddlehit.point;
								}
								lr.material = blueMat;
							} else if (!mpscript.isDead) {
								justMissed = true;
								lr.material = missMat;
							}
						}
						if (!mpscript.isDead && time < ActualTime && bluePrevTime != blueEnd.GetComponent<Beat> ().time) {//TODO: beat script reference
							float calculatedWorth;
							if (blueEnd.GetComponent<Beat> ().time < ActualTime) {
								calculatedWorth = blueEnd.GetComponent<Beat> ().time - bluePrevTime;
								bluePrevTime = blueEnd.GetComponent<Beat> ().time;
							} else if (bluePrevTime == 0f) {
								calculatedWorth = (ActualTime) - time;
							} else {
								calculatedWorth = ((ActualTime) - time) - (bluePrevTime - time);
							}
							calculatedWorth *= worth;
							if (!justMissed) {//hit
								Hit (calculatedWorth);
							} else {//miss
								mpscript.hp -= calculatedWorth * .01f;
								mpscript.maxScore += calculatedWorth;
								mpscript.UpdateStats ();
							}

							if (bluePrevTime != blueEnd.GetComponent<Beat> ().time) {
								bluePrevTime = ActualTime;
							}
						}


						if (bePos.y < 6.5f && !blueBeingMissed) {
							Destroy (blueEnd.gameObject);
							Destroy (this.gameObject);
						}
						Vector3[] points = new Vector3[2];
						RaycastHit2D hit = Physics2D.Linecast (bePos, this.transform.position, 1 << 9);
						if (justMissed && !blueBeingMissed) {
							this.transform.position = new Vector3 (hit.point.x, 6.5f, 0f);
							blueBeingMissed = true;
						}
						if (hit == true && isHit == true) {
							points [0] = hit.point;
						} else {
							points [0] = this.transform.position;
						}
						points [1] = bePos;
						lr.SetPositions (points);


					}
					break;
				case 7:
					prevPos = this.transform.position;
					if (place <= 0.5f) {
						this.transform.position = new Vector3 (Mathf.Sin (((newY - 6.5f) * 0.5f) - (Mathf.PI / 2f) + (Mathf.PI * place)) * 2.5f, this.transform.position.y, this.transform.position.z);
					} else if (place > 0.5f) {
						this.transform.position = new Vector3 (Mathf.Sin (((newY - 6.5f) * 0.5f) - (Mathf.PI / 2f) - (Mathf.PI * place)) * 2.5f, this.transform.position.y, this.transform.position.z);
					}
					break;
				}
			}
			if (!isTechnical && type != 6) {
				if (type == 7 && Physics2D.Raycast (prevPos, prevPos - this.transform.position, Mathf.Sqrt (Mathf.Pow (Mathf.Abs (this.transform.position.x - prevPos.x), 2f) + Mathf.Pow (Mathf.Abs (this.transform.position.y - newY), 2f)), 1 << 8)) {
					this.transform.position = new Vector3 (this.transform.position.x, 6.7f, this.transform.position.z);
					Hit (worth);
				} else if (type != 7 && Physics2D.Raycast (this.transform.position, Vector3.down, this.transform.position.y - newY, 1 << 8)) {
					this.transform.position = new Vector3 (this.transform.position.x, 6.7f, this.transform.position.z);
					Hit (worth);
				} else if (newY < 6f && !isMissed) {
					if (childSprite == null) {
						this.GetComponent<Renderer> ().material = missMat;
					}
					isMissed = true;
					foreach (Transform child in this.transform) {
						if (child.gameObject != childSprite) {
							Destroy (child.gameObject);
						} else {
							childSprite.GetComponent<Renderer> ().material = missMat;
						}
					}
					if (!mpscript.isDead) {
						mpscript.hp -= worth * .01f;
						mpscript.maxScore += worth;
						mpscript.UpdateStats ();
					}
				}
				if (newY < Camera.main.ScreenToWorldPoint (Vector3.zero).y && isMissed) {//TODO: don't use Camera.main
					Destroy (this.gameObject);
				}
			}
			if (!blueHasPassed) {
				this.transform.position = new Vector3 (this.transform.position.x, newY, this.transform.position.z);
			} else {
				this.transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - (speed * Time.fixedDeltaTime), this.transform.position.z);
			}
			if (isTechnical) {
				if (this.transform.position.y < 6.5f) {
					if (type == 1) {
						mpscript.MapFinish ();
					} else if (type == -1 || type == -2 || type == -3) {
						mpscript.PaddleSize (type, place);
					} else if (type == -4 || type == -5 || type == -6) {
						mpscript.SpeedMult (type, place);
					}
					Destroy (this.gameObject);
				}
			}
		}
	}

	void OnCollisionEnter2D(){
		Hit (worth);
	}

	private void Hit(float val){
		if (!mpscript.isDead) {
			mpscript.paddleScript.Flash ();
			if (Master.control.flash) {
				switch (type) {
				case 4:
					mpscript.BigFlash ();
					break;
				case 6:
					mpscript.RandFlash ();
					break;
				default:
					mpscript.MediumFlash ();
					break;
				}
			}
			mpscript.score += val;
			mpscript.maxScore += val;
			if (mpscript.hp + (val * 0.0025f) <= 1f) {
				mpscript.hp += val * 0.0025f;
			} else {
				mpscript.hp = 1f;
			}
			mpscript.UpdateStats ();
			if (type != 6) {
				if (Master.control.particles) {
					Instantiate (explosion).transform.position = new Vector3 (this.transform.position.x, this.transform.position.y - 0.1f, 0f);
				}
				Destroy (this.gameObject);
			}
		}
	}
}
