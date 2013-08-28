using System;
using UnityEngine;

public class AbstractBuilding : MonoBehaviour, IBuilding
{
	public AbstractBuilding ()
	{
	}

	#region IBuilding implementation

	public virtual IUnit CreateUnit ()
	{
		throw new NotImplementedException ();
	}

	public virtual IUnit UpgradeUnit ()
	{
		throw new NotImplementedException ();
	}

	public virtual void ShowOptions ()
	{
		throw new NotImplementedException ();
	}

	public virtual int HP {
		get {
			throw new NotImplementedException ();
		}
	}

	public virtual Player Owner {
		get {
			throw new NotImplementedException ();
		}
		set {
			throw new NotImplementedException ();
		}
	}

	public virtual Vector3 BuildingPosition {
		get {
			throw new NotImplementedException ();
		}
	}

	public virtual GameObject ParentObject {
		get {
			throw new NotImplementedException ();
		}
	}

	#endregion
}

