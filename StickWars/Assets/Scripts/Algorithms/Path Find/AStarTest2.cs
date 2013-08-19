using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarTest2 : MonoBehaviour {
	
	private PathFinding pathFinder;
	private List<Square> path = null;
	private int index = 0;
	private bool walk;
	private float time;
	
	private float walkVelocity = 0.15f;
	private GameObject hero;
	
	private Map map;
	private CameraControllerInGame cameraController;
	
	private bool debug = true;
	
	void Start () {
		hero = GameObject.Find("hero");
		
		createMap();
		
		startCameraController();
		
		pathFinder = new SimpleAStar(map, false);
		
		walk = false;
		time = 0;
	}
	
	void startCameraController(){
		
		cameraController = GameObject.Find("Camera").GetComponent<CameraControllerInGame>();
		
		cameraController.setCameraPosition( new Vector3( 40, 40, -70 ) );
		cameraController.setCameraRotation( new Vector3( 50, 0, 0 ) );
		
		cameraController.setMargins( 40, -70, -40, -20 );
		cameraController.setMinMaxZoom( 10, 40 );
		
		cameraController.setMap( map );
		
		cameraController.activate();
	}
	
	void createMap(){
		map = GameObject.Find("Map_100_100_5").GetComponent<Map>();
		
		map.SizeX = 100;
		map.SizeY = 100;
		
		map.SquareWidth = 5;
		map.SquareHeight = 5;
		map.MapGameObject = GameObject.Find( "Map_100_100_5" );
		
		map.buildMap();
		
		map.createRandomsObstacles( 1000, map.ObstaclePrefab );
	}
	
	void Update () {

		if( walk ){
			time += Time.deltaTime;
			if( time >= walkVelocity && path.Count > 0 ){
				time -= walkVelocity;
				
				Square posicao = path[index];
				index++;
				
				hero.transform.position = new Vector3(posicao.Center.x , hero.transform.position.y, posicao.Center.y );
				
				if( index == path.Count){
					walk = false;
					time = 0;
					index = 0;
					
					path.Clear();
				}
			}
		}
		
		if( Input.GetButtonDown("Fire2") && !walk){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool right = false;
			
			if( Physics.Raycast( ray, out hit) ){
				pathFinder.ClearLogic();
				right = pathFinder.PathFind( map.getCoordinatesByWorldPosition( hero.transform.position ) , map.getCoordinatesByWorldPosition( hit.point ) );

				if( right ){
					path = pathFinder.getPath();
					
					if( debug ){
						
					}
					
					index = 0;
					time = 0;
					walk = true;
				}
			}	
		}
	}
	
	void drawPath(){
		
	}
	
	
}