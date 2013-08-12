using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Map : MonoBehaviour {
	
	private Square[,] squares = null;
	
	private int sizeX;
	private int sizeY;
	
	private float squareWidth;
	private float squareHeight;
	
	private float mapWidth;
	private float mapHeight;
	
	private bool mapBuilded = false;
	
	private GameObject mapGameObject;
	
	public GameObject ObstaclePrefab;
	
	public Map (){
	}
	
	public Map (int sizeX, int sizeY, float squareWidth, float squareHeight, GameObject mapGameObject){
		this.sizeX = sizeX;
		this.sizeY = sizeY;
		this.squareWidth = squareWidth;
		this.squareHeight = squareHeight;
		this.mapGameObject = mapGameObject;
	}
	
	public bool buildMap(){
		mapBuilded = true;
		
		squares = new Square[sizeX, sizeY];
		
		if( sizeX < 1 ){
			mapBuilded = false;	
		}
		
		if( sizeY < 1 ){
			mapBuilded = false;	
		}
		
		if( squareWidth < 1 ){
			mapBuilded = false;	
		}
		
		if( squareHeight < 1 ){
			mapBuilded = false;	
		}
			
		if( mapBuilded ){
			
			mapWidth  = sizeX * squareWidth;
			mapHeight = sizeY * squareHeight;
			
			clearSquares();
		}
		
		return mapBuilded;	
	}

	public void clearSquares(){
		int x, y;
		foreach (Vector2 point in AllSquares()){
			x = (int) point.x;
			y = (int) point.y;
            squares[x, y] = new Square(x, y, new Vector2( ( x * squareWidth ) + (squareWidth / 2), (y * squareHeight * -1) + (squareHeight / 2 * -1) ) );
    	}		
	}
	
	public void addObstacle( int x, int y ){
		addObstacle( x, y, null );
	}
	
	public void addObstacle( int x, int y, GameObject prefabGameObject ){
		GameObject go = null;
		Square square;
		
		square = squares[x, y];
		if( prefabGameObject != null ){
			go = (GameObject)Instantiate(prefabGameObject);
			go.transform.position = new Vector3( square.Center.x, go.transform.position.y , square.Center.y );
			go.transform.parent = mapGameObject.transform;
		}

		square.ContentCode = SquareContent.Wall;
		square.Obstacle = go;
	}
	
	public void addObstacles( List<Vector2> positions ){
		addObstacle( positions , null );
	}
	
	public void addObstacle( List<Vector2> positions, GameObject prefabGameObject ){
		
		foreach( Vector2 position in positions ){
			addObstacle( (int) position.x, (int) position.y, prefabGameObject );	
		}
		
	}
	
	public void createRandomsObstacles( int numberOfObstacles, GameObject prefabGameObject ){
		for( int i = 0; i < numberOfObstacles; i++ ){
			int x = Random.Range( 0, sizeX );
			int y = Random.Range( 0, sizeY );
			addObstacle( x, y, prefabGameObject );
		}
	}
	
    public Vector2 FindByCode(SquareContent contentIn){

        foreach (Vector2 point in AllSquares()){
            if (getSquare(point.x, point.y).ContentCode == contentIn){
                return new Vector2(point.x, point.y);
            }
        }
        return new Vector2(-1, -1);
    }
	
	public bool SquareOpen(float x, float y){	
		return SquareOpen( getSquare( x, y ) );
    }
	
    public bool SquareOpen(int x, int y){	
		return SquareOpen( getSquare( x, y ) );
    }
	
	public bool SquareOpen(Square square){

        switch (square.ContentCode){
            case SquareContent.Empty:
                return true;
            case SquareContent.Target:
                return true;
            case SquareContent.Start:
                return true;
            case SquareContent.Wall:
            default:
                return false;
        }
    }
	
    public IEnumerable AllSquares(){
        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                yield return new Vector2(x, y);
            }
        }
    }
	
	public Square getSquare( int x, int y ){
		return squares[x, y];
	}
	
	public Square getSquare( float x, float y ){
		return getSquare( (int)x, (int)y );
	}
	
	public Vector2 getCoordinatesByWorldPosition( float x, float y ){
		return getCoordinatesByWorldPosition( new Vector2( x, y ) );
	}
	
	public Vector2 getCoordinatesByWorldPosition( int x, int y ){
		return getCoordinatesByWorldPosition( new Vector2( x, y ) );
	}
	
	public Vector2 getCoordinatesByWorldPosition( Vector3 worldPosition ){
		return getCoordinatesByWorldPosition( new Vector2( worldPosition.x, worldPosition.z ) );
	}
	
	public Vector2 getCoordinatesByWorldPosition( Vector2 worldPosition ){
		Vector2 centerMap;
		float startLeft, startTop;
		float auxLeft, auxTop;
		int x, y;
		
		centerMap = new Vector2( mapGameObject.transform.position.x, mapGameObject.transform.position.z );
		
		startLeft = centerMap.x - ( mapWidth  / 2);
		startTop = centerMap.y + ( mapHeight / 2);
		
		auxLeft = startLeft;
		auxTop  = startTop;
		x = -1;
		y = -1;
		
		while( auxLeft < worldPosition.x ){
			auxLeft += squareWidth;
			x++;
		}
		
		while( auxTop > worldPosition.y ){
			auxTop -= squareHeight;
			y++;
		}
		
		return new Vector2( x, y );
	}
	
	public Square NearestFreeSquare( Vector2 position ){
		return NearestFreeSquare( getSquare( position.x, position.y ) );
	}
	
	public Square NearestFreeSquare( Square square ){
		Square currentSquare = square;
		
		int currentX, currentY;

		int spiralLevel;
		int initialX, initialY;
		int walkCount = 1;
		
		currentX = square.X;
		currentY = square.Y;
		
		initialX = currentX;
		initialY = currentY;
		
		spiralLevel = 0;
		
		// direction mostra para onde o espiral ta indo (0 = direita) (1 = cima) (2 = esquerda) (3 = baixo)
		int direction = 0;
		
		for( int index = 0; currentSquare.ContentCode == SquareContent.Wall; index++ ){
			if( currentX == initialX && currentY == initialY && walkCount == 1 ){
				spiralLevel++;	
				initialY++;
				currentY++;
				direction = 0;
				walkCount = 0;
			}else{
				switch(	direction ){
				case 0:
					currentX++;
					break;
				case 1:
					currentY--;
					break;
				case 2:
					currentX--;
					break;
				case 3:
					currentY++;
					break;
				}
				walkCount++;
				
				if( direction == 0 ){
					if( walkCount == spiralLevel ){
						direction = (direction + 1) % 4;
						walkCount = 0;
					}
					
				}else{
					if( walkCount == spiralLevel * 2 ){
						direction = (direction + 1) % 4;
						walkCount = 0;
					}
				}
			}
			
			if( currentX < 0 || currentY < 0 || currentX >= sizeX || currentY >= sizeY ){
				continue;	
			}
			currentSquare = squares[currentX, currentY];
		}
		
		return currentSquare;
	}
	
	private void printMap(){
		for( var i = 0; i < sizeX; i++ ){
			for( var j = 0; j < sizeY; j++ ){
				Debug.Log ( "x: " + i + " y: "+ j +" = " + squares[ i, j ].ContentCode );
			}
		}
	}

	public Square[,] Squares {
		get {
			return this.squares;
		}
		set {
			squares = value;
		}
	}

	public int SizeX {
		get {
			return this.sizeX;
		}
		set {
			sizeX = value;
		}
	}

	public int SizeY {
		get {
			return this.sizeY;
		}
		set {
			sizeY = value;
		}
	}

	public float SquareWidth {
		get {
			return this.squareWidth;
		}
		set {
			squareWidth = value;
		}
	}

	public float SquareHeight {
		get {
			return this.squareHeight;
		}
		set {
			squareHeight = value;
		}
	}

	public float MapWidth {
		get {
			return this.mapWidth;
		}
		set {
			mapWidth = value;
		}
	}

	public float MapHeight {
		get {
			return this.mapHeight;
		}
		set {
			mapHeight = value;
		}
	}

	public bool MapBuilded {
		get {
			return this.mapBuilded;
		}
		set {
			mapBuilded = value;
		}
	}

	public GameObject MapGameObject {
		get {
			return this.mapGameObject;
		}
		set {
			mapGameObject = value;
		}
	}
	
}
