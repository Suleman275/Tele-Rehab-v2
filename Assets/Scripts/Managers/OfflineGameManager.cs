using System;
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
    private int ballDroppedCount;
    private int wallHeight;
    private string handChoice;

    private GameObject player;

    public Action<int, int> OnBallCompleted;
    public Action OnGameCompleted;

    private void Awake() {
        Instance = this;
    }

    public void StartGame(string handChoice, int totalNumberOfBalls, int wallHeight) {
        DataRecorder.Instance.objsToTrack.Clear(); // doing manually for now

        this.totalNumberOfBalls = totalNumberOfBalls;
        this.wallHeight = wallHeight;
        this.handChoice = handChoice;

        gameEnv.SetActive(true);
        var spawnedBalls = ballSpawner.SpawnBalls(handChoice, totalNumberOfBalls);
        middleWall.SetWallHeight(wallHeight);

        if (handChoice == "Left") {
            rightTrigger.SetActive(true);
        }
        else if (handChoice == "Right") {
            leftTrigger.SetActive(true);
        }

        foreach (var ball in spawnedBalls) {
            DataRecorder.Instance.AddObjectToTrack(ball);
        }

        if (player == null) {
            player = Instantiate(playerPrefab);
        }

        var playerController = player.GetComponent<Offline_Player>();

        DataRecorder.Instance.AddObjectToTrack(playerController.leftHand);
        DataRecorder.Instance.AddObjectToTrack(playerController.rightHand);

        DataRecorder.Instance.StartReccording();
    }

    public void BallCompleted() {
        numCompletedBalls++;

        if (numCompletedBalls == totalNumberOfBalls) {
            OnGameCompleted?.Invoke();

            DataRecorder.Instance.StopReccording();

            SaveSessionData();
        }
        else {
            OnBallCompleted?.Invoke(numCompletedBalls, totalNumberOfBalls);
        }
    }

    public void BallDropped() {
        ballDroppedCount++;
    }

    public void StopGame() {
        Destroy(player);
        player = null;

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
        ballDroppedCount = 0;

        StartGame(handChoice, totalNumberOfBalls, wallHeight); //can clear balls before game restart
    }

    private void SaveSessionData() {
        var sessionData = new SessionDataModel(
            Guid.NewGuid().ToString(),
            DataRecorder.Instance.sessionStartTime,
            DataRecorder.Instance.sessionEndTime, 
            UserDataManager.Instance.userEmail,
            "",
            "Offline",
            handChoice,
            wallHeight,
            ballDroppedCount,
            DataRecorder.Instance.data,
            DataRecorder.Instance.skeletonData);

        APIManager.Instance.TryPostSessionData(sessionData);
    }
}