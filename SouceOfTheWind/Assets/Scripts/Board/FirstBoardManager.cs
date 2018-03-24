using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FirstBoardManager : BoardManager {

	public int columns = 25; 										//Number of columns in our game board.
	public int rows = 20;											//Number of rows in our game board.



	void Awake(){
		floorYArray = new float[25, 20];
		lastTileArray = new string[25, 20];
		floorHightArray = new int[25, 20];
		lastTileOrderArray = new int[25, 20];
		lastTileSortArray = new string[25, 20];

		playerPosArray = new int[1, 2]{ {8, 17} };

		base.Awake ();
	}

	// Use this for initialization
	void Start () {
		
	}




	int layerOneRender(int LayerOrder,Transform boardHolder, int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = sandfloorTiles [1];

				//渲染水
				if (( (y == 0 || y==1) && x >= 5 && x <= 12) ||
					( y==2 && x>=4 && x<= 11) ||
					( y==3 && x>=4 && x<=9) ||
					( y==4 && x>=4 && x<=8) ||
					( y>4 && y<= 9 && x<= 11-y) ||
					( y==10 && x<=2) ||
					( y>=11 && y<=12 && x<=1)) {
					toInstantiate = waterfloorTiles [0];
				}
				//渲染植被土
				else if(
					((y == 0 || y==1) &&(x<=4 || x>=13)) ||
					( y ==2 &&( x<=3 || x>=12)) ||
					( y ==3 &&(x<=3 || x>=10)) ||
					( y ==4 && (x<=3 || x>=9)) ||
					( y ==5 && x>11-y)){
					toInstantiate = grassfloorTiles [4];
				}

				//渲染洞
				else if (
					(y == 11 && x>= 13 && x<=14) ||
					(y == 12 && x>= 10 && x<=14) ||
					(y == 13 && x>=9 && x<=15) ||
					(y == 14 && x>=9 && x<=14) ||
					((y == 15||y==16) && x>=8 && x<=14)
				){
					toInstantiate = lowfloorTiles [0];
				}
						

				else{
					toInstantiate = sandfloorTiles [1];
				}

				toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "underFloor";

				LayerOrder = setFloor (toInstantiate, LayerOrder,x ,y, true);

			}


		return LayerOrder;
	}



	int layerTwoRender(int LayerOrder,int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = sandfloorTiles [1];

				//渲染水
				if (( (y == 0 || y==1) && x >= 5 && x <= 12) ||
					( y==2 && x>=4 && x<= 11) ||
					( y==3 && x>=4 && x<=9) ||
					( y==4 && x>=4 && x<=8) ||
					( y>4 && y<= 9 && x<= 11-y) ||
					( y==10 && x<=2) ||
					( y>=11 && y<=12 && x<=1)) {
					toInstantiate = null;
				}

				//渲染植被土
				else if (
					( y == 0  && (x <= 4 || x >= 13)) ||
					( y == 1 && ( x<=3  || x>=13))	||
					(y == 2 && ((x <= 2 && x>=1) || x > 14)) ||
					(y == 3 &&  x > 14) ||
					(y == 4 && x > 15) ||
						(y == 5 && x >15)) {
					toInstantiate = lowfloorTiles [1];
				}

				//渲染洞
				else if (
					(y == 11 && x>= 13 && x<=14) ||
					(y == 12 && x>= 10 && x<=14) ||
					(y == 13 && x>=9 && x<=15) ||
					(y == 14 && x>=9 && x<=14) ||
					(y == 15 && x>=8 && x<=14)
				) {
					toInstantiate = null;
				}

				else if( ( y == 2 && x ==0) ||
					( y==3 && x<=2)||
					(y == 4 && x <=3)){
					toInstantiate = stonefloorTiles[0];
				}

				else if (y==16 && x>=8 && x<=14){
					toInstantiate = sandfloorTiles [2];
				}

				else {
					toInstantiate = lowfloorTiles [2];
				}


				if (toInstantiate != null) {
					toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "floor";

					LayerOrder = setFloor (toInstantiate, LayerOrder, x, y, false);
				}
			}


		layerTwoCoverRender (LayerOrder,y);
		return LayerOrder;
	}
		

	int layerTwoCoverRender(int LayerOrder,int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = null;

				//渲染完全绿草
				if (( y == 0  && (x <= 4 || x >= 13)) ||
					( y == 1 && ( x<=3 || x>=13))	||
					(y == 2 && ((x <= 2 && x>0) || x > 14)) ||
					(y == 3 &&  x > 14) ||
					(y == 4 &&  x > 15) ||
					(y == 5 && x >15) ||
					(y == 6 && (x>=18 && x<=20))
				) {
					toInstantiate = CoverfloorTiles [0];
				}

				//渲染浓绿草

				else if((y==1 && x==4) ||
					( y == 2 &&(x == 3 || (x>=12 && x<=13))) ||
					( y == 3 && x==3) ||
					( y == 6 && x>=20)
				){
					toInstantiate = CoverfloorTiles [2];
				}

				if (toInstantiate != null) {
					toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "floor";

					LayerOrder = setCover (toInstantiate, LayerOrder, x, y);
				}


			}

		return LayerOrder;
	}


	int layerThreeRender(int LayerOrder,int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = null;

				//渲染正常石头块
				if(( y == 3 && x == 1) ||
					( y == 4 && x<=2) ||
					( y==17 &&(x>=10 && x<=20 && x!=14)) ||
					( y == 18 &&( x>=10 && x<=21)) ||
					( y == 19 && x>=10)
				){
					toInstantiate = stonefloorTiles[0];
				}

				//渲染矮石头块
				else if ((y == 16 && (x>=10 && x<=17) ||
					( y == 17 &&(x==9 || x==14 || x==21 ||x==22)) ||
					( y == 18 && x>=21)
				)
				){
					toInstantiate = lowfloorTiles [0];
				}

				if (toInstantiate != null) {
					toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "higherFloor";

					LayerOrder = setFloor (toInstantiate, LayerOrder, x, y, false);
				}
			}


		return LayerOrder;
	}

	int layerFourRender(int LayerOrder,int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = null;

				//渲染正常石头块
				if((y==4 && x==1) ||
					(y==18 &&( x>=11 && x<=20)) ||
					(y == 19 &&( x>=10 && x<=23))
				
				){
					toInstantiate = stonefloorTiles[0];
				}

				else if ( y== 18 && x==22){
					toInstantiate = lowfloorTiles [0];
				}

				if (toInstantiate != null) {
					toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "higherFloor";

					LayerOrder = setFloor (toInstantiate, LayerOrder, x, y, false);
				}

			}

		return LayerOrder;
	}

	int layerFiveRender(int LayerOrder,int y){
		
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for (int x = 0; x < columns; x++) {
				GameObject toInstantiate = null;

				if((y ==18 && (x>=13 && x<=18)) ||
					(y== 19 && (x>=11 && x<=19 && x!=15))
				){
					toInstantiate = stonefloorTiles[0];
				}

				if (toInstantiate != null) {
					toInstantiate.GetComponent<SpriteRenderer> ().sortingLayerName = "higherFloor";

					LayerOrder = setFloor (toInstantiate, LayerOrder, x, y, false);
				}
				

			}

		return LayerOrder;
	}


	void BoardRender(Transform boardHolder){

		int LayerOrder = 0;

		for (int y = rows - 1; y >= 0; y--) {
			for (int layerIndex = 0; layerIndex < 5; layerIndex++) {
				//Loop along x axis, starting from -1 (to fill corner) with floor or outerwall edge tiles.
				if (layerIndex == 0) {
					LayerOrder = layerOneRender (LayerOrder, boardHolder,y);
				} else if (layerIndex == 1) {
					LayerOrder = layerTwoRender (LayerOrder,y);
				} else if (layerIndex == 2) {
					LayerOrder = layerThreeRender (LayerOrder,y);
				} else if (layerIndex == 3) {
					LayerOrder = layerFourRender (LayerOrder,y);
				} else if (layerIndex == 4) {
					LayerOrder = layerFiveRender (LayerOrder,y);
				}
			}
		}

	}

	protected override void PlayersRender(Transform playersHolder){
		//print (playerPosArray [0,0]);
		for(int i = 0; i<Players.Length; i++) {
			rendePlayer (playerPosArray [i, 0], playerPosArray [i, 1], Players[i],playersHolder);
		}
	}


	void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
		playersHolder = new GameObject ("Players").transform;
		BoardRender (boardHolder);
		//print (floorHightArray [1, 14]);

		PlayersRender(playersHolder);

	}

	public void SetupScene (){
		BoardSetup ();
	}

	// Update is called once per frame
	void Update () {
		
	}
}
