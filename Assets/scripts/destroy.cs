using UnityEngine;
using System.Collections;

public class destroy : MonoBehaviour {
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void OnCollisionExit2D(Collision2D other){
		Destroy (other.gameObject);
		Debug.Log ("fired");
	}
}
