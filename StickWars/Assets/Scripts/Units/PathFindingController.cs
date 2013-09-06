using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class PathFindingController : AIPath{
	
	/** Minimum velocity for moving */
	public float sleepVelocity = 0.4F;
	
	/** Speed relative to velocity with which to play animations */
	public float animationSpeed = 0.2F;
	
	private Camera cam;

	// Use this for initialization
	void Start (){
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();
		
		SearchPath();
	}
	
	public override Vector3 GetFeetPosition (){
		return tr.position;
	}
	
	
	public override void OnTargetReached () {
		SearchPath();
	}
	
	// Update is called once per frame
	void Update (){
		RaycastHit hit;
		if (Physics.Raycast	(cam.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity)) {
			target.position = hit.point;
		}
		
		Vector3 velocity;
		
		if( canMove ){
			Vector3 dir = CalculateVelocity (GetFeetPosition());
			
			//Rotate towards targetDirection (filled in by CalculateVelocity)
			if (targetDirection != Vector3.zero) {
				RotateTowards (targetDirection);
			}
			
			if (dir.sqrMagnitude > sleepVelocity*sleepVelocity) {
				//If the velocity is large enough, move
			} else {
				//Otherwise, just stand still (this ensures gravity is applied)
				dir = Vector3.zero;
			}
			
			if (navController != null)
				navController.SimpleMove (GetFeetPosition(), dir);
			else if (controller != null)
				controller.SimpleMove (dir);
			else
				Debug.LogWarning ("No NavmeshController or CharacterController attached to GameObject");
			
			velocity = controller.velocity;
		} else {
			velocity = Vector3.zero;
		}
	}
}

