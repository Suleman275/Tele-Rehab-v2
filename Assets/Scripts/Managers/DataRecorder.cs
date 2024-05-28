using System;
using System.Collections.Generic;
using UnityEngine;

public class DataRecorder : MonoBehaviour {
    public static DataRecorder Instance;

    private bool isRecording;

    public List<GameObject> objsToTrack;

    public Dictionary<string, List<DataPoint>> data;
    public Dictionary<float, Astra.Joint[]> skeletonData;
    public Dictionary<float, ushort> emgData;

    private float timer;
    private float recordFrequency;

    public float sessionStartTime;
    public float sessionEndTime;

    private void Awake() {
        Instance = this;

        InitDataStore();

        timer = 0;
        recordFrequency = 5;
    }

    public void StartReccording() {
        InitDataStore();
        isRecording = true;
        timer = 0;
        sessionStartTime = TimeManager.Instance.timeSinceApplicationStart;
        print("Session recording started at " + sessionStartTime);
        print($"recording {objsToTrack.Count} items");
    }

    public void StopReccording() {
        isRecording = false;
        timer = 0;
        sessionEndTime = TimeManager.Instance.timeSinceApplicationStart;
        print("Session recording ended at " + sessionEndTime);
        print($"recorded {data.Count} items");
    }

    //private void ResetData() {
    //    foreach (var kvp in data) {
    //        data[kvp.Key].Clear();
    //    }
    //}

    private void InitDataStore() {
        data = new Dictionary<string, List<DataPoint>>();
        skeletonData = new Dictionary<float, Astra.Joint[]>();
        emgData = new Dictionary<float, ushort>();

        foreach (var obj in objsToTrack) {
            string id = Guid.NewGuid().ToString();
            data.Add(id, new List<DataPoint>());

            // Add the id to the GameObject for reference during import
            SetGameObjectId(obj, id);
        }
    }

    private void SetGameObjectId(GameObject obj, string id) {
        //yield return new WaitForEndOfFrame(); // Wait for next frame to ensure components are initialized
        obj.GetComponent<TrackableObject>().objectId = id; // Add the id to the GameObject name
        //print($"set {obj.name} id to {id}");
    }

    public void AddObjectToTrack(GameObject obj) {
        objsToTrack.Add(obj);

        print("tracking object: " + obj.name);
    }

    private void Update() {
        if (isRecording) {
            timer += Time.unscaledDeltaTime;

            if (timer >= 1 / recordFrequency) {
                foreach (GameObject obj in objsToTrack) {
                    string id = obj.GetComponent<TrackableObject>().objectId; // Get the id from the GameObject (optional)
                    var x = new DataPoint(id, obj.name, TimeManager.Instance.timeSinceApplicationStart, obj.transform.position);
                    data[id].Add(x);
                }

                //trying to save skeleton as well
                var body = AstraManager.Instance.GetBody(0);

                if (body != null) {
                    var joints = AstraManager.Instance.GetJointsFromBody(body);

                    if (joints != null) {
                        skeletonData.Add(TimeManager.Instance.timeSinceApplicationStart, joints);
                    }
                }

                if (EMGManager.Instance.isEMGConnected) {
                    emgData.Add(TimeManager.Instance.timeSinceApplicationStart, EMGManager.Instance.currentVal);
                }

                timer = 0;
            }
        }
    }
}