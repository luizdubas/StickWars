using UnityEngine;
using System.Collections.Generic;

public class PeasantHouse : MonoBehaviour, IBuilding 
{
	int _unitNumber;
	Queue<string> _peasantQueue;
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
		if( Input.GetButtonDown("Fire1")){
			_peasantQueue.Enqueue("peasant"+_unitNumber);
			_unitNumber++;
			//esperar o numero de segundo para criar e ai sim criar a unidade
			CreateUnit();
		}
	}
	
	#endregion
	
	#region IBuilding implementation
	public IUnit CreateUnit ()
	{
		Vector3 position = new Vector3(BuildingPosition.x,0,BuildingPosition.z + 14);
		
		GameObject peasant = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/Prefabs/Peasant.prefab", typeof(GameObject)), 
				position, Quaternion.identity ) as GameObject;
		peasant.name = _peasantQueue.Dequeue();
		Unit peasantUnit = peasant.GetComponent<Unit>();
		if(peasantUnit != null)
		{
			Debug.Log("Here!!!");
			peasantUnit.Owner = Owner;
		}
		return peasantUnit;
	}

	public IUnit UpgradeUnit ()
	{
		return null;
	}
	#endregion
}

