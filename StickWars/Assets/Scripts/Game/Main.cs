using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

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
		//hero = GameObject.Find("hero");
		
		createMap();
		
		startCameraController();
		
		//pathFinder = new SimpleAStar(map, false);
		
		walk = false;
		time = 0;
	}
	
	void startCameraController(){
		
		cameraController = GameObject.Find("Main Camera").GetComponent<CameraControllerInGame>();
		
		cameraController.setCameraPosition( new Vector3( 450, 50, -350 ) );
		cameraController.setCameraRotation( new Vector3( 50, 180, 0 ) );
		
			//	                     l   t    r    b
		cameraController.setMargins( 50, 5, -50, 105 );
		cameraController.setMinMaxZoom( 10, 50 );
		
		cameraController.setMap( map );
		
		cameraController.activate();
	}
	
	void createMap(){
		map = GameObject.Find("ground").GetComponent<Map>();
		
		map.SizeX = 100;
		map.SizeY = 100;
		
		map.SquareWidth = 5;
		map.SquareHeight = 5;
		map.MapGameObject = GameObject.Find( "ground" );
		
		map.buildMap();
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
		
		/*
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
		*/

	}
	
	void drawPath(){
		
	}
}
