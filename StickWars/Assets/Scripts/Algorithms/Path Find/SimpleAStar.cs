using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleAStar : PathFinding{

    private bool diagonal = false;
	private bool mapBuilded = false;
	private Vector2[] _movements;
	private Square[,] _squares = null;
	
	private int sizeX;
	private int sizeY;
	
	private float squareWidth;
	private float squareHeight;
    
	public Square[,] getSquares(){
        return _squares;
    }

    public SimpleAStar(){
        InitMovements();
    }
	
	public SimpleAStar(int sizeX, int sizeY, float squareWidth, float squareHeight){
		setMapConfiguration( sizeX, sizeY, squareWidth, squareHeight );
		
        InitMovements();
	}
	
	public void setMapConfiguration(int sizeX, int sizeY, float squareWidth, float squareHeight){
		_squares = new Square[sizeX, sizeY];
		
		this.sizeX = sizeX;
		this.sizeY = sizeY;
		this.squareWidth = squareWidth;
		this.squareHeight = squareHeight;
		
		clearSquares();
		
		mapBuilded = true;
	}

    public void clearSquares(){
		float x, y;
        foreach (Vector2 point in AllSquares()){
			x = point.x;
			y = point.y;
            _squares[(int)x, (int)y] = new Square((int)x, (int)y, new Vector2( x + (squareWidth / 2), (y * -1) + (squareHeight / 2 * -1) ) );
        }
    }

	/*
	public Vector2 getVectorPosition( Vector3 target ){
		Vector2 position = new Vector2(0, 0);
		
		//Debug.Log(target);
		position.x = (int) target.x;
		position.y = (int) target.z * -1;
		//Debug.Log(position);
		
		return position;
	}
	*/
	
	public void setStart(Vector2 target){
		_squares[(int) target.x, (int)target.y].ContentCode = SquareContent.Start;
	}
	
	public void setTarget(Vector2 target){
		_squares[(int) target.x,(int) target.y].ContentCode = SquareContent.Target;
	}
	
	public void setWalls(List<Vector2> walls){		
		foreach( Vector2 vector2 in walls ){
			_squares[(int)vector2.x,(int)vector2.y].ContentCode = SquareContent.Wall;
		}
			
	}
	
	public void clearStartAndTarget(){
		Vector2 point = FindCode(SquareContent.Target);
		
		point = FindCode(SquareContent.Target);
		
		_squares[(int) point.x,(int) point.y].ContentCode = SquareContent.Empty;
		
		point = FindCode(SquareContent.Start);
		
		_squares[(int) point.x,(int) point.y].ContentCode = SquareContent.Empty;
	}

    public void ClearLogic(){

        foreach (Vector2 point in AllSquares()){
            int x = (int) point.x;
            int y = (int) point.y;
            _squares[x, y].DistanceSteps = 10000;
            _squares[x, y].IsPath = false;
        }
    }

	private void printMap(){
		for( var i = 0; i < sizeX; i++ ){
			for( var j = 0; j < sizeY; j++ ){
				Debug.Log ( "x: " + i + " y: "+ j +" = " + _squares[ i, j ].ContentCode );
			}
		}
	}
	
    public bool PathFind( Vector2 start, Vector2 target ){
	
		if( !mapBuilded ){
			return false;
		}
		Debug.Log("1");
		
		_squares[(int) start.x,(int) start.y].ContentCode = SquareContent.Start;
		_squares[(int) target.x,(int) target.y].ContentCode = SquareContent.Target;
        
		Debug.Log("2");
		
//		printMap();

		
		/*
         * 
         * Find path from hero to monster. First, get coordinates
         * of hero.
         * 
         * */
        Vector2 startingPoint = FindCode(SquareContent.Target);
        int heroX = (int) startingPoint.x;
        int heroY = (int) startingPoint.y;
		Debug.Log("startingPoint = "+startingPoint);
		
        if (heroX == -1 || heroY == -1){
            return false;
        }
        /*
         * 
         * Target starts at distance of 0.
         * 
         * */
        _squares[heroX, heroY].DistanceSteps = 0;
		
//        for (int i = 0; i < 10; i++){
		while(true){
            bool madeProgress = false;
			
			Debug.Log("madeProgress = "+madeProgress);
            /*
             * 
             * Look at each square on the board.
             * 
             * */
            foreach (Vector2 mainPoint in AllSquares()){
//				Debug.Log(mainPoint);
                int x = (int) mainPoint.x;
                int y = (int) mainPoint.y;

                if (SquareOpen(x, y)){
//					Debug.Log("OPEN");
                    int passHere = _squares[x, y].DistanceSteps;

                    foreach (Vector2 movePoint in ValidMoves(x, y)){
						//Debug.Log("Valida Move ");
						//Debug.Log(movePoint);
                        int newX = (int) movePoint.x;
                        int newY = (int) movePoint.y;
                        int newPass = passHere + 1;
						
						//Debug.Log("_squares[newX, newY].DistanceSteps = " + _squares[newX, newY].DistanceSteps );
						//Debug.Log(" newPass  = " + newPass );
						
                        if (_squares[newX, newY].DistanceSteps > newPass){
							/*
							Debug.Log("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
							Debug.Log( _squares[newX, newY].DistanceSteps );
							Debug.Log( newPass );
							Debug.Log( mainPoint );
							Debug.Log( movePoint );
							*/
							
                            _squares[newX, newY].DistanceSteps = newPass;
                            madeProgress = true;
                        }
                    }
                }
            }
            if (!madeProgress){
                break;
            }
        }
		
		return true;
    }

    public List<Square> getPath(){
		Vector2 lowestPoint = Vector2.zero;
		
		List<Square> path = new List<Square>();
        /*
         * 
         * Mark the path from monster to hero.
         * 
         * */
        Vector2 startingPoint = FindCode(SquareContent.Start);
        int pointX = (int)startingPoint.x;
        int pointY = (int)startingPoint.y;
        if (pointX == -1 && pointY == -1){
            return null;
        }

		
        for (int i = 0; i < 50; i++){
//		while(true){
            /*
             * 
             * Look through each direction and find the square
             * with the lowest number of steps marked.
             * 
             * */
            lowestPoint = Vector2.zero;
            int lowest = 10000;

            foreach (Vector2 movePoint in ValidMoves(pointX, pointY)){
                int count = _squares[(int) movePoint.x, (int) movePoint.y].DistanceSteps;
                if (count < lowest){
                    lowest = count;
                    lowestPoint.x = movePoint.x;
                    lowestPoint.y = movePoint.y;
                }
            }
            if (lowest != 10000){
                /*
                 * 
                 * Mark the square as part of the path if it is the lowest
                 * number. Set the current position as the square with
                 * that number of steps.
                 * 
                 * */
                _squares[(int) lowestPoint.x, (int) lowestPoint.y].IsPath = true;
				
				pointX = (int) lowestPoint.X;
                pointY = (int) lowestPoint.Y;
				
				Debug.Log("LOWEST");
				Debug.Log(lowestPoint);
				
				path.Add( _squares[(int) lowestPoint.x, (int) lowestPoint.y] );
            
			}else{
                break;
            }

            if (_squares[pointX, pointY].ContentCode == SquareContent.Target){
                /*
                 * 
                 * We went from monster to hero, so we're finished.
                 * 
                 * */
                break;
            }
        }
		
		return path;
    }

    private IEnumerable AllSquares(){
        for (int x = 0; x < sizeX; x++){
            for (int y = 0; y < sizeY; y++){
                yield return new Vector2(x, y);
            }
        }
    }

    private IEnumerable ValidMoves(int x, int y){
        foreach (Vector2 movePoint in _movements){
            int newX = x + (int) movePoint.x;
            int newY = y + (int) movePoint.y;

            if (ValidCoordinates(newX, newY) && SquareOpen(newX, newY)){
                yield return new Vector2(newX, newY);
            }
        }
    }
	
   private void InitMovements(){

        if (diagonal){
			_movements = new Vector2[]{
                new Vector2(-1, -1),
                new Vector2(0, -1),
                new Vector2(1, -1),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                new Vector2(-1, 1),
                new Vector2(-1, 0)
            };
        }else{
			_movements = new Vector2[]{
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(-1, 0)
            };
            
        }
    }
	
    private bool ValidCoordinates(int x, int y){
        if (x < 0){
            return false;
        }
        if (y < 0){
            return false;
        }
        if (x > sizeX-1){
            return false;
        }
        if (y > sizeY-1){
            return false;
        }
        return true;
    }

    private bool SquareOpen(int x, int y){

        switch (_squares[x, y].ContentCode){
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

    private Vector2 FindCode(SquareContent contentIn){

        foreach (Vector2 point in AllSquares()){
            if (_squares[(int) point.x, (int) point.y].ContentCode == contentIn){
                return new Vector2(point.x, point.y);
            }
        }
        return new Vector2(-1, -1);
    }
}