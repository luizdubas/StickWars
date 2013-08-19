using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player
{
	private	List<IUnit> _units;
	private List<IBuilding> _buildings;
	private int _sticks;
	private int _circles;
	private bool _isHuman;
	
	public Player (bool isHuman)
	{		
		_isHuman = isHuman;
	}
}
