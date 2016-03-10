using UnityEngine;
using System.Collections;

public class startup_script : MonoBehaviour {

	public void start(){
		Application.LoadLevel (1);
	}
	public void exit(){
		Application.Quit();
	}
}
