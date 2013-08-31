using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	enum MenuState { mainMenu, options, howTo, regras }
	
	public GUISkin _guiSkin;
	public float _areaX;
	public float _areaY;
	public float _areaWidth;
	public float _areaHeight;

	public Texture2D _logo;

	// Use this for initialization
	void Start () {
		Internationalization.Initialize ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		DrawMainMenu ();
	}

	void DrawMainMenu(){
		//Define a skin que será usada para montar a interface
		GUI.skin = _guiSkin;

		//Cria a matriz da GUI, isso faz com que a interface seja visualizada de maneira correta em qualquer resolução
		GUI.matrix = Matrix4x4.TRS (new Vector3(0, 0, 0), Quaternion.identity, new Vector3 (Screen.width / 1280f, Screen.height / 720f, 1));
		//Começa a area de desenho da GUI (Graphical User Interface)
		GUI.DrawTexture (new Rect (362, 3, 557, 231), _logo);
		GUILayout.BeginArea (new Rect (_areaX, _areaY, _areaWidth, _areaHeight));

		//Cria botão do Modo Amistoso
		if(GUI.Button (new Rect(0f, 0f, 504, 105), Internationalization.Get("new_game")))
		{
			//Define o que vai ocorrer quando o botão for clicado
			Application.LoadLevel("WalkScenario");
		}

		//Cria botão do Modo Amistoso
		if(GUI.Button (new Rect(0f, 140f, 504, 105), Internationalization.Get("options")))
	   	{
			//Define o que vai ocorrer quando o botão for clicado
			Application.LoadLevel("Sandbox\\Map2");
		}

		//Cria botão do Modo Amistoso
		if(GUI.Button (new Rect(0f, 300f, 504, 105), Internationalization.Get("exit")))
		{
			//Define o que vai ocorrer quando o botão for clicado
			Application.Quit();
		}
		GUILayout.EndArea ();
	}
}
