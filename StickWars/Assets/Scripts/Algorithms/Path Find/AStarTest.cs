using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarTest : MonoBehaviour {
	
	PathFinding pathFinder;
	List<Square> path = null;
	int index = 0;
	bool walk;
	float time;
	
	float walkVelocity = 0.15f;
	GameObject hero;
	
	Map map;
	
	void Start () {
		
		createMap();
		
		pathFinder = new SimpleAStar(map, false);
		
		walk = false;
		time = 0;
		hero = GameObject.Find("hero");
	}
	
	void createMap(){
		map = new Map();
		
		map.SizeX = 20;
		map.SizeY = 20;
		
		map.SquareWidth = 1;
		map.SquareHeight = 1;
		map.MapGameObject = GameObject.Find( "Map" );
		
		map.buildMap();
		
		List<Vector2> walls = new List<Vector2>();
		
		walls.Add( new Vector2(4,0) );
		
		walls.Add( new Vector2(4,1) );
		
		walls.Add( new Vector2(0,2) );
		walls.Add( new Vector2(3,2) );
		walls.Add( new Vector2(8,2) );
		
		walls.Add( new Vector2(2,3) );
		
		walls.Add( new Vector2(3,4) );
		
		walls.Add( new Vector2(4,5) );
		
		walls.Add( new Vector2(2,6) );
		walls.Add( new Vector2(4,6) );
		
		walls.Add( new Vector2(4,7) );
		
		walls.Add( new Vector2(2,8) );
		walls.Add( new Vector2(8,8) );
		
		walls.Add( new Vector2(4,9) );
		walls.Add( new Vector2(5,9) );
		walls.Add( new Vector2(6,9) );
		
		walls.Add( new Vector2(10,11) );
		
		map.addObstacles( walls );
	}
	
	void Update () {
		
		if( walk ){
			time += Time.deltaTime;
			if( time >= walkVelocity ){
				time -= walkVelocity;
				
				Square posicao = path[index];
				index++;
				
				hero.transform.position = new Vector3(posicao.Center.x , 0, posicao.Center.y );
				
				if( index == path.Count){
					walk = false;
					time = 0;
					index = 0;
				}
			}
		}
		
		if( Input.GetButtonDown("Fire1") && !walk){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool right = false;
			
			if( Physics.Raycast( ray, out hit) ){
				pathFinder.ClearLogic();
				right = pathFinder.PathFind( map.getCoordinatesByWorldPosition( hero.transform.position ) , map.getCoordinatesByWorldPosition( hit.point ) );

				if( right ){
					path = pathFinder.getPath();
							
					index = 0;
					time = 0;
					walk = true;
				}
			}	
		}
	}

}