using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	private	List<IUnit> _units = new List<IUnit>();
	private List<IBuilding> _buildings = new List<IBuilding>();
	private int _sticks;
	private int _circles;
	private bool _isHuman;
	private bool _isMain;
	public Color _stickColor;

	private Transform _selectionBox;
	private Vector2 screenSelectionStartPoint;
	private Vector2 screenSelectionEndPoint;
	private Vector3 sceneSelectionStartPoint;
	private Vector3 sceneSelectionEndPoint;
	private Player _controlledPlayer;
	private List<IUnit> _selectedUnits = new List<IUnit>();
	private RaycastHit[] _selectedUnitsToSave;
	private bool _multiSelection = false;
	private bool _selectionEnded = false;
	private float _xUnitPer3DUnit;
	private float _yUnitPer3DUnit;
	private int _minMouseDrag = 10;
	
	public Player (bool isHuman, bool isMain)
	{		
		_isHuman = isHuman;
		_isMain = isMain;
	}

	public bool IsMain{
		get { return _isMain; }
	}

	public void AddUnit(IUnit unit){
		_units.Add (unit);
	}
	
	public void AddBuilding(IBuilding building){
		_buildings.Add (building);
	}

	public void Start(Transform selectionBox){
		_selectionBox = selectionBox;
		_selectionBox.parent = null;
		ConvertSceneToScreenScale ();
	}
	
	// Update is called once per frame
	public void Update () {


		//Unit Selection Region
		if( Input.GetButtonDown("Fire1") ){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<(int)LayerConstants.GROUND)){
				sceneSelectionStartPoint = hit.point;
			}

			if( Physics.Raycast( ray, out hit, 1<<(int)LayerConstants.UNITS) ){
				ClearSelectedUnits();
				_selectionEnded = false;
				if(hit.collider.GetComponent<Unit>() != null){
					hit.collider.GetComponent<Unit>().Selected = true;
					_selectedUnitsToSave = new RaycastHit[1];
					_selectedUnitsToSave[0] = hit;
				}
			}
			screenSelectionStartPoint = Input.mousePosition;
		}

		if (Input.GetButton ("Fire1")) {
			PreviewSelectedUnits();
		}

		if( Input.GetButtonUp("Fire1") ){
			_selectionEnded = true;
			screenSelectionStartPoint = Input.mousePosition;
			if(_multiSelection){
				SaveSelectedUnits();
			}
			else if(_selectedUnitsToSave.Length == 1){
				_selectedUnits.Add(_selectedUnitsToSave[0].collider.GetComponent<Unit>());
			}
			_selectedUnitsToSave = new RaycastHit[0];
			_selectionBox.localScale = new Vector3(0,0,0);
			_selectionBox.position = new Vector3(0,0,0);
		}
		
	}
	
	public void OnGUI(GUISkin guiSkin) {		
		if( Input.GetButton("Fire1") && Vector2.Distance(screenSelectionStartPoint, Input.mousePosition) > _minMouseDrag ){
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
	#region Unit Selection

	void ConvertSceneToScreenScale(){
		Ray ray1 = Camera.main.ScreenPointToRay (new Vector3 (0, 0, 0));
		RaycastHit hit1;
		Ray ray2 = Camera.main.ScreenPointToRay (new Vector3 (Screen.width,Screen.height, 0));
		RaycastHit hit2;
		Physics.Raycast(ray1, out hit1, Mathf.Infinity, 1<<8);
		Physics.Raycast(ray2, out hit2, Mathf.Infinity, 1<<8);
		_xUnitPer3DUnit = Screen.width/Mathf.Abs(hit1.point.x-hit2.point.x);
		_yUnitPer3DUnit = Screen.height/Mathf.Abs(hit1.point.z-hit2.point.z);
	}
	
	void SaveSelectedUnits(){
		ClearSelectedUnits ();
		_selectionEnded = true;
		foreach(RaycastHit hit in _selectedUnitsToSave){
			if(hit.collider.GetComponent<Unit>() == null) continue;
			_selectedUnits.Add (hit.transform.GetComponent<Unit>());
			hit.transform.GetComponent<Unit>().Selected = true;
		}
	}

	void PreviewSelectedUnits(){
		_multiSelection = (Vector2.Distance(screenSelectionStartPoint, Input.mousePosition) > _minMouseDrag);
		if (_multiSelection) {
			Debug.Log ("Trying to multiselect");
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<(int)LayerConstants.GROUND)){
				_selectionBox.localScale = new Vector3(100,1,hit.point.z-sceneSelectionStartPoint.z);
				_selectionBox.position = new Vector3(sceneSelectionStartPoint.x, sceneSelectionStartPoint.y, sceneSelectionStartPoint.z+(_selectionBox.lossyScale.z/2));
				_selectedUnitsToSave = _selectionBox.rigidbody.SweepTestAll(new Vector3(hit.point.x-sceneSelectionStartPoint.x,0,0), Mathf.Abs(hit.point.x-sceneSelectionStartPoint.x));
				for(var i=0;i<_selectedUnitsToSave.Length;i++){
					//Test if every collider is a Unit and on the same Team
					if(_selectedUnitsToSave[i].collider.GetComponent<Unit>() == null){return;}
					if(_selectedUnitsToSave[i].collider.GetComponent<Unit>().Owner == this){
						_selectedUnitsToSave[i].collider.GetComponent<Unit>().Selected = true;
					}
				}
			}
		}
	}

	void UpdateSelectedUnits(){
		for (int i = _selectedUnits.Count-1; i >= 0; i--) {
			if(_selectedUnits[i] == null)
				_selectedUnits.RemoveAt(i);
		}
	}
	
	void ClearSelectedUnits(){
		foreach (IUnit unit in _selectedUnits) {
			unit.Selected = false;
		}
		_selectedUnits.Clear ();
	}

	public List<IUnit> SelectedUnits{
		get { return _selectedUnits; }
	}

	#endregion

}
