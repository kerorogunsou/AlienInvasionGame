using UnityEngine;
using System.Collections;

public class enemy : MonoBehaviour {
    public int hp;
    const float speed = 0.1f;
    const float outOfScreenRadius = 15f;
    protected const int moderateDamage = 10;

    protected void CheckIfMovedOffScreen() {
        if (transform.position.magnitude > outOfScreenRadius) {
            tag = "out";
        }
    }
    protected void MoveLeft() {
        transform.Translate(Vector3.left * speed);
    }
    protected void TakeDamage(int damage, string tagIfDeadOrKilled) {
        hp -= damage;
        if (hp <= 0) {
            tag = tagIfDeadOrKilled;
        }
    }
}
