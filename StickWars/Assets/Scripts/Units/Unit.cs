using UnityEngine;
using System;

public class Unit : MonoBehaviour, IUnit
{
	public static int UNIT_ID = 0;
	int _id;
	public int _hp;
	bool _selected;
	IUnitClass _unitClass;
	Player _owner;
	LineRenderer _lineRenderer;
	GameObject _selectionIndicator;
	MatchController _controller;
	
	public Player Owner {
		get {
			return _owner;
		}
		set {
			_owner = value;
		}
	}
	
	public int ID {
		get {
			return _id;
		}
	}
	
	public bool Selected{
		get { return _selected; }
		set { 
			_selected = value;
			if (Owner.IsMain) {
				_selectionIndicator.SetActive (_selected);
			}
		}
	}
	
	public GameObject ParentObject{
		get { return this.gameObject; }
	}
	
	public int HP {
		get {
			return _hp;
		}
		set {
			_hp = value;
		}
	}

	public IUnitClass UnitClass {
		get {
			return _unitClass;
		}
		set {
			_unitClass = value;
		}
	}
	
	public Unit(){
		_id = UNIT_ID++;
		_unitClass = new PeasantClass();
		_hp = _unitClass.HP;
		Debug.Log("Unit initialized: "+_id+" "+_unitClass.Name);
	}

	public void Awake(){
		_lineRenderer = this.gameObject.GetComponent<LineRenderer> ();
		foreach(Transform t in this.gameObject.transform){
			if(t.name == "selection"){
				_selectionIndicator = t.gameObject;
				break;
			}
		}
		_selectionIndicator.SetActive (false);
	}

	public void Update()
	{
		if (_controller == null) {
			_controller = (MatchController) GameObject.FindObjectOfType(typeof(MatchController));
			//DEBUG DELETE LATER
			if (Owner == null) {
				Owner = _controller.ControlledPlayer;
				SetColor(Owner._stickColor);
			}
		}
		float percent = _hp / (float)_unitClass.HP;
		_lineRenderer.SetWidth (percent, percent);
	}
	
	#region IUnit implementation
	public void MoveTo (UnityEngine.Vector3 point)
	{
		throw new NotImplementedException ();
	}

	public void StopMovement ()
	{
		throw new NotImplementedException ();
	}

	public void AttackMode ()
	{
		throw new NotImplementedException ();
	}

	public void DefenseMode ()
	{
		throw new NotImplementedException ();
	}

	public void AttackUnit (IUnit unit)
	{
		throw new NotImplementedException ();
	}

	public void AttackBuilding (IBuilding building)
	{
		throw new NotImplementedException ();
	}

	public void Build (IBuilding building)
	{
		throw new NotImplementedException ();
	}

	public void Destroy (IBuilding building)
	{
		throw new NotImplementedException ();
	}

	public void SetColor(Color playerColor){
		foreach(Transform t in this.gameObject.transform){
			if(t.name == "peasant"){
				foreach(Transform modelChildren in t.transform){
					if(modelChildren.name == "Body"){
						modelChildren.gameObject.renderer.material.color = playerColor;
					}else if(modelChildren.name == "Head"){
						modelChildren.gameObject.renderer.material.SetColor("_OutlineColor", playerColor);
					}
				}
				break;
			}
		}
	}

	#endregion
}

