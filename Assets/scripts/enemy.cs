using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour {
    public int hp;
    const float speed = 0.1f;
    const float outOfScreenRadius = 15f;
    const int moderateDamage = 10;

    void CheckIfMovedOffScreen() {
        if (transform.position.magnitude > outOfScreenRadius) {
            tag = "out";
        }
    }
    void MoveLeft() {
        transform.Translate(Vector3.left * speed);
    }
    void TakeDamage(int damage, string tagIfDeadOrKilled) {
        hp -= damage;
        if (hp <= 0) {
            tag = tagIfDeadOrKilled;
        }
    }
}
