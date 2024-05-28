using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class OnlineGameManager : NetworkBehaviour {
    [SerializeField] GameObject gameEnv;
    [SerializeField] MiddleWall wall;
    [SerializeField] Online_BallSpawner ballSpawner;
    [SerializeField] GameObject leftTrigger;
    [SerializeField] GameObject rightTrigger;

    public static OnlineGameManager Instance;

    List<ulong> connectedClients;
    RoomDataModel roomData;
    int completedBalls;
    int ballDroppedCount;
    List<GameObject> spawnedBalls;
    bool hasGameStarted;
    bool isGameComplete;

    public Action<int, int> OnBallCompleted;
    public Action OnGameCompleted;
    public Action OnGameStarted;


    private void Awake() {
        Instance = this;
    }

    private void Start() {
        connectedClients = new List<ulong>();
        completedBalls = 0;
        ballDroppedCount = 0;
        hasGameStarted = false;
        isGameComplete = false;

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }

    private void OnClientConnected(ulong obj) {
        if (IsServer) {
            connectedClients.Add(obj);

            print("total connected clients: " + connectedClients.Count);
        }
    }

    private void Update() {
        HandleGameStart();
    }

    private void HandleGameStart() {
        if (!IsServer) {
            return;
        }

        if (connectedClients.Count == 2 && !hasGameStarted && !isGameComplete) {
            hasGameStarted = true;
            StartGameClientRPC();
            GetGameDataFromRoomClientRPC();
            ActivateGameEnvClientRpc();
            SetWallHeightClientRPC(roomData.wallHeight);
            SpawnBalls(roomData.exerciseType, roomData.ballCount);
            ActivateBallTriggersClientRpc(roomData.exerciseType);

            DataRecorder.Instance.StartReccording();
        }
    }

    [ClientRpc]
    private void StartGameClientRPC() {
        OnGameStarted?.Invoke();
    }

    [ServerRpc(RequireOwnership = false)]
    public void RestartGameServerRPC() {
        completedBalls = 0;
        ballDroppedCount = 0;
        foreach (var ball in spawnedBalls) {
            DataRecorder.Instance.objsToTrack.Remove(ball);
            Destroy(ball);
        }
        spawnedBalls.Clear();
        hasGameStarted = false;
        isGameComplete = false;
    }

    public void StopGame() {
        NetworkManager.Singleton.Shutdown();

        DataRecorder.Instance.objsToTrack.Clear();

        if (roomData.exerciseType == "Left") {
            rightTrigger.SetActive(false);
        }
        else if (roomData.exerciseType == "Right") {
            leftTrigger.SetActive(false);
        }

        wall.SetWallHeight(0);

        if (IsServer) {
            foreach (var ball in spawnedBalls) { 
                Destroy(ball);
            }
        }

        gameEnv.SetActive(false);
    }

    [ClientRpc]
    private void ActivateGameEnvClientRpc() {
        gameEnv.SetActive(true);
    }

    [ClientRpc]
    private void GetGameDataFromRoomClientRPC() {
        roomData = RoomManager.Instance.currentRoom;
    }

    [ClientRpc]
    private void SetWallHeightClientRPC(int height) {
        wall.SetWallHeight(height);
    }

    [ServerRpc]
    public void BallCompletedServerRPC() {
        if (!IsServer) {
            return;
        }

        completedBalls++;

        if (completedBalls == roomData.ballCount) {
            GameCompletedClientRPC();

            isGameComplete = true;

            DataRecorder.Instance.StopReccording();

            SaveSessionData();
        }
        else {
            //OnBallCompleted?.Invoke(numCompletedBalls, totalNumberOfBalls);
            BallCompletedClientRPC(completedBalls, roomData.ballCount);
        }
        
    }

    [ClientRpc]
    private void BallCompletedClientRPC(int completedBalls, int totalBalls) {
        OnBallCompleted?.Invoke(completedBalls, totalBalls);
    }

    [ServerRpc]
    public void BallDroppedServerRPC() {
        if (IsServer) {
            ballDroppedCount++;
        }
    }

    private void SpawnBalls(string orientation, int ballCount) {
        if (IsServer) {
            spawnedBalls = ballSpawner.SpawnBalls(orientation, ballCount);

            foreach (var ball in spawnedBalls) {
                DataRecorder.Instance.AddObjectToTrack(ball);
            }
        }
    }

    [ClientRpc]
    private void ActivateBallTriggersClientRpc(string handChoice) {
        if (handChoice == "Left") {
            rightTrigger.SetActive(true);
        }
        else if (handChoice == "Right") {
            leftTrigger.SetActive(true);
        }
    }

    [ClientRpc]
    private void GameCompletedClientRPC() {
        OnGameCompleted?.Invoke();
    }

    private void SaveSessionData() {
        if (!IsServer) {
            return;
        }

        var sessionData = new SessionDataModel(
            Guid.NewGuid().ToString(),
            DataRecorder.Instance.sessionStartTime,
            DataRecorder.Instance.sessionEndTime,
            roomData.patientName,
            roomData.doctorName,
            "Online",
            roomData.exerciseType,
            roomData.wallHeight,
            ballDroppedCount,
            DataRecorder.Instance.data,
            DataRecorder.Instance.skeletonData,
            DataRecorder.Instance.emgData);

        APIManager.Instance.TryPostSessionData(sessionData);
    }
}
