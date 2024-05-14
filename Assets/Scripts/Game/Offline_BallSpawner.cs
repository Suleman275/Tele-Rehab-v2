using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offline_BallSpawner : MonoBehaviour {
    [SerializeField] GameObject ballPrefab;

    public List<GameObject> SpawnBalls(string orientation, int ballCount) {
        List<GameObject> balls = new List<GameObject>();

        float offset = 0f;

        if (orientation == "Left") {
            offset = -6.5f;
        }
        else if (orientation == "Right") {
            offset = 6.5f;
        }

        for (int i = 0; i < ballCount; i++) {
            float randomX = Random.Range(-3f, 3f);
            Vector3 spawnPosition = new Vector3(transform.position.x + randomX + offset, transform.position.y, transform.position.z);
            var ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
            ball.transform.SetParent(transform);

            balls.Add(ball);
        }

        return balls;
    }

    public void ClearChildren() {
        // Destroy all children of this GameObject
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }
    }
}