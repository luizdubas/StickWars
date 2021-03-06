using UnityEngine;
using System.Collections.Generic;

public class BowTrainingGround : AbstractBuilding
{
	int _unitNumber;
	Queue<PeasantQueueItem> _peasantQueue;
	bool _showGUI;
	Player _owner;
	int _createdUnits; //ONLY FOR DEBUG // DELETE LATER!!!!!!
	int _secondsToWait = 1;
	float _timer = 0;
	bool _creatingUnit = false;

	public GameObject _unitToCreate;
	public GUISkin _skin;
	public GameObject _birthPointIndicator;

	public override string Name {
		get {
			return "BowTrainingGround";
		}
	}

	public override int HP {
		get {
			return 100;
		}
	}

	public override float SecondsToCreate {
		get {
			return 2;
		}
	}

	public override Player Owner{
		get { return _owner; }
		set { _owner = value; }
	}

	public override Vector3 BuildingPosition{
		get { return this.gameObject.transform.position; }
	}

	public override GameObject ParentObject{
		get { return this.gameObject; }
	}

	#region Behaviour

	// Use this for initialization
	void Start () {
		_peasantQueue = new Queue<PeasantQueueItem>();
		_unitNumber = 0;
		_createdUnits = 0;
		if (_owner != null)
			_owner.AddBuilding (this);
	}

	// Update is called once per frame
	void Update () {
		/*if (_creatingUnit) {
			_timer -= Time.fixedDeltaTime;
			Debug.Log ("creating timer: "+_timer);
			PeasantQueueItem item = _peasantQueue.Peek ();
			float percent = 1 - _timer;
			//item.BirthPoint.GetComponent<ParticleSystem> ().particleEmitter = (8 * percent) + 2;
			if(_timer <= 0){
				CreateUnit ();
				if(_peasantQueue.Count > 0){
					_timer = _secondsToWait;
				}else{
					_creatingUnit = false;
				}
			}
		}*/
	}

	public void OnGUI()
	{
		GUI.skin = _skin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 768f, 1));
		/*if (_showGUI) {			
			if(GUI.Button(new Rect(446, 296, 128, 128),"",_skin.GetStyle("AddPeasant"))){
				QueueUnit();
			}	
			if(GUI.Button(new Rect(736, 296, 128, 128),"",_skin.GetStyle("CancelAction"))){
				_showGUI = false;
				Owner.GUIHidden ();
			}
		}*/
	}

	#endregion

	#region IBuilding implementation
	public override void DrawGUI () {
		/*_showGUI = true;
		Owner.ShowingGUI ();*/
	}

	private void QueueUnit(){
		if (!_creatingUnit)
			_timer = _secondsToWait;
		Vector3 position = new Vector3(BuildingPosition.x + (5 * _unitNumber),8,BuildingPosition.z + 34);
		PeasantQueueItem item = new PeasantQueueItem ("peasant" + _unitNumber, position);
		item.BirthPoint = GameObject.Instantiate(_birthPointIndicator, position, Quaternion.Euler (new Vector3 (270, 0, 0)) ) as GameObject;
		_peasantQueue.Enqueue(item);
		_unitNumber++;
		_creatingUnit = true;
	}

	public override IUnit CreateUnit ()
	{
		PeasantQueueItem item = _peasantQueue.Dequeue();
		GameObject peasant = GameObject.Instantiate( _unitToCreate, 
		                                            item.Position, Quaternion.identity ) as GameObject;
		GameObject.DestroyImmediate (item.BirthPoint);
		peasant.name = item.Name;
		Unit peasantUnit = peasant.GetComponent<Unit>();
		if(peasantUnit != null)
		{
			peasantUnit.Owner = Owner;
			peasantUnit.SetColor(Owner._stickColor);
		}
		_createdUnits++;
		return peasantUnit;
	}

	public override IUnit UpgradeUnit ()
	{
		return null;
	}

	public override int MaterialCost (MaterialType material)
	{
		switch(material){
			case MaterialType.Circle:
			return 3;
			case MaterialType.Stick:
			return 15;
		}
		return 0;
	}

	#endregion
}


