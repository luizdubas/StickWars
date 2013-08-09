using UnityEngine;
using System.Collections;
using System.Collections.Generic;

interface PathFinding{
	
	void setMapConfiguration(int sizeX, int sizeY, float squareWidth, float squareHeight);
	
	void setWalls(List<Vector2> walls);
	
	void clearStartAndTarget();
	void clearSquares();
	void ClearLogic();
	
	bool PathFind( Vector2 start, Vector2 target );
	
	List<Square> getPath();
	
	Square[,] getSquares();
}

