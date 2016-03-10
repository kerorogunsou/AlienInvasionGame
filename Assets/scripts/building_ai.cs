using UnityEngine;
using System.Collections;

public class building_ai : enemy {
    const float speed = 0.04f;

    void Awake() {
        gameObject.SetActive(false);
    }
	
	void FixedUpdate () {
        CheckIfMovedOffScreen();
        MoveLeft();
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "laser_damage") {
            TakeDamage(moderateDamage, "killed");
        }
        else if (other.gameObject.tag == "shot") {
            TakeDamage(moderateDamage, "dead");
        }
        else if(other.gameObject.tag == "bomber" || other.gameObject.tag == "plane") {
            // planes modify tags for buildings they kill, so building leaves it the way it is
            TakeDamage(hp, tag);
        }
    }
}
