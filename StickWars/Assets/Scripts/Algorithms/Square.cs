using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public enum SquareContent
{
    Empty,
    Start,
    Target,
    Wall
};

public class Square{
	private bool _isPath = false;
	
	private int _distanceSteps = 10000;
	private int x;
	private int y;
	
	private SquareContent _contentCode = SquareContent.Empty;
	
	private Vector2 _center = Vector2.zero;
	
	private GameObject obstacle;

	
	public Square(int x, int y, Vector2 _center){
		this.x = x;
		this.y = y;
		this._center = _center;
	}
    
	public SquareContent ContentCode{
        get { return _contentCode; }
        set { _contentCode = value; }
    }
    
    public int DistanceSteps{
        get { return _distanceSteps; }
        set { _distanceSteps = value; }
    }

    public bool IsPath{
        get { return _isPath; }
        set { _isPath = value; }
    }
	
	public Vector2 Center{
        get { return _center; }
        set { _center = value; }
    }
	
	public int X{
		get { return x; }
		set { x = value; }
	}
	
	public int Y{
		get { return y; }
		set { y = value; }
	}

	public GameObject Obstacle {
		get {
			return this.obstacle;
		}
		set {
			obstacle = value;
		}
	}
}