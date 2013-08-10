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
		if( Input.GetButtonDown("Fire1")){
			CreateUnit();
		}
	}
	
	#endregion
	
	#region IBuilding implementation
	public IUnit CreateUnit ()
	{
		GameObject codeInstantiatedPrefab = GameObject.Instantiate( Resources.LoadAssetAtPath("Assets/Prefabs/Peasant.prefab", typeof(GameObject)), new Vector3(10,0,10), Quaternion.identity ) as GameObject;
		codeInstantiatedPrefab.name = "teste";
		//GameObject obj = GameObject.Instantiate(testPrefab,new Vector3(10,0,10), Quaternion.identity);
		return null;
	}

	public IUnit UpgradeUnit ()
	{
		throw new NotImplementedException ();
	}
	
	public GameObject ParentObject{
		get { return this.gameObject; }
	}

	public int HP {
		get {
			return 200;
		}
	}
	#endregion
}

