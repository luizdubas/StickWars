using System;
using UnityEngine;

public class MatchController : MonoBehaviour{	
	
	private Vector2 screenSelectionStartPoint;
	private Vector3 sceneSelectionStartPoint;
	
	public GUISkin guiSkin;
	
	public MatchController (){
	}

	#region Behaviour

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
		if( Input.GetButtonDown("Fire1") ){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit) ){
				sceneSelectionStartPoint = hit.point;
				
				screenSelectionStartPoint = Input.mousePosition;
			}
		}
		
		if( Input.GetButtonUp("Fire1") ){
			screenSelectionStartPoint = Input.mousePosition;
		}
		
	}
	
	void OnGUI() {
		GUI.skin = guiSkin;
		
		if( Input.GetButton("Fire1") && Vector2.Distance(screenSelectionStartPoint, Input.mousePosition) > 10 ){
			//Screen coordinates are bottom-left is (0,0) and top-right is (Screen.width, Screen.height)
			GUI.Box( 
				new Rect(
					screenSelectionStartPoint.x, 
					Screen.height-screenSelectionStartPoint.y, 
					Input.mousePosition.x-screenSelectionStartPoint.x, 
					-(Input.mousePosition.y-screenSelectionStartPoint.y)), 
					"", 
					guiSkin.customStyles[0]
			);
		}

	}
	
	#endregion
}

