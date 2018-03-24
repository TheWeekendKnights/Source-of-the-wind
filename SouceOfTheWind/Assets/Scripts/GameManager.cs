using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

	public static GameManager instance = null;
	private FirstBoardManager boardScript;

	// Use this for initialization

	void Awake(){
		
		if (instance == null) {
			instance = this;
		} else if (instance != this) {
			Destroy (gameObject);
		}

		boardScript = GetComponent<FirstBoardManager>();

		InitGame ();
	}



	void InitGame(){
		boardScript.SetupScene ();
	}



	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
