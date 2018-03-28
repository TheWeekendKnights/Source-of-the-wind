using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class MovingObject : MonoBehaviour {


	public int MovePoints;
	public int hp;
	public float Strength; //力
	public float Dexterity; // 技
	public float Intelligence; //智
	public float Mentality; //信
	public GameObject moveRangeTile;
	public GameObject attackRangeTile;
	protected Transform rangeHolder;
	private float FarPosValue = 1000f;
	public int speed = 4;


	protected List<Vector3> bestPath = new List<Vector3> ();
	protected int[] bestPathArray;
	protected int[,] PathArray;
	protected int[] dis;
	protected int movingToNum;

	public Vector3 ObjGridVec; //网格上的位置

	protected List<Vector3> MoveRange = new List<Vector3> ();

	private void setPathArrayByPos(Vector3 pos, int i){
		int index = MoveRange.IndexOf(pos);
		if(index != -1){
			PathArray[i,index] = 1;
		}
	}

	private void initPathArray(int Count){
		PathArray = new int[Count,Count];
		for(int i = 0 ; i < Count;i++){
			for (int i2 = 0; i2 < Count; i2++) {
				PathArray[i, i2] = 100000;
			}
		}
	}

	private void initBestPathArray(int Count){
		bestPathArray = new int[MoveRange.Count];

		for (int i = 0; i < MoveRange.Count; i++) {
			bestPathArray [i] = 0;
		}
	}

	protected void setMoveRange(int x, int y, int MovePoints){

		Vector3 newMovePos;
		MoveRange.Clear ();

		MoveRange.Add(new Vector3 (x, y, -0f));

		//print (BoardManager.instance.floorMoveableArray [8, 16]);

		for (int i = 0; i <= MovePoints; i++) {
			if (i == 0) {
				for (int i2 = -MovePoints; i2 <= MovePoints; i2++) {
					if (x + i2 < BoardManager.instance.columns && y + i < BoardManager.instance.rows) {
						if (BoardManager.instance.floorMoveableArray [x + i2, y + i] != 0) {
							newMovePos = new Vector3 (x + i2, y + i, 0f);
							MoveRange.Add (newMovePos);
						}
					}
				
				}
			}else {
				for (int i2 = -MovePoints+i; i2 <= MovePoints-i; i2++) {
					if (x + i2 < BoardManager.instance.columns && y + i < BoardManager.instance.rows) {
						if (BoardManager.instance.floorMoveableArray [x + i2, y + i] != 0) {
							newMovePos = new Vector3 (x + i2, y + i, -0f);
							MoveRange.Add (newMovePos);
						}
					}

					if (x + i2 < BoardManager.instance.columns && y - i > -1) {
						if (BoardManager.instance.floorMoveableArray [x + i2, y - i] != 0) {
							newMovePos = new Vector3 (x + i2, y - i, -0f);
							MoveRange.Add (newMovePos);
						}
					}

				}
			}
		}

		initPathArray(MoveRange.Count);
		initBestPathArray (MoveRange.Count);

		for (int i = 0; i < MoveRange.Count; i++) {
			PathArray [i,i] = 0;
			Vector3 currentPos = MoveRange [i];
			Vector3 leftPos = new Vector3 (currentPos.x - 1, currentPos.y, currentPos.z);
			Vector3 rightPos = new Vector3 (currentPos.x + 1, currentPos.y, currentPos.z);
			Vector3 topPos = new Vector3 (currentPos.x, currentPos.y + 1, currentPos.z);
			Vector3 downPos = new Vector3 (currentPos.x, currentPos.y - 1, currentPos.z);
			setPathArrayByPos (leftPos, i);
			setPathArrayByPos (rightPos, i);
			setPathArrayByPos (topPos, i);
			setPathArrayByPos (downPos, i);
		}

		dis = new int[MoveRange.Count];
		for (int i = 0; i < MoveRange.Count; i++) {
			dis [i] = PathArray [0, i];
		}

		bool[] doneArray = new bool[MoveRange.Count];
		doneArray [0] = true;
		for (int i = 1; i < MoveRange.Count; i++) {
			doneArray [i] = false;
		}

		for(int i = 0 ; i< MoveRange.Count-1;i++){
			int bestIndex = 0;
			int nerestPathLength = 100000;

			for (int i2 = 0; i2 < MoveRange.Count; i2++) {
				if (dis [i2] <= nerestPathLength && !doneArray [i2]) {
					bestIndex = i2;
					nerestPathLength = dis [i2];
				}
			}

			doneArray [bestIndex] = true;
			for (int i2 = 0; i2 < MoveRange.Count; i2++) {
				int newPathLength = PathArray [bestIndex, i2] + nerestPathLength;
				if (newPathLength < dis [i2]) {
					bestPathArray [i2] = bestIndex;
					dis [i2] = newPathLength;
				}
			}
		}

		for (int i = 0; i < MoveRange.Count; i++) {
			if (dis [i] > MovePoints) {
				MoveRange[i] = new Vector3(x,y,-0f);
			}
		}
		//moveRanges.Remove(moveRanges[0]);

		int originIndex = MoveRange.IndexOf (new Vector3 (x, y, -0f));

		while ( originIndex != -1) {
			//MoveRange.Remove(MoveRange[originIndex]);
			MoveRange[originIndex] = new Vector3(FarPosValue,FarPosValue,0f);
			originIndex = MoveRange.IndexOf (new Vector3 (x, y, -0f));
		}


	}
		

	protected Vector3 RangeTransFromIntoWorldPos(Vector3 pos){
		Vector3 vec = new Vector3();
		int x = Convert.ToInt32 (pos.x);
		int y = Convert.ToInt32 (pos.y);

		vec.x = x+y;
		if (BoardManager.instance.lastTileArray [x, y] == "lowFloor") {
			vec.y = BoardManager.instance.floorYArray [x, y] + 0.25f;
		} else if (BoardManager.instance.lastTileArray [x, y] == "normalFloor") {
			vec.y = BoardManager.instance.floorYArray [x, y] + 0.75f;
		}
		return vec;
	}

	protected void setSingleRange(int x, int y, string Type, GameObject toInstantiate){


		//跳过原地坐标的图
		if (x < FarPosValue) {
			print ("zzz");
			Vector3 vec  =RangeTransFromIntoWorldPos(new Vector3(x,y,0f));

			if (Type == "move") {
				toInstantiate = moveRangeTile;
			}

			toInstantiate.GetComponent<SpriteRenderer> ().sortingOrder = BoardManager.instance.lastTileOrderArray [x, y] + 1;

			GameObject instance =
				Instantiate (toInstantiate, vec, Quaternion.identity) as GameObject;


			//Set the parent of our newly instantiated object instance to boardHolder, this is just organizational to avoid cluttering hierarchy.
			instance.transform.SetParent (rangeHolder);
		} 
			



	}

	protected void RendeMoveRange(List<Vector3> MoveRange){
		rangeHolder = new GameObject ("moveRange").transform;
		foreach (Vector3 pos in MoveRange) {
			setSingleRange (Convert.ToInt32(pos.x),Convert.ToInt32(pos.y), "move", moveRangeTile);
		}
	}

	protected void move(Vector3 newPos){
		
		float step = speed * Time.deltaTime;
		this.transform.position = Vector3.MoveTowards (this.transform.position, MovingObjTransFromIntoWorldPos(newPos), step);
	}


	protected Vector3 MovingObjTransFromIntoWorldPos(Vector3 newPos){


		Vector3 realPos = new Vector3();

		int x = Convert.ToInt32 (newPos.x);
		int y = Convert.ToInt32 (newPos.y);
		realPos.x = x + y;
		if (BoardManager.instance.lastTileArray [x,y ] == "lowFloor") {
			realPos.y = BoardManager.instance.floorYArray [x, y] +1f;
		} else if (BoardManager.instance.lastTileArray [x, y] == "normalFloor"){
			realPos.y = BoardManager.instance.floorYArray [x, y] +1.5f;
		}
			
		return realPos;

	}

	protected Vector3 MovingObjTransFromIntoGridPos(Vector3 Pos){
		Vector3 newPos = new Vector3();

		foreach(Vector3 vec in MoveRange){
			if (vec.x != FarPosValue) {
				if (RangeTransFromIntoWorldPos (vec) == Pos) {
					newPos = vec;
				}
			}
		}

		return newPos;
	}

	protected void setBestPath(Vector3 newPos){

		bestPath = new List<Vector3> ();

		int targetIndex = MoveRange.IndexOf (newPos);
		bestPath.Add (newPos);
		while(bestPathArray[targetIndex]!=0){
			bestPath.Add (MoveRange [bestPathArray [targetIndex]]);
			targetIndex = bestPathArray [targetIndex];
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
