using UnityEngine;
using System.Collections;

public class bullet_script : MonoBehaviour {

	const float laserSpeed = 0.5f;
    const float bulletSpeed = 0.5f;
    const float outOfScreenRadius = 50f;

    void Awake () {
		gameObject.SetActive (false);
	}

	void FixedUpdate () {
        Vector3 direction = new Vector3(Mathf.Cos(Mathf.Deg2Rad * transform.rotation.z), Mathf.Sin(Mathf.Deg2Rad * transform.rotation.z), 0);
            
        if(tag == "laser") {
           transform.Translate(direction * laserSpeed);
        }
        else {
            transform.Translate(direction * bulletSpeed);
        }
        if (transform.position.magnitude > outOfScreenRadius) {
			tag = "out";
		}
	}
	void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag == ("reflective_shield")) {
            ReflectFromObject(other.transform.position);
        }
	}
    void ReflectFromObject(Vector3 objectPos) {
        Vector2 direction = new Vector2(Mathf.Cos(Mathf.Deg2Rad * transform.rotation.z), Mathf.Sin(Mathf.Deg2Rad * transform.rotation.z));
        Vector2 newDirection = Vector2.Reflect(direction, transform.position - objectPos);
        transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan(newDirection.y / newDirection.x), Vector3.forward);
    }
}
