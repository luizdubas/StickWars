using UnityEngine;
using System.Collections;
using Pathfinding;

[RequireComponent(typeof(Seeker))]
public class PathFindingController : AIPath{
	
	/** Minimum velocity for moving */
	public float sleepVelocity = 0.4F;
	
	/** Speed relative to velocity with which to play animations */
	public float animationSpeed = 0.2F;
	
	//The max distance from the AI to a waypoint for it to continue to the next waypoint
	public float nextWaypointDistance = 3;
	//The waypoint we are currently moving towards
	private int currentWaypoint = 0;

	Transform unit;

	// Use this for initialization
	void Start (){
		unit = this.gameObject.transform.FindChild ("model");
	}
	
	public override Vector3 GetFeetPosition (){
		return tr.position;
	}
	
	
	public override void OnTargetReached () {
		unit.animation.Play ("Rest");
		unit.animation.wrapMode = WrapMode.Loop;	
	}
	
	public void OnPathComplete (Path p) {
	}

	// Update is called once per frame
	void Update (){
			
		if (path == null) {
			return;
		}
		if (target != null) {
			return;
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
		
		/*
		//Direction to the next waypoint
		Vector3 dir = (path.vectorPath[currentWaypoint]-transform.position).normalized;
		dir *= speed * Time.deltaTime;
		controller.SimpleMove (dir);
		//Check if we are close enough to the next waypoint
		//If we are, proceed to follow the next waypoint
		if (Vector3.Distance (transform.position,path.vectorPath[currentWaypoint]) < nextWaypointDistance) {
			currentWaypoint++;
			return;
		}
		*/
		
	}
	
	public void move(Vector3 target){
		unit.animation.Play ("Walk");
		unit.animation.wrapMode = WrapMode.Loop;
		seeker.StartPath(transform.position, target, OnPathComplete);
	}

	public void reenableMove(bool playAnimation){
		if(playAnimation)
		{
			unit.animation.Play ("Walk");
			unit.animation.wrapMode = WrapMode.Loop;	
		}
		canMove = true;
	}

	public void stop(bool playAnimation){
		if(playAnimation)
		{
			unit.animation.Play ("Rest");
			unit.animation.wrapMode = WrapMode.Loop;	
		}
		canMove = false;
	}
}

