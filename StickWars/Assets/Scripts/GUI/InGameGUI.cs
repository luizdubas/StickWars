using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	public Texture2D _header;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 720f, 1));
		GUI.DrawTexture (new Rect (0, 0, 588, 60), _header);
	}
}
