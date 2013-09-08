using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	public GUISkin _skin;
	public Texture2D _header;
	private bool _showResources = false;
	private bool _showConstruction = false;
	private bool _showConstructionOptions = false;
	private MatchController _controller;

	// Use this for initialization
	void Start () {
		_controller = GameObject.Find ("Player").GetComponent<MatchController> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.skin = _skin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 720f, 1));
		float positionY = 0;
		if (!_showConstruction)
			positionY = -60;
		
		bool buttonPressed = false;
		if(GUI.Button (new Rect (648, positionY + 3, 64, 43),"",_skin.GetStyle("AddPeasantHouse"))){
			_controller.ControlledPlayer.ShowGhostBuilding(0);
			buttonPressed = true;
		}
		if(GUI.Button (new Rect (728, positionY + 3, 41, 43),"",_skin.GetStyle("AddBowTrainingGround"))){
			_controller.ControlledPlayer.ShowGhostBuilding(1);
			buttonPressed = true;
		}
		if(GUI.Button (new Rect (600, positionY, 588, 74),"",_skin.GetStyle("GameHeader"))){
			if(!buttonPressed)
				_showConstruction = !_showConstruction;
		}
		/*GUI.Label (new Rect (52, 8 + positionY, 120, 40), _controller.ControlledPlayer.Sticks.ToString());
		GUI.Label (new Rect (242, 8 + positionY, 120, 40), _controller.ControlledPlayer.Circles.ToString());
		GUI.Label (new Rect (432, 8 + positionY, 120, 40), _controller.ControlledPlayer.Gold.ToString());*/
	}
}
