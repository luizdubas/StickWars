using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SimpleAStar : PathFinding{

    private bool diagonal = false;
	private Vector2[] _movements;
	private Map map = null;
	
	private float squareWidth;
	private float squareHeight;

    public SimpleAStar(){
        InitMovements();
    }
	
	public SimpleAStar(Map map, bool diagonal){
		this.map = map;
		this.diagonal = diagonal;
		
        InitMovements();
	}
	
	public void setStart(Vector2 target){
		Vector2 oldPosition = map.FindByCode(SquareContent.Start);

		if( oldPosition.x > -1 && oldPosition.y > -1 ){
			map.getSquare( oldPosition.x, oldPosition.y).ContentCode = SquareContent.Empty;		
		}
		
		map.getSquare( target.x, target.y).ContentCode = SquareContent.Start;
	}
	
	public void setTarget(Vector2 target){
		Vector2 oldPosition = map.FindByCode(SquareContent.Target);

		if( oldPosition.x > -1 && oldPosition.y > -1 ){
			map.getSquare( oldPosition.x, oldPosition.y).ContentCode = SquareContent.Empty;		
		}
		
		map.getSquare( target.x, target.y).ContentCode = SquareContent.Target;
	}

	public void clearStartAndTarget(){
		Vector2 point;
		
		point = map.FindByCode(SquareContent.Target);
		
		map.getSquare( point.x, point.y).ContentCode = SquareContent.Empty;
		
		point = map.FindByCode(SquareContent.Start);
		
		map.getSquare( point.x, point.y).ContentCode = SquareContent.Empty;
	}

    public void ClearLogic(){

        foreach (Vector2 point in map.AllSquares()){
            map.getSquare( point.x, point.y ).DistanceSteps = 10000;
            map.getSquare( point.x, point.y ).IsPath = false;
        }
    }
	
    public bool PathFind( Vector2 start, Vector2 target ){
	
		if( !map.MapBuilded ){
			return false;
		}
		
		setStart( start );
		setTarget( target );
		
		/*
         * 
         * Find path from start to target. First, get coordinates of start.
         * 
         * */
        Vector2 startingPoint = map.FindByCode(SquareContent.Target);
        int heroX = (int) startingPoint.x;
        int heroY = (int) startingPoint.y;
		
        if (heroX == -1 || heroY == -1){
            return false;
        }
        /*
         * 
         * Target starts at distance of 0.
         * 
         * */
        map.getSquare(heroX, heroY).DistanceSteps = 0;
		
		while(true){
            bool madeProgress = false;
			
            /*
             * 
             * Look at each square on the board.
             * 
             * */
            foreach (Vector2 mainPoint in map.AllSquares()){
				int x = (int) mainPoint.x;
                int y = (int) mainPoint.y;
				Square mainPointSquare = map.getSquare( x, y );

                if (map.SquareOpen(mainPointSquare)){
                    int passHere = mainPointSquare.DistanceSteps;

                    foreach (Vector2 movePoint in ValidMoves(x, y)){
                        Square movePointSquare = map.getSquare( movePoint.x, movePoint.y );
                        int newPass = passHere + 1;
						
                        if (movePointSquare.DistanceSteps > newPass){
                            movePointSquare.DistanceSteps = newPass;
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
        Vector2 startingPoint = map.FindByCode(SquareContent.Start);
        int pointX = (int)startingPoint.x;
        int pointY = (int)startingPoint.y;
        if (pointX == -1 && pointY == -1){
            return null;
        }

		while(true){
            /*
             * 
             * Look through each direction and find the square
             * with the lowest number of steps marked.
             * 
             * */
            lowestPoint = Vector2.zero;
            int lowest = 10000;

            foreach (Vector2 movePoint in ValidMoves(pointX, pointY)){
                int count = map.getSquare(movePoint.x, movePoint.y).DistanceSteps;
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
				Square lowestPointSquare = map.getSquare( lowestPoint.x, lowestPoint.y );
                lowestPointSquare.IsPath = true;
				
				pointX = (int) lowestPoint.x;
                pointY = (int) lowestPoint.y;
				
				path.Add( lowestPointSquare );
            
			}else{
                break;
            }

            if ( map.getSquare(pointX, pointY).ContentCode == SquareContent.Target){
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

	private IEnumerable ValidMoves(int x, int y){
        foreach (Vector2 movePoint in _movements){
            int newX = x + (int) movePoint.x;
            int newY = y + (int) movePoint.y;

            if (ValidCoordinates(newX, newY) && map.SquareOpen(newX, newY)){
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
        if (x > map.SizeX - 1){
            return false;
        }
        if (y > map.SizeY - 1){
            return false;
        }
        return true;
    }

}