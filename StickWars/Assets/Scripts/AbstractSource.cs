using UnityEngine;
using System.Collections;

public class AbstractSource : MonoBehaviour, IMaterialSource {
	public int _amount = 1;
	public int _squareSize = 1;
	public MaterialType _type;
	public AnimationType _animation;
	
	#region Behaviour

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	#endregion
	
	#region IMaterialSource implementation
	
	public void CollectSource (ref int quantity)
	{
		if(_amount - quantity < 0)
			quantity = _amount;
		_amount -= quantity;
	}
	
	#endregion
}
