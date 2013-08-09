using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarTest2 : MonoBehaviour {
	
	PathFinding pathFinder;
	List<Square> path = null;
	int index = 0;
	bool walk;
	float time;
	
	float walkVelocity = 0.15f;
	
	void Start () {
		pathFinder = new SimpleAStar(100, 100, 5, 5);
		setWalls();
		
		walk = false;
		time = 0;
	}
	
	public Vector2 getWorldPosition( Vector2 target ){
		Vector2 position = new Vector2(0, 0);
		
		position.x = target.x + 0.5f;
		position.y = (target.y + 0.5f) * - 1;
		
		return position;
	}
	
	void Update () {
		
		if( walk ){
			time += Time.deltaTime;
			if( time >= walkVelocity ){
				time -= walkVelocity;
				
				Square posicao = path[index];
				index++;
				
				GameObject hero = GameObject.Find("hero");
				
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
				GameObject hero = GameObject.Find("hero");
				
				Vector2 start = new Vector2( hero.transform.position.x, hero.transform.position.z * -1 );
				Vector2 target = new Vector2( hit.point.x, hit.point.z * -1 );
				
				pathFinder.ClearLogic();
				right = pathFinder.PathFind( start, target );

				if( right ){
					path = pathFinder.getPath();
							
					index = 0;
					time = 0;
					walk = true;
				}
			}	
		}
	}
	
	void setWalls(){
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
		
		pathFinder.setWalls( walls );
	}
}