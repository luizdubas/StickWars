using UnityEngine;
using System.Collections;
using Pathfinding;

public class GhostBuilding : MonoBehaviour {

	public Texture2D _greenTexture;
	public Texture2D _redTexture;
	public int _numberRaysX = 50;
	public int _numberRaysZ = 50;
	public GameObject _temporaryObject;
	public GameObject _objectToConstruct;
	public Player _owner;
	private float _timer = 0;
	private GameObject _temporary;
	private bool _canConstruct = true;
	private bool _isCreating = false;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(!_isCreating){
			if(Input.GetKeyDown(KeyCode.Escape)){
				GameObject.Destroy (this.gameObject);
			}

			Ray rayCursor = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hitCursor;
			if (Physics.Raycast (rayCursor, out hitCursor, Mathf.Infinity, 1 << (int)LayerConstants.GROUND)) {
				this.transform.position = new Vector3 (hitCursor.point.x, 7, hitCursor.point.z);
			}

			_canConstruct = this.CanConstruct () && _owner.CheckBuildingCost(_objectToConstruct.GetComponent<AbstractBuilding>());

			if(!_canConstruct){
				this.renderer.material.mainTexture = _redTexture;
			}else{
				this.renderer.material.mainTexture = _greenTexture;
				if( Input.GetButton("Fire1") ){
					_owner.ApplyBuildingCost (_objectToConstruct.GetComponent<AbstractBuilding> ());
					_timer = _objectToConstruct.GetComponent<AbstractBuilding> ().SecondsToCreate;
					float scaleZ = this.transform.localScale.x / 2;
					float positionY = scaleZ * 5.25f;
					Vector3 tempPosition = new Vector3 (this.transform.position.x, positionY, this.transform.position.z);
					_temporary = GameObject.Instantiate (_temporaryObject, tempPosition, Quaternion.Euler(new Vector3(270,0,0))) as GameObject;
					_temporary.transform.localScale = new Vector3 (this.transform.localScale.x, this.transform.localScale.y, scaleZ);
					
					ApplyGameObjectInPathFind( _temporary );
					
					this.transform.localScale = Vector3.zero;
					_isCreating = true;
				}
			}
		}else{
			
			_timer -= Time.fixedDeltaTime;
			float percent = 1 - _timer;
			if(_timer <= 0){
				GameObject createdObject = GameObject.Instantiate (_objectToConstruct, this.transform.position, Quaternion.Euler(new Vector3(270,0,0))) as GameObject;
				
				ApplyGameObjectInPathFind( createdObject );
				
				if (createdObject.GetComponent<AbstractBuilding> () != null) {
					_owner.AddBuilding (createdObject.GetComponent<AbstractBuilding> ());
					createdObject.GetComponent<AbstractBuilding> ().Owner = _owner;
				}
				GameObject.Destroy (_temporary);
				GameObject.Destroy (this.gameObject);
			}
		}
	}

	bool CanConstruct(){
		float startPositionX = this.transform.position.x - (this.renderer.bounds.size.x / 2);
		float startPositionZ = this.transform.position.z - (this.renderer.bounds.size.z / 2);
		float stepX = this.renderer.bounds.size.x / _numberRaysX;
		float stepZ = this.renderer.bounds.size.z / _numberRaysZ;
		int groundMask = 1 << (int)LayerConstants.GROUND;
		int buildingMask = 1 << (int)LayerConstants.BUILDINGS;
		int resourceMask = 1 << (int)LayerConstants.RESOURCES;
		for (int i = 0; i <= _numberRaysZ; i++) {
			float zPosition = startPositionZ + (stepZ * i);
			for (int x = 0; x <= _numberRaysX; x++) {
				float xPosition = startPositionX + (stepX * x);
				Ray ray = new Ray (new Vector3 (xPosition, 100, zPosition), Vector3.down);
				RaycastHit rayhit;
				Debug.DrawRay (new Vector3 (xPosition, 100, zPosition), new Vector3(0,-10,0));
				if (Physics.Raycast (ray, out rayhit, 200, groundMask | buildingMask | resourceMask) ) {
					if(rayhit.transform.tag.ToUpper() != "GROUND")
						return false;
				}else{
					return false;
				}
			}
		}
		return true;
	}
	
	void ApplyGameObjectInPathFind( GameObject go ){
		Bounds b = go.collider.bounds;
		
		float x = b.size.x;
		float y = b.size.y;
				
		//4 e apenas uma borda.
		Bounds newBounds = new Bounds( b.center, new Vector3( x + 4.0f, y + 4.0f, b.size.z ) );
			
		GraphUpdateObject guo = new GraphUpdateObject( newBounds );
		AstarPath.active.UpdateGraphs (guo);
		AstarPath.active.FlushGraphUpdates();
	}
}
