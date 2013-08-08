using UnityEngine;
using System;

public class PeasantHouse : MonoBehaviour, IBuilding 
{
	public GameObject _unitToCreate;

	#region Behaviour

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	#endregion
	
	#region IBuilding implementation
	public IUnit CreateUnit ()
	{
		throw new NotImplementedException ();
	}

	public IUnit UpgradeUnit ()
	{
		throw new NotImplementedException ();
	}

	public int HP {
		get {
			return 200;
		}
	}
	#endregion
}

