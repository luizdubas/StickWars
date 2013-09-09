using System;
using UnityEngine;


public class BuildingController : MonoBehaviour
{
	private MatchController _controller;
	private Player _owner;

	public BuildingController ()
	{
	}
	
	#region Behaviour
	
	// Use this for initialization
	void Start () {
		//Deve estar no mesmo objeto do MatchController
		_controller = this.gameObject.GetComponent<MatchController> ();
		_owner = _controller.ControlledPlayer;
	}
	
	// Update is called once per frame
	void Update () {
		if (_owner == null)
			_owner = _controller.ControlledPlayer;
		if( Input.GetButtonDown("Fire1")){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool right = false;
			
			if( Physics.Raycast( ray, out hit) ){
				if(hit.collider.gameObject.tag == "Building"){
					AbstractBuilding building = hit.collider.gameObject.GetComponent<AbstractBuilding>();
					if(building != null){ 
						_owner.SelectBuild (building);
					}
				}
			}	
		}
	}
	
	#endregion
}

