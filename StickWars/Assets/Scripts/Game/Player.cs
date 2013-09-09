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
	private int _gold;
	private bool _isHuman;
	private bool _isMain;
	public Color _stickColor;
	public GameObject[] _ghostObjects;

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
	private bool _mouseOnGui = false;
	private bool _showBuildingOptions = false;
	private GameObject _grid;
	private IBuilding _selectedBuilding;

	
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

	public void Start(Transform selectionBox, GameObject grid, ref GameObject[] ghostObjects){
		_selectionBox = selectionBox;
		_selectionBox.parent = null;
		_ghostObjects = ghostObjects;
		_grid = grid;
		ConvertSceneToScreenScale ();
	}
	
	// Update is called once per frame
	public void Update () {

		//Unit Selection Region
		if (!_mouseOnGui) {
			if (Input.GetButtonDown ("Fire1")) {
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				int groundLayer = 1 << (int)LayerConstants.GROUND;
				int buildingLayer = 1 << (int)LayerConstants.BUILDINGS;
				if (Physics.Raycast (ray, out hit, Mathf.Infinity, groundLayer | buildingLayer)) {
					if (hit.collider.gameObject.layer == (int)LayerConstants.GROUND) {
						sceneSelectionStartPoint = hit.point;
						UnselectBuild ();
					}
				}

				if (Physics.Raycast (ray, out hit, 1 << (int)LayerConstants.UNITS)) {
					ClearSelectedUnits ();
					_selectionEnded = false;
					if (hit.collider.GetComponent<Unit> () != null) {
							_selectedUnitsToSave = new RaycastHit[1];
							_selectedUnitsToSave [0] = hit;
					}
				}
				screenSelectionStartPoint = Input.mousePosition;
			}

			if (Input.GetButton ("Fire1")) {
				PreviewSelectedUnits ();
			}

			if (Input.GetButtonUp ("Fire1")) {
				_selectionEnded = true;
				screenSelectionStartPoint = Input.mousePosition;
				SaveSelectedUnits ();
				_selectedUnitsToSave = new RaycastHit[0];
				_selectionBox.localScale = new Vector3 (1, 1, 1);
				_selectionBox.position = new Vector3 (10, 1000, -10);
			}
			
			if (Input.GetButtonUp ("Fire2") && _selectedUnits.Count > 0) {
				Vector3 target;
				RaycastHit hit;
				if (Physics.Raycast	(Camera.main.ScreenPointToRay (Input.mousePosition), out hit, Mathf.Infinity)) {
					target = hit.point;
					
					foreach( IUnit unity  in _selectedUnits ){
						PathFindingController pfc = ( (Unit) unity ).gameObject.GetComponent<PathFindingController>();
						pfc.move( target );
					}
				}
			}
			
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
		if (_selectedUnitsToSave != null && _selectedUnitsToSave.Length > 0) {
			Debug.Log ("Saving!!!");
			bool hasBuilders = false;
			foreach (RaycastHit hit in _selectedUnitsToSave) {
				if (hit.collider.GetComponent<Unit> () == null)
						continue;
				Unit unit = hit.transform.GetComponent<Unit> ();
				_selectedUnits.Add (unit);
				unit.Selected = true;
				if (unit.UnitClass.CanBuild)
					hasBuilders = true;
			}
		}
	}

	void PreviewSelectedUnits(){
		_multiSelection = (Vector2.Distance(screenSelectionStartPoint, Input.mousePosition) > _minMouseDrag);
		if (_multiSelection) {
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<(int)LayerConstants.GROUND)){
				_selectionBox.localScale = new Vector3(100,1,hit.point.z-sceneSelectionStartPoint.z);
				_selectionBox.position = new Vector3(sceneSelectionStartPoint.x, sceneSelectionStartPoint.y, sceneSelectionStartPoint.z+(_selectionBox.lossyScale.z/2));
				_selectedUnitsToSave = _selectionBox.rigidbody.SweepTestAll(new Vector3(hit.point.x-sceneSelectionStartPoint.x,0,0), Mathf.Abs(hit.point.x-sceneSelectionStartPoint.x));
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
		_showBuildingOptions = false;
	}

	public void ShowingGUI(){
		_mouseOnGui = true;
	}

	public void GUIHidden(){
		_mouseOnGui = false;
	}

	public List<IUnit> SelectedUnits{
		get { return _selectedUnits; }
	}

	public int Sticks{
		get { return _sticks; }
	}

	public int Circles{
		get { return _circles; }
	}

	public int Gold{
		get { return _gold; }
	}

	#endregion

	#region Building
	
	public void SelectBuild(IBuilding building){
		_selectedBuilding = building;
		ClearSelectedUnits ();
	}

	public void UnselectBuild(){
		_selectedBuilding = null;
	}

	public IBuilding SelectedBuilding{
		get { return _selectedBuilding; }
	}

	public void ShowGhostBuilding(int building){
		Ray rayCursor = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hitCursor;
		Vector3 position = Vector3.zero;
		if (Physics.Raycast (rayCursor, out hitCursor, Mathf.Infinity, 1 << (int)LayerConstants.GROUND)) {
			position = new Vector3 (hitCursor.point.x, 7, hitCursor.point.z);
		}
		GameObject ghostBuilding = GameObject.Instantiate(_grid, position, Quaternion.Euler (new Vector3 (270, 0, 0)) ) as GameObject;
		Vector3 size = _ghostObjects [building].renderer.bounds.size;
		ghostBuilding.transform.localScale = new Vector3(size.x / 2 , size.z / 2, 1);
		ghostBuilding.GetComponent<GhostBuilding> ()._objectToConstruct = _ghostObjects [building];
		ghostBuilding.GetComponent<GhostBuilding> ()._owner = this;
		_showBuildingOptions = false;
	}

	#endregion
}
