using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ReplayManager : MonoBehaviour {
    [SerializeField] GameObject leftHandGhostPrefab;
    [SerializeField] GameObject rightHandGhostPrefab;
    [SerializeField] GameObject ballPrefab;
    [SerializeField] GameObject gameEnv;
    [SerializeField] MiddleWall wall;

    public static ReplayManager Instance;

    private bool isReplaying;
    private bool isPasued;

    private int index1;
    private int index2;

    private float timeValue;

    private Dictionary<string, GameObject> trackedObjects;
    private SessionDataModel gameData;

    public Action OnReplayStarted;
    public Action OnReplayFinished;
    public Action OnReplayStopped;
    public Action<string> OnReplayError;


    private void Awake() {
        Instance = this;

        trackedObjects = new Dictionary<string, GameObject>();
        
        isPasued = false;
    }

    public void SetSessionData(SessionDataModel sessionData) {
        this.gameData = sessionData;
    }

    public void StartReplay() {
        if (gameData == null) {
            OnReplayError?.Invoke("Session data has not been set");
        }

        gameEnv.SetActive(true);
        wall.SetWallHeight(gameData.wallHeight);

        isReplaying = true;

        timeValue = gameData.sessionStartTime - 1f;
        print("Starting replay from " + timeValue);
    }

    public void PauseReplay() {
        isPasued = true;
    }

    public void UnPauseReplay() {
        isPasued = false;
    }

    public void TogglePause() { 
        isPasued = !isPasued;
        print("is paused: " + isPasued);
    }

    public void StopReplay() {
        gameEnv.SetActive(false);
        wall.SetWallHeight(0);

        isReplaying = false;

        foreach (var kvp in trackedObjects) {
            Destroy(kvp.Value);
        }

        trackedObjects.Clear();
    }

    public void Update() {
        if (isReplaying && !isPasued) {
            timeValue += Time.unscaledDeltaTime;

            GetIndex();
            SetTransforms();

            if (timeValue > gameData.sessionEndTime) {
                OnReplayFinished?.Invoke();
            }
        }
    }

    private void GetIndex() {
        var objData = GetFirstObjectData();

        for (int i = 0; i < objData.Count - 2; i++) {
            if (objData[i].timeStamp == timeValue) {
                index1 = i;
                index2 = i;
                return;
            }
            else if (objData[i].timeStamp < timeValue && timeValue < objData[i + 1].timeStamp) {
                index1 = i;
                index2 = i + 1;
                return;
            }
        }

        index1 = objData.Count - 1;
        index2 = objData.Count - 1;
    }

    private List<DataPoint> GetFirstObjectData() {
        return gameData.data.Values.ToList()[0];
    }


    private void SetTransforms() {
        foreach (KeyValuePair<string, List<DataPoint>> kvp in gameData.data) {

            if (!trackedObjects.ContainsKey(kvp.Key)) {
                print("Creating new " + kvp.Value[0].objectName);
                // Instantiate the object only if it's not already tracked
                switch (kvp.Value[0].objectName) {
                    case "Left_Hand":
                        trackedObjects[kvp.Key] = Instantiate(leftHandGhostPrefab);
                        break;
                    case "Right_Hand":
                        trackedObjects[kvp.Key] = Instantiate(rightHandGhostPrefab);
                        break;
                    case "Online_Ball(Clone)":
                        trackedObjects[kvp.Key] = Instantiate(ballPrefab);
                        break;
                    
                    case "Offline_Ball(Clone)":
                        trackedObjects[kvp.Key] = Instantiate(ballPrefab);
                        break;
                }
            }

            var trackedObject = trackedObjects[kvp.Key];

            if (index1 == index2) {
                trackedObject.transform.position = gameData.data[kvp.Key][index1].position;
            }
            else {
                float interpolationFactor = (timeValue - gameData.data[kvp.Key][index1].timeStamp) / (gameData.data[kvp.Key][index2].timeStamp - gameData.data[kvp.Key][index1].timeStamp);

                trackedObject.transform.position = Vector3.Lerp(gameData.data[kvp.Key][index1].position, gameData.data[kvp.Key][index2].position, interpolationFactor);
            }
        }
    }
}
