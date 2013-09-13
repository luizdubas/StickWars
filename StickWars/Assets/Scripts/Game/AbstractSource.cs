using UnityEngine;
using System.Collections;

/// <summary>
/// Fonte de material abstrata
/// </summary>
public class AbstractSource : MonoBehaviour, IMaterialSource {
	public int _amount = 1;
	public MaterialType _type;
	public AnimationType _animation;
	
	#region Behaviour

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(_amount == 0){
			GameObject.DestroyImmediate (this.gameObject);
		}
	}
	
	#endregion
	
	#region IMaterialSource implementation
	
	public void CollectSource (ref int quantity)
	{
		//Nao pode permitir que dois camponeses consigam retirar recurso ao mesmo tempo
		//Senao um deles poderia fazer o recurso ficar negativo
		lock (this) {
			if (_amount - quantity < 0)
					quantity = _amount;
			_amount -= quantity;
		}
	}
	
	#endregion
}
