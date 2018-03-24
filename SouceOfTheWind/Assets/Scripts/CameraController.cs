using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	// Use this for initialization

	private float LongitudinalSpeed = 0.4f;
	private float LandscapeSpeed = 0.4f;

	void Start () {
		Resolution[] resolutions = Screen.resolutions;
	}
	
	// Update is called once per frame
	void Update () {


		if (Input.GetAxis("Mouse ScrollWheel") <0 || Input.GetKeyDown(KeyCode.A))  {  
			if (Camera.main.orthographicSize <= 11) {
				Camera.main.orthographicSize += 0.4f;

				if (Camera.main.ScreenToWorldPoint (new Vector3 (0f, Screen.height, 0f)).y >= 12) {
					Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y - 0.4f, Camera.main.transform.position.z);
				}

				if (Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0, 0f)).y <=-14) {
					Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + 0.4f, Camera.main.transform.position.z);
				}

				if (Camera.main.ScreenToWorldPoint (new Vector3 (Screen.width, 0f, 0f)).x >= 46) {
					Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x - 0.4f, Camera.main.transform.position.y, Camera.main.transform.position.z);
				}

				if (Camera.main.ScreenToWorldPoint (new Vector3 (0f, 0f, 0f)).x <= -4) {
					Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x + 0.4f, Camera.main.transform.position.y, Camera.main.transform.position.z);
				}


			}
		} 

		if (Input.GetAxis ("Mouse ScrollWheel") > 0) {
			if (Camera.main.orthographicSize >= 3) {
				Camera.main.orthographicSize -= 0.4f;
			}
		} 

		//print (Camera.main.ScreenToWorldPoint (new Vector3(0f,0f,0f)).y);

		//print (Input.mousePosition.y);
		if (Input.mousePosition.y / Screen.height >= 0.95 && Camera.main.ScreenToWorldPoint (new Vector3(0f,Screen.height,0f)).y <=12) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y + LongitudinalSpeed, Camera.main.transform.position.z);
		}

		if (Input.mousePosition.y / Screen.height <= 0.05 && Camera.main.ScreenToWorldPoint (new Vector3(0f,0f,0f)).y >= -14) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x, Camera.main.transform.position.y - LongitudinalSpeed, Camera.main.transform.position.z);
		}

		if (Input.mousePosition.x / Screen.width >= 0.95 && Camera.main.ScreenToWorldPoint (new Vector3(Screen.width,0f,0f)).x <= 46) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x +LandscapeSpeed, Camera.main.transform.position.y, Camera.main.transform.position.z);
		}

		if (Input.mousePosition.x / Screen.width <= 0.05 && Camera.main.ScreenToWorldPoint (new Vector3(0f,0f,0f)).x >= -4) {
			Camera.main.transform.position = new Vector3 (Camera.main.transform.position.x - LandscapeSpeed, Camera.main.transform.position.y , Camera.main.transform.position.z);
		}
			
	}
}
