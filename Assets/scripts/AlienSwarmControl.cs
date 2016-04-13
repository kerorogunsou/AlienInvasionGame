using UnityEngine;
using System.Collections.Generic; 

public class AlienSwarmControl : MonoBehaviour {
    int health = 100;
    const int moderateDamage = 10;
    bool alive = true;
    const float movementSpeed = 1f;

    Animator anim;
    public GameObject laser;
    ProjectilePool lasers = new ProjectilePool();
    bool shieldActive = false;
    GameObject shield;
    CustomizeableTimer shieldTimer = new CustomizeableTimer(500);
    bool autofire = false;
    CustomizeableTimer autofireTimer = new CustomizeableTimer(200);

    bool startedFiring = false;
    int individualFireDelay;
    bool touched = false;
    float colliderRadius;

    GameObject temp;

    void Start() {
        Color AlienTint = new Color(Random.Range(0, 1f), Random.Range(0, 1f), Random.Range(0, 1f));
        InitShield(AlienTint);
        laser.GetComponent<SpriteRenderer>().color = AlienTint;
        lasers.InitPool(laser, 100, transform.position);
        colliderRadius = GetComponentInChildren<CircleCollider2D>().radius + lasers.pool[0].GetComponent<BoxCollider2D>().size.x;

        individualFireDelay = Random.Range(0, 10);
        anim = GetComponent<Animator>();
    }
    void InitShield(Color AlienTint) {
        Transform[] children = GetComponentsInChildren<Transform>();
        for (int i = 0; i < children.Length; i++) {
            if (children[i].gameObject.tag == "reflective_shield") {
                shield = children[i].gameObject;
                break;
            }
        }
        shield.SetActive(shieldActive);
        shield.GetComponent<SpriteRenderer>().color = AlienTint;
    }
    void SpawnLaser() {
        PrepareToShootOrShootInTheAir();
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 lookPos = Camera.main.ScreenToWorldPoint(mousePos);
        lookPos = lookPos - transform.position;
        float shootingAngle = Mathf.Atan2(lookPos.y, lookPos.x) * Mathf.Rad2Deg;

        Vector3 step = new Vector3(Mathf.Cos(Mathf.Deg2Rad * shootingAngle), Mathf.Sin(Mathf.Deg2Rad * shootingAngle), 0);
        Vector3 laserPosition = transform.position + step;
        lasers.InstantiateIfFree("laser_damage", laserPosition * colliderRadius, Quaternion.AngleAxis(shootingAngle, Vector3.forward));
    }
    void PrepareToShootOrShootInTheAir() { 
        if (individualFireDelay < 0 && !startedFiring) {
            startedFiring = true;
            anim.SetBool("fire", true);
        }
        else {
            individualFireDelay--;
        }
    }
    void FixedUpdate() {
        // cleaning object pool
        lasers.CheckDeadsInPool(transform.position);
        ProcessBonuses();
        if (Input.touchCount > 0 && (!touched || autofire)) {
            SpawnLaser();
            touched = true;
        }
        else if (touched && Input.touchCount == 0) {
            touched = false;
        }
        else if ((Input.GetKey(KeyCode.Space) && autofire || Input.GetKeyDown(KeyCode.Space)) ||
                 (Input.GetKey(KeyCode.Mouse0) && autofire || Input.GetKeyDown(KeyCode.Mouse0))) {
            SpawnLaser();
        }
        if (health < 0 && alive) {
            alive = false;
            Destroy(gameObject);
            }
    }
    void ProcessBonuses() {
        if (shieldActive) {
            if (shieldTimer.Count()) {
                shieldActive = false;
                shield.SetActive(shieldActive);
            }
            else {
                if (shield.transform.position != transform.position) {
                    shield.transform.position = transform.position;
                }
            }
        }
        if (autofire) {
            if (autofireTimer.Count()) {
                autofire = false;
            }
        }
    }
    void AcceptBonuse(string bonuseTag) {
        if (bonuseTag.Contains("health")) {
            health = 100;
        }
        else if (bonuseTag.Contains("shield")) {
            shieldActive = true;
            shieldTimer.Reset();
            shield.SetActive(true);
        }
        else if (bonuseTag.Contains("autofire")) {
            autofire = true;
            autofireTimer.Reset();
        }
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "shot") {
            if (health >= 0) {
                health -= moderateDamage;
            }
        }
        else if (other.gameObject.tag.Contains("bonus")) {
            AcceptBonuse(other.gameObject.tag);
            other.gameObject.tag = "out";
        }
    }
}
