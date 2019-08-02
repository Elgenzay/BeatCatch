using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditorBeat : MonoBehaviour {

	public GameObject editorMaster;

	public float time;
	public float place;
	public short type;

	private float dragOffsetX;
	private float dragOffsetY;

	// Use this for initialization
	void Start () {
		UpdateBeat ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown(){
		editorMaster.GetComponent<Editor> ().isDraggingBeat = true;
		editorMaster.GetComponent<Editor> ().selectedBeat = this.gameObject;
		editorMaster.GetComponent<Editor> ().UpdateBeatMenu ();
		dragOffsetX = this.transform.position.x - Camera.main.ScreenToWorldPoint (new Vector3(Input.mousePosition.x, 0f, 0f)).x;
		dragOffsetY = this.transform.position.y - Camera.main.ScreenToWorldPoint (new Vector3(0f, Input.mousePosition.y, 0f)).y;
	}

	void OnMouseUp(){
		editorMaster.GetComponent<Editor> ().isDraggingBeat = false;
	}

	void OnMouseDrag(){
		float xMove = 0f;
		if (Input.GetKey (KeyCode.Q)) {
			xMove = Camera.main.WorldToScreenPoint(new Vector3(this.transform.position.x,0f,0f)).x;
			dragOffsetX = 0f;
		} else {
			xMove = Input.mousePosition.x;
		}
		this.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(xMove, Input.mousePosition.y, -Camera.main.transform.position.z));
		this.transform.position = new Vector3 (this.transform.position.x + dragOffsetX, this.transform.position.y + dragOffsetY, -Camera.main.transform.position.z);
		UpdateBeat ();
		editorMaster.GetComponent<Editor> ().UpdateBeatMenu ();
	}

	public void UpdateBeat(){
		time = (this.transform.position.y - 6.6f) / editorMaster.GetComponent<Editor> ().zoom;
		place = Camera.main.WorldToScreenPoint (new Vector3(this.transform.position.x, 0f, 0f)).x / Screen.width;

		switch (type){
		default:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().standardMaterial;
			break;
		case 2:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().greenMaterial;
			break;
		case 3:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().yellowMaterial;
			break;
		case 4:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().whiteMaterial;
			break;
		case 5:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().purpleMaterial;
			break;
		case 6:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().blueMaterial;
			break;
		case 7:
			this.GetComponent<Renderer> ().material = editorMaster.GetComponent<Editor> ().orangeMaterial;
			break;
		}
	}
}
