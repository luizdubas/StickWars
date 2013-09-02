using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	public GUISkin _skin;
	public Texture2D _header;
	private bool _showResources = false;
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
		if (!_showResources)
			positionY = -60;
		if(GUI.Button (new Rect (0, positionY, 588, 74),"",_skin.GetStyle("GameHeader"))){
			_showResources = !_showResources;
		}
		GUI.Label (new Rect (52, 8 + positionY, 120, 40), _controller.ControlledPlayer.Sticks.ToString());
		GUI.Label (new Rect (242, 8 + positionY, 120, 40), _controller.ControlledPlayer.Circles.ToString());
		GUI.Label (new Rect (432, 8 + positionY, 120, 40), _controller.ControlledPlayer.Gold.ToString());
	}
}
