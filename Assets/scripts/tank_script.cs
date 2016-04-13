using UnityEngine;
using System;

public class tank_script : enemy {
	public int id;
	private int reloadCounter;
    const int reloadTime = 10;
    public GameObject projectile;
    ProjectilePool projectiles = new ProjectilePool();
    public GameObject explosion;
	public GameObject fire;
	public GameObject boom;
	public delegate void shoot_delegate(Vector3 my_pos, float angle);
	public shoot_delegate shoot;

    void Start() {
        projectiles.InitPool(projectile, 100, transform.position);
    }
	void Awake () {
		gameObject.SetActive (false);
	}
    void Fire(float angle) {
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * angle), Mathf.Sin(Mathf.Deg2Rad * angle), 0);
        float len = GetComponent<CircleCollider2D>().radius + projectiles[0].GetComponent<CircleCollider2D>().radius + 0.1f;
        int i = projectiles.InstantiateIfFree("shot", transform.position + direction * len, Quaternion.AngleAxis(angle, Vector3.forward);
    }
	void FixedUpdate () {
        projectiles.CheckDeadsInPool(transform.position);
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
            Fire(shootingDirection);
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
