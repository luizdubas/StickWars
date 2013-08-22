using System;
using UnityEngine;
using System.Collections.Generic;

public class MatchController : MonoBehaviour{	

	private List<Player> _players;
	private Vector2 screenSelectionStartPoint;
	private Vector2 screenSelectionEndPoint;
	private Vector3 sceneSelectionStartPoint;
	private Vector3 sceneSelectionEndPoint;
	private Player _controlledPlayer;
	private List<IUnit> _selectedUnits = new List<IUnit>();
	private List<RaycastHit> _selectedUnitsToSave  = new List<RaycastHit>();
	private bool _multiSelection = false;
	private bool _selectionEnded = false;
	private int _minMouseDrag = 10;
	
	public GUISkin guiSkin;
	
	public MatchController (){
	}

	#region Behaviour

	// Use this for initialization
	void Start () {
		_players = new List<Player> ();
		Player human = new Player (true, true);
		Player computer = new Player (false, false);
		human._stickColor = new Color(0,0,0.3f);
		computer._stickColor = new Color(0.3f,0,0);
		_controlledPlayer = human;
		_players.Add (human);
		_players.Add (computer);
		Transform selectionBox = transform.Find("Selection Box");
		_controlledPlayer.Start (selectionBox);
	}
	
	// Update is called once per frame
	void Update () {
		_controlledPlayer.Update ();
		
	}
	
	void OnGUI() {
		GUI.skin = guiSkin;
		_controlledPlayer.OnGUI (guiSkin);
	}
	
	#endregion

	public List<IUnit> SelectedUnits{
		get { return _controlledPlayer.SelectedUnits; }
	}

	public Player ControlledPlayer{
		get { return _controlledPlayer; }
	}

}

