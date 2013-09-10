using System;
using UnityEngine;
using System.Collections.Generic;

public class MatchController : MonoBehaviour{	

	private List<Player> _players;
	private Player _controlledPlayer;
	
	public GUISkin guiSkin;
	public GameObject[] _ghostBuildings;
	public GameObject _grid;
	
	public MatchController (){
	}

	#region Behaviour

	// Use this for initialization
	void Start () {
		_players = new List<Player> ();
		Dictionary<MaterialType,int> startingQuantity = new Dictionary<MaterialType, int> ();
		startingQuantity.Add (MaterialType.Circle, 50);
		startingQuantity.Add (MaterialType.Stick, 100);
		startingQuantity.Add (MaterialType.Gold, 0);
		Player human = new Player (true, true, startingQuantity);
		Player computer = new Player (false, false, startingQuantity);
		human._stickColor = new Color(0,0,0.3f);
		computer._stickColor = new Color(0.3f,0,0);
		_controlledPlayer = human;
		_players.Add (human);
		_players.Add (computer);
		Transform selectionBox = transform.Find("Selection Box");
		_controlledPlayer.Start (selectionBox, _grid, ref _ghostBuildings);
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

