using UnityEngine;
using System.Collections;

public class boom : MonoBehaviour {
	int lifeCount = 0;
    const int explosionLifespan = 20;

	void OnEnable () {
        lifeCount = 0;
	}
	
	void FixedUpdate () {
        lifeCount++;
        if (lifeCount >= explosionLifespan) {
			this.tag = "out";
		}
	}
}
