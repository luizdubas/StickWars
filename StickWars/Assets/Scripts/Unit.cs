using UnityEngine;
using System;

namespace AssemblyCSharp
{
	public class Unit : MonoBehaviour, IUnit
	{
		public static int UNIT_ID = 0;
		int _id;
		int _hp;
		IUnitClass _unitClass;
		
		public int ID {
			get {
				return _hp;
			}
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
		#endregion
	}
}

