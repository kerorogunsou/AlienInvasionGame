using UnityEngine;
using System.Collections;

public class bonus_ai : MonoBehaviour {
    public GameObject targetAlien;
    const float speed = 0.2f;

    void Update() {
        if (targetAlien != null) {
            // move to alien that got bonuse
            transform.position += Vector3.Normalize(targetAlien.transform.position - transform.position) * speed;
        }
        else {
            // alien has died and bonuse must selfdestruct
            tag = "out";
        }
    }
}
