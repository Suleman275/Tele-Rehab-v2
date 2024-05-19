using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Online_BallSpawner : MonoBehaviour {
    [SerializeField] private GameObject ballPrefab;

    private int ballCount;


    //public void SpawnBalls(int ballCount) {
    //    this.ballCount = ballCount;
    //    print("spawning balls");
    //    for (int i = 0; i < ballCount; i++) {
    //        float randomX = Random.Range(-3f, 3f);
    //        Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z);
    //        var ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity).GetComponent<Online_Ball>();
    //        ball.NetworkObject.Spawn();

    //        DataRecorder.Instance.AddObjectToTrack(ball.gameObject);
    //    }
    //}

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
            //ball.transform.SetParent(transform);

            ball.GetComponent<NetworkObject>().Spawn();

            balls.Add(ball);
        }

        return balls;
    }
}
