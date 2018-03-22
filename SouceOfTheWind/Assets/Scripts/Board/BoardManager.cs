using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class BoardManager : MonoBehaviour {
	public float[,] floorYArray;
	public string[,] lastTileArray;
	public int[,] floorHightArray;

	public GameObject[] grassfloorTiles;
	public GameObject[] waterfloorTiles;
	public GameObject[] lowfloorTiles;
	public GameObject[] sandfloorTiles;
	public GameObject[] stonefloorTiles; 
	public GameObject[] CoverfloorTiles; 

	protected Transform boardHolder;
	protected Dictionary<string, string> HeightDic = new Dictionary<string, string>();



	public void updateFloorYArray(int x, int y, float high){
		floorYArray [x, y] = high;
	}

	public void updateLastTileArray(int x, int y, string lastTile){
		lastTileArray [x, y] = lastTile;
	}

	public void updateFloorHightArray(int x, int y, int hight){
		floorHightArray [x, y] = floorHightArray [x, y] + hight;
	}

	//用来覆盖新的砖块素材
	public Vector3 Overlay(int x, int y, GameObject toInstantiate){
		Vector3 newVec = new Vector3();
		newVec.x = x + y;

		if (toInstantiate.tag != lastTileArray [x, y]) {
			newVec.y = floorYArray [x, y] + 1.0f;

		} else if (toInstantiate.tag == "lowFloor") {
			newVec.y = floorYArray [x, y] + 0.5f;
		} else {
			newVec.y = floorYArray [x, y] + 1.5f;
		}
		floorYArray [x, y] = newVec.y;
		lastTileArray [x, y] = toInstantiate.tag;
		floorHightArray [x, y] += Int32.Parse (HeightDic [toInstantiate.tag]);

		newVec.z = 0f;

		return newVec;
	}

	public Vector3 CoverTile(int x, int y){
		Vector3 newVec = new Vector3();
		newVec.x = x + y;
		if (lastTileArray [x, y] == "lowFloor") {
			newVec.y = floorYArray [x, y] - 0.5f;
		} else {
			newVec.y = floorYArray [x, y];
		}

		return newVec;
	}

	public Vector3 TransFromWorldToISO(Vector3 vec, GameObject toInstantiate){
		Vector3 newVec = new Vector3();

		newVec.x = vec.x + vec.y;
		newVec.y = (vec.y - vec.x) / 2;

		if (toInstantiate.tag == "lowFloor") {
			newVec.y -= 0.5f;
		}

		return newVec;
	}

	protected int setFloor(GameObject toInstantiate, int LayerOrder, int x, int y, bool FirstFloor){
		//Choose a random tile from our array of floor tile prefabs and prepare to instantiate it.
		//GameObject toInstantiate = grassfloorTiles [Random.Range (0, grassfloorTiles.Length)];

		toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder = LayerOrder;
		LayerOrder += 1;
		Vector3 ISOVec = new Vector3 ();
		if (FirstFloor) {
			ISOVec = TransFromWorldToISO (new Vector3 (x, y, 0f), toInstantiate);

			int height;
			height = Int32.Parse (HeightDic [toInstantiate.tag]);
			updateFloorYArray(x,y,TransFromWorldToISO (new Vector3 (x, y, 0f), toInstantiate).y);
			updateLastTileArray (x, y, toInstantiate.tag);
			updateFloorHightArray(x,y,height);

		} else {
			ISOVec = Overlay (x, y, toInstantiate);

		}
		GameObject instance =
			Instantiate (toInstantiate, ISOVec, Quaternion.identity) as GameObject;






		//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
		instance.transform.SetParent (boardHolder);

		return LayerOrder;
	}


	protected int setCover(GameObject toInstantiate,int LayerOrder, int x, int y){

		toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder = LayerOrder;
		LayerOrder += 1;

		Vector3 ISOVec = new Vector3 ();
		ISOVec = CoverTile (x, y);

		GameObject instance =
			Instantiate (toInstantiate, ISOVec, Quaternion.identity) as GameObject;

		instance.transform.SetParent (boardHolder);

		return LayerOrder;
	}


	protected void Awake(){
		HeightDic.Add ("lowFloor", "1");
		HeightDic.Add ("normalFloor", "3");
	}
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
