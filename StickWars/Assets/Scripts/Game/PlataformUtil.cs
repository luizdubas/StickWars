using UnityEngine;
using System.Collections;

public class PlataformUtil : MonoBehaviour{

	public static bool IsMoveButtonUp(){
		bool returnValue = false;
		switch( Application.platform ){
		case RuntimePlatform.WindowsPlayer:
			returnValue = Input.GetButtonUp ("Fire2");
			break;
		case RuntimePlatform.Android:
			returnValue = Input.GetButtonUp ("Fire1");
			break;
		case RuntimePlatform.WP8Player:
			returnValue = Input.GetButtonUp ("Fire1");
			break;
		default:
			returnValue = Input.GetButtonUp ("Fire2");
			break;
		}
	
		return returnValue;
	}
	
	public static bool IsMoveButtonDown(){
		bool returnValue = false;
		switch( Application.platform ){
		case RuntimePlatform.WindowsPlayer:
			returnValue = Input.GetButtonDown ("Fire2");
			break;
		case RuntimePlatform.Android:
			returnValue = Input.GetButtonDown ("Fire1");
			break;
		case RuntimePlatform.WP8Player:
			returnValue = Input.GetButtonDown ("Fire1");
			break;
		default:
			returnValue = Input.GetButtonDown ("Fire2");
			break;
		}
		
		return returnValue;
	}
	
}

