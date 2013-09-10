using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	public GUISkin _skin;
	public Texture2D _header;
	private bool _showResources = false;
	private bool _showConstruction = false;
	private bool _showConstructionOptions = false;
	private MatchController _controller;
	private Player _owner;

	// Use this for initialization
	void Start () {
		_controller = GameObject.Find ("Player").GetComponent<MatchController> ();
		_owner = _controller.ControlledPlayer;
	}
	
	// Update is called once per frame
	void Update () {
		_showConstructionOptions = _owner.SelectedBuilding != null;
	}

	void OnGUI(){
		GUI.skin = _skin;
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 720f, 1));
		float positionY = 0;
		if (!_showConstruction)
			positionY = -60;
		if (_showConstructionOptions) {
			positionY = 0;
			_showConstruction = false;
		}
		bool buttonPressed = false;

		Rect gameHeaderRect = new Rect (600, positionY, 588, 74);

		if (!_showConstructionOptions) {
			_owner.MouseOnGUI = false;
			if (GUI.Button (new Rect (648, positionY + 3, 64, 43), "", _skin.GetStyle ("AddPeasantHouse"))) {
					_controller.ControlledPlayer.ShowGhostBuilding (0);
					buttonPressed = true;
			}
			if (GUI.Button (new Rect (728, positionY + 3, 41, 43), "", _skin.GetStyle ("AddBowTrainingGround"))) {
					_controller.ControlledPlayer.ShowGhostBuilding (1);
					buttonPressed = true;
			}
			if(GUI.Button (gameHeaderRect,"",_skin.GetStyle("GameHeader"))){
				if(!buttonPressed)
					_showConstruction = !_showConstruction;
			}
		}else{
			_owner.SelectedBuilding.DrawGUI ();
			GUI.Box (gameHeaderRect,"", _skin.GetStyle ("GameHeader"));
			Vector2 mousePosition = Input.mousePosition;
			mousePosition.y = Screen.height - mousePosition.y;
			_owner.MouseOnGUI = gameHeaderRect.Contains (mousePosition);
		}
		GUI.Box (new Rect (1, 1, 330, 43), "", _skin.GetStyle ("ResourcesHeader"));
		GUI.Label (new Rect (21, 5, 120, 45), _owner.Sticks.ToString());
		GUI.Label (new Rect (211, 5, 120, 45), _owner.Circles.ToString());
	}
}
