using UnityEngine;
using System.Collections.Generic;

public class PeasantHouse : AbstractBuilding
{
	int _unitNumber;
	Queue<string> _peasantQueue;
	bool _showGUI;
	Player _owner;
	int _createdUnits; //ONLY FOR DEBUG // DELETE LATER!!!!!!
	int _secondsToWait = 1;
	float _timer = 0;
	bool _creatingUnit = false;

	public GameObject _unitToCreate;
	public GUISkin _skin;

	public override int HP {
		get {
			return 200;
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
		_peasantQueue = new Queue<string>();
		_unitNumber = 0;
		_createdUnits = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (_creatingUnit) {
			_timer -= Time.fixedDeltaTime;
			Debug.Log ("creating timer: "+_timer);
			if(_timer <= 0){
				CreateUnit ();
				if(_peasantQueue.Count > 0){
					_timer = _secondsToWait;
				}else{
					_creatingUnit = false;
				}
			}
		}
		//DEBUG DELETE LATER
		if (Owner == null) {
			MatchController matchController =(MatchController) GameObject.FindObjectOfType(typeof(MatchController));
			Owner = matchController.ControlledPlayer;
		}
	}
	
	public void OnGUI()
	{
		GUI.skin = _skin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 768f, 1));
		if (_showGUI) {			
			if(GUI.Button(new Rect(20, 40f, 128, 128),"",_skin.GetStyle("AddPeasant"))){
				QueueUnit();
			}	
			if(GUI.Button(new Rect(200, 40f, 128, 128),"",_skin.GetStyle("CancelAction"))){
				_showGUI = false;
			}
		}
	}
	
	#endregion
	
	#region IBuilding implementation
	public override void ShowOptions () {
		Debug.Log("HERE!! PeasantHouse->ShowOptions()");
		_showGUI = true;
	}

	private void QueueUnit(){
		if (!_creatingUnit)
			_timer = _secondsToWait;
		_peasantQueue.Enqueue("peasant"+_unitNumber);
		_unitNumber++;
		_creatingUnit = true;
	}

	public override IUnit CreateUnit ()
	{
		Vector3 position = new Vector3(BuildingPosition.x + (5 * _createdUnits),0,BuildingPosition.z + 34);
		GameObject peasant = GameObject.Instantiate( _unitToCreate, 
				position, Quaternion.identity ) as GameObject;
		peasant.name = _peasantQueue.Dequeue();
		Unit peasantUnit = peasant.GetComponent<Unit>();
		//Debug.Log("Peasant is null? "+(peasantUnit == null));
		if(peasantUnit != null)
		{
			//Debug.Log("Here!!!");
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

	#endregion
}

