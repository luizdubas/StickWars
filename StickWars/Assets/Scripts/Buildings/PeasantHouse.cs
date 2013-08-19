using UnityEngine;
using System.Collections.Generic;

public class PeasantHouse : MonoBehaviour, IBuilding 
{
	int _unitNumber;
	public GameObject _unitToCreate;
	Queue<string> _peasantQueue;
	bool _showGUI;
	Player _owner;

	public int HP {
		get {
			return 200;
		}
	}
	
	public Player Owner{
		get { return _owner; }
		set { _owner = value; }
	}
	
	public Vector3 BuildingPosition{
		get { return this.gameObject.transform.position; }
	}
	
	public GameObject ParentObject{
		get { return this.gameObject; }
	}
	
	#region Behaviour

	// Use this for initialization
	void Start () {
		_peasantQueue = new Queue<string>();
		_unitNumber = 0;
	}
	
	// Update is called once per frame
	void Update () {
		/*if( Input.GetButtonDown("Fire1")){
			_peasantQueue.Enqueue("peasant"+_unitNumber);
			_unitNumber++;
			//esperar o numero de segundo para criar e ai sim criar a unidade
			CreateUnit();
		}*/
	}
	
	#endregion
	
	#region IBuilding implementation
	public void ShowOptions () {
		_showGUI = true;
	}

	public IUnit CreateUnit ()
	{
		Vector3 position = new Vector3(BuildingPosition.x,0,BuildingPosition.z + 34);
		GameObject peasant = GameObject.Instantiate( _unitToCreate, 
				position, Quaternion.identity ) as GameObject;
		peasant.name = _peasantQueue.Dequeue();
		Unit peasantUnit = peasant.GetComponent<Unit>();
		//Debug.Log("Peasant is null? "+(peasantUnit == null));
		if(peasantUnit != null)
		{
			//Debug.Log("Here!!!");
			peasantUnit.Owner = Owner;
		}
		return peasantUnit;
	}

	public void OnGUI()
	{
		if (_showGUI) {

		}
	}

	public IUnit UpgradeUnit ()
	{
		return null;
	}

	#endregion
}

