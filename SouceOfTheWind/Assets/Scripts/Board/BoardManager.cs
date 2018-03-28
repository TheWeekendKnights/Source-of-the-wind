using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public abstract class BoardManager : MonoBehaviour {

	public static BoardManager instance = null;

	public int columns ; 										//Number of columns in our game board.
	public int rows ;											//Number of rows in our game board.
	public float[,] floorYArray;
	public string[,] lastTileArray;
	public int[,] floorHightArray;
	public string[,] lastTileSortArray;
	public int[,] lastTileOrderArray;
	public int[,] floorMoveableArray;
	public int floorHight;

	public GameObject[] grassfloorTiles;
	public GameObject[] waterfloorTiles;
	public GameObject[] lowfloorTiles;
	public GameObject[] sandfloorTiles;
	public GameObject[] stonefloorTiles; 
	public GameObject[] CoverfloorTiles;
	public GameObject[] Players;

	protected int[,] playerPosArray;
	protected Transform boardHolder;
	protected Transform playersHolder;
	protected Dictionary<string, string> HeightDic = new Dictionary<string, string>();



	public void updateFloorYArray(int x, int y, float high){
		floorYArray [x, y] = high;
	}

	public void updateLastTileArray(int x, int y, string lastTile, string lastTileSort, int lastTileOrder){

		lastTileArray [x, y] = lastTile;
		lastTileSortArray [x, y] = lastTileSort;
		lastTileOrderArray [x, y] = lastTileOrder;
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
		updateLastTileArray(x,y,toInstantiate.tag,toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName,toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder);
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
			updateLastTileArray (x, y, toInstantiate.tag, toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName,toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder);
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

	protected virtual void PlayersRender(Transform playersHolder){
		
	}

	public bool isOverlay(int x, int y){		
		//print (lastTileSortArray [x, y + 1]);
		if (x > 0) {
			if (lastTileSortArray [x - 1, y] == "higherFloor" || lastTileSortArray [x, y + 1] == "higherFloor") {
				return true;
			}
		}else if(lastTileSortArray [x, y + 1] == "higherFloor") {
			return true;
		} 
			return false;

	}

	protected void rendePlayer(int x, int y, GameObject player, Transform playersHolder){
		Vector3 newVec = new Vector3();
		newVec.x = x + y;

		if (lastTileArray [x, y] == "lowFloor") {
			newVec.y = floorYArray [x, y] +1f;
		} else if (lastTileArray [x, y] == "normalFloor"){
			newVec.y = floorYArray [x, y] +1.5f;
		}

		player.GetComponent<SpriteRenderer> ().sortingOrder = lastTileOrderArray[x,y] +1;

		if (isOverlay (x, y)) {
			player.GetComponent<SpriteRenderer> ().sortingLayerName = "higherFloor";
		} else {
			player.GetComponent<SpriteRenderer> ().sortingLayerName = "floor";
		}

		GameObject instance =
			Instantiate (player, newVec, Quaternion.identity) as GameObject;

		instance.transform.SetParent (playersHolder);


	}

	protected void initFloorMoveAbleArray(){
		for (int i = 0; i < columns; i++) {
			for (int i2 = 0; i2 < rows; i2++) {
				if (floorHightArray [i, i2] == floorHight) {
					floorMoveableArray [i, i2] = 1;
				}
			}
		}
	}

	protected void updateFloorMoveAbleArray(int x, int y, int MoveAble){
		floorMoveableArray [x, y] = MoveAble;
	}

	protected void Awake(){
		HeightDic.Add ("lowFloor", "1");
		HeightDic.Add ("normalFloor", "3");

		floorYArray = new float[columns, rows];
		lastTileArray = new string[columns, rows];
		floorHightArray = new int[columns, rows];
		lastTileOrderArray = new int[columns, rows];
		lastTileSortArray = new string[columns, rows];
		floorMoveableArray = new int[columns, rows];

		if (instance == null) {
			instance = this;
		} 

	}
		
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
