using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class DataRecorder : MonoBehaviour {
    public static DataRecorder Instance;

    private bool isRecording;

    public List<GameObject> objsToTrack;

    public Dictionary<string, List<DataPoint>> data;
    public Dictionary<float, Astra.Joint[]> skeletonData;

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
    }

    public void StopReccording() {
        isRecording = false;
        timer = 0;
        sessionEndTime = TimeManager.Instance.timeSinceApplicationStart;
        print("Session recording ended at " + sessionEndTime);
    }

    private void ResetData() {
        foreach (var kvp in data) {
            data[kvp.Key].Clear();
        }
    }

    private void InitDataStore() {
        data = new Dictionary<string, List<DataPoint>>();
        skeletonData = new Dictionary<float, Astra.Joint[]>();

        foreach (var obj in objsToTrack) {
            string id = Guid.NewGuid().ToString();
            data.Add(id, new List<DataPoint>());

            // Add the id to the GameObject for reference during import (optional)
            SetGameObjectId(obj, id);
        }
    }

    private void SetGameObjectId(GameObject obj, string id) {
        //yield return new WaitForEndOfFrame(); // Wait for next frame to ensure components are initialized
        obj.GetComponent<TrackableObject>().objectId = id; // Add the id to the GameObject name (optional)
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

                timer = 0;
            }
        }
    }

    //public string ExportDataToJson() { //only meant to be run on the server
    //    var sessionData = new SessionDataModel(Guid.NewGuid().ToString(), sessionStartTime, sessionEndTime, UserDataManager.Instance.userEmail, UserDataManager.Instance.joinedDoctorName, data, TimeManager.Instance.currentTime.ToString("yyyy-MM-dd"));

    //    var json = JsonConvert.SerializeObject(sessionData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

    //    return json;
    //}

    //public void ImportDataFromJson(string json) {
    //    SessionDataModel importedData = JsonConvert.DeserializeObject<SessionDataModel>(json);

    //    this.sessionEndTime = importedData.sessionEndTime;
    //    this.sessionStartTime = importedData.sessionStartTime;
    //    this.data = importedData.data;

    //    print("data imported");
    //}
}