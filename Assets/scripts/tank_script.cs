using UnityEngine;
using System;

public class tank_script : enemy {
	public int id;
	private int reloadCounter;
    const int reloadTime = 10;

	public GameObject explosion;
	public GameObject fire;
	public GameObject boom;
	public delegate void shoot_delegate(Vector3 my_pos, float angle);
	public shoot_delegate shoot;

	void Awake () {
		gameObject.SetActive (false);
	}

	void FixedUpdate () {
        CheckIfMovedOffScreen();
        MoveLeft();
        ShootOrReload();
	}

    void ShootOrReload() {
        reloadCounter++;
        if (reloadCounter > reloadTime) {
            float shootingDirection;
            if (tag == "tank") {
                shootingDirection = Random.Range(10f, 170f);
            }
            else if (tag == "plane") {
                shootingDirection = Random.Range(0f, 360f);
            }
            else if (tag == "bomber") {
                shootingDirection = -90;
            }
            shoot(transform.position, shootingDirection);
            reloadCounter = 0;
		}
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "laser_damage") {
            TakeDamage(moderateDamage, "killed");
        }
        else if (other.gameObject.tag == "shot") {
            TakeDamage(moderateDamage, "dead");
        }
        else if ((tag == "bomber" || tag == "plane" ) && other.gameObject.tag == "building") {
            TakeDamage(hp, "dead");
            other.gameObject.tag = "dead";
        }
	}

}
