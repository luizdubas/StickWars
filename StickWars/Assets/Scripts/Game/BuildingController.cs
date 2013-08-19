using System;
using UnityEngine;


public class BuildingController : MonoBehaviour
{
	public BuildingController ()
	{
	}
	
	#region Behaviour
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if( Input.GetButtonDown("Fire1")){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			bool right = false;
			
			if( Physics.Raycast( ray, out hit) ){
				if(hit.collider.gameObject.tag == "Building"){
					Debug.Log ("Acertou construçao");
				}
			}	
		}
	}
	
	#endregion
}

