using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineGameManager : MonoBehaviour {
    public static OfflineGameManager Instance;

    [SerializeField] GameObject gameEnv;
    [SerializeField] Offline_BallSpawner ballSpawner;
    [SerializeField] MiddleWall middleWall;
    [SerializeField] GameObject leftTrigger;
    [SerializeField] GameObject rightTrigger;
    [SerializeField] GameObject playerPrefab;

    private int totalNumberOfBalls;
    private int numCompletedBalls;
    private int wallHeight;
    private string handChoice;

    private GameObject player;

    //add session start and end times

    public Action<int, int> OnBallCompleted;
    public Action OnGameCompleted;

    private void Awake() {
        Instance = this;
    }

    public void StartGame(string handChoice, int totalNumberOfBalls, int wallHeight) {
        this.totalNumberOfBalls = totalNumberOfBalls;
        this.wallHeight = wallHeight;
        this.handChoice = handChoice;

        gameEnv.SetActive(true);
        ballSpawner.SpawnBalls(handChoice, totalNumberOfBalls);
        middleWall.SetWallHeight(wallHeight);

        if (handChoice == "Left") {
            rightTrigger.SetActive(true);
        }
        else if (handChoice == "Right") {
            leftTrigger.SetActive(true);
        }

        player = Instantiate(playerPrefab);
    }

    public void BallCompleted() {
        numCompletedBalls++;

        if (numCompletedBalls == totalNumberOfBalls) {
            OnGameCompleted?.Invoke();
        }
        else {
            OnBallCompleted?.Invoke(numCompletedBalls, totalNumberOfBalls);
        }
    }

    public void StopGame() {
        Destroy(player);

        if (handChoice == "Left") {
            rightTrigger.SetActive(false);
        }
        else if (handChoice == "Right") {
            leftTrigger.SetActive(false);
        }

        middleWall.SetWallHeight(0);
        ballSpawner.ClearChildren();
        gameEnv.SetActive(false);
    }

    public void RestartGame() {
        numCompletedBalls = 0;

        ballSpawner.SpawnBalls(handChoice, totalNumberOfBalls); //can clear children before game restart
    }
}