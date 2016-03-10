using UnityEngine;
using System.Collections;

public class rotate_background : MonoBehaviour {
	int ctr = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		ctr++;
		if(ctr > 2){
			gameObject.transform.RotateAround(Vector3.up, 0.01f);
			ctr = 0;
		}
	}
}
