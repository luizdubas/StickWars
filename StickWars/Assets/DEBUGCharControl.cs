using UnityEngine;
using System.Collections;

public class DEBUGCharControl : MonoBehaviour {
	private GameObject stickChar;
	private bool moving;
	private float rotation = 0;
	private Vector3 move = Vector3.zero;
	
	// Use this for initialization
	void Start () {
		stickChar = GameObject.Find("peasant");
		if(stickChar == null)
			Debug.Log("STICK CHAR NULL");
	}
	
	// Update is called once per frame
	void Update () {
		#region input robo
		if(Input.GetKeyDown("down"))
		{
			Debug.Log("Current rotation = "+stickChar.transform.eulerAngles.y);
			moving = true;
			move = new Vector3(0,0,8);
			rotation = -stickChar.transform.eulerAngles.y;
			stickChar.transform.Rotate(new Vector3(0,rotation,0));
			Debug.Log("New rotation = "+rotation);
			stickChar.animation.Play("Walk");
			stickChar.animation.wrapMode = WrapMode.Loop;
		}
		if(Input.GetKeyDown("up"))
		{
			Debug.Log("Current rotation = "+stickChar.transform.eulerAngles.y);
			moving = true;
			move = new Vector3(0,0,-8);
			if(rotation != 180)
				rotation = (stickChar.transform.eulerAngles.y - 180) * -1;
			else 
				rotation = 0f;
			Debug.Log("New rotation = "+rotation);
			stickChar.transform.Rotate(new Vector3(0,rotation,0));
			stickChar.animation.Play("Walk");
			stickChar.animation.wrapMode = WrapMode.Loop;
			
		}
		if(Input.GetKeyDown("right"))
		{
			Debug.Log("Current rotation = "+stickChar.transform.eulerAngles.y);
			moving = true;
			move = new Vector3(-8,0,0);
			if(rotation != 270)
				rotation = (stickChar.transform.eulerAngles.y - 270) * -1;
			else 
				rotation = 0f;
			stickChar.transform.Rotate(new Vector3(0,rotation,0));
			Debug.Log("New rotation = "+rotation);
			stickChar.animation.Play("Walk");
			stickChar.animation.wrapMode = WrapMode.Loop;
		}
		if(Input.GetKeyDown("left"))
		{
			Debug.Log("Current rotation = "+stickChar.transform.eulerAngles.y);
			moving = true;
			move = new Vector3(8,0,0);
			if(rotation != 90)
				rotation = (stickChar.transform.eulerAngles.y - 90) * -1;
			else 
				rotation = 0f;
			stickChar.transform.Rotate(new Vector3(0,rotation,0));
			Debug.Log("New rotation = "+rotation);
			stickChar.animation.Play("Walk");
			stickChar.animation.wrapMode = WrapMode.Loop;
		}
		
		if(Input.GetKeyUp("right") || Input.GetKeyUp("up") || Input.GetKeyUp("down") || Input.GetKeyUp("left")){
			moving = false;
			stickChar.animation.Blend("Rest");
			stickChar.animation.Stop("Walk");
		}
		if(moving)
			stickChar.transform.position += move * Time.deltaTime;
		#endregion
		
		/*if(Input.GetKeyDown(KeyCode.Alpha5)){
			Camera.main.transform.position = new Vector3(0,1,-5.5f);
			Camera.main.transform.eulerAngles = new Vector3(0,0,0);
		}
		if(Input.GetKeyDown(KeyCode.Alpha0)){
			Camera.main.transform.position = new Vector3(-15,5,0);
			Camera.main.transform.eulerAngles = new Vector3(25,90,0);
		}*/
	}
}
