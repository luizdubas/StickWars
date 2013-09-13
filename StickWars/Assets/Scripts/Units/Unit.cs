using UnityEngine;
using System;

public class Unit : MonoBehaviour, IUnit
{
	public static int UNIT_ID = 0;
	int _id;
	public int _hp;
	bool _selected;
	AbstractSource _collectedObject;
	IUnitClass _unitClass;
	Player _owner;
	LineRenderer _lineRenderer;
	MatchController _controller;
	PathFindingController _movementController;
	float _timer = 0;
	
	bool _waitStartAnimationEnd = false;
	bool _waitEndAnimationEnd = false;
	bool _isCollecting = false;
	bool _movingToCollect = false;
	bool _searchObjectToCollect = false;
	
	Transform _unit;
	Transform _leftHand, _rightHand;
	Transform _basket;
	GameObject _selectionIndicator;
	
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

	public void Start(){
		_lineRenderer = this.gameObject.GetComponent<LineRenderer> ();
		_selectionIndicator = this.gameObject.transform.FindChild ("selection").gameObject;
		_selectionIndicator.SetActive (false);
		_unit = this.gameObject.transform.FindChild ("model");
		_basket = this.gameObject.transform.FindChild ("basket");
		_leftHand = this.gameObject.transform.Find ("model/Armature/Body_0/Shoulder_L/ArmStart_L/ArmEnd_L");
		_rightHand = this.gameObject.transform.Find ("model/Armature/Body_0/Shoulder_R/ArmStart_R/ArmEnd_R");
		_movementController = this.gameObject.GetComponent<PathFindingController>();
	}

	public void Update()
	{
		if (_controller == null) {
			_controller = (MatchController) GameObject.FindObjectOfType(typeof(MatchController));
			if (Owner == null) {
				Owner = _controller.ControlledPlayer;
				SetColor(Owner._stickColor);
			}
		}
		if(_waitStartAnimationEnd && !_unit.animation.isPlaying){
			_unit.animation.Play ("Collect");
			_unit.animation.wrapMode = WrapMode.Loop;
			_isCollecting = true;
			_waitStartAnimationEnd = false;
			_basket.parent = _leftHand;
			_basket.localPosition = new Vector3 (-1.66f, 1.5f, -0.34f);
			_basket.gameObject.SetActive (true);
		}
		if(_waitEndAnimationEnd && !_unit.animation.isPlaying){
			_movementController.reenableMove (true);
			_waitEndAnimationEnd = false;
		}
		if(_isCollecting){
			if(_collectedObject == null || _collectedObject._amount == 0){
				_basket.gameObject.SetActive (false);
				_waitEndAnimationEnd = true;
				_unit.animation.Play ("EndCollect");
				_unit.animation.wrapMode = WrapMode.Once;
				_isCollecting = false;
				_collectedObject = null;
			}
			_timer -= Time.fixedDeltaTime;
			if(_timer <= 0){
				int collected = UnitClass.ResourceCapacity;
				if (_collectedObject != null) {
					MaterialType type = _collectedObject._type;
					_collectedObject.CollectSource (ref collected);
					_owner.AddMaterial (type, collected);
					_timer = UnitClass.SecondsPerCollect;
				}
			}
		}
		float percent = _hp / (float)_unitClass.HP;
		_lineRenderer.SetWidth (percent, percent);
	}
	
	#region IUnit implementation
	public void MoveTo (UnityEngine.Vector3 point)
	{
		if (!_movingToCollect)
			StopCollecting ();
		_movementController.move (point);
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

	public void StartCollecting (MaterialType material){

	}

	public void StartCollecting (AbstractSource source){
		_collectedObject = source;
		_movingToCollect = true;
		_timer = UnitClass.SecondsPerCollect;
		MoveTo (source.gameObject.transform.position);
	}

	public void StopCollecting() {
		_collectedObject = null;
		_isCollecting = false;
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

	void OnTriggerEnter(Collider collider) {
		if(_movingToCollect && collider.gameObject.layer == _collectedObject.gameObject.layer){
			_movementController.stop (false);
			_waitStartAnimationEnd = true;
			_unit.animation.Play ("StartCollect");
			_unit.animation.wrapMode = WrapMode.Once;
			_movingToCollect = false;
		}
	}

	#endregion
}

