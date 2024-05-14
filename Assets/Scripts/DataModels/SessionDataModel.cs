using System;
using System.Collections.Generic;

[Serializable]
public class SessionDataModel {
    public string _id;
    public float sessionStartTime;
    public float sessionEndTime;
    public string patientName;
    public string doctorName;
    public string sessionType;
    public string exerciseType;
    public int wallHeight;
    public Dictionary<string, List<DataPoint>> data;
    public Dictionary<float, Astra.Joint[]> skeletonData;

    public SessionDataModel(string _id, float sessionStartTime, float sessionEndTime, string patientName, string doctorName, string sessionType, string exerciseType, int wallHeight, Dictionary<string, List<DataPoint>> data, Dictionary<float, Astra.Joint[]> skeletonData) {
        this._id = _id;
        this.sessionStartTime = sessionStartTime;
        this.sessionEndTime = sessionEndTime;
        this.patientName = patientName;
        this.doctorName = doctorName;
        this.sessionType = sessionType;
        this.exerciseType = exerciseType;
        this.wallHeight = wallHeight;
        this.data = data;
        this.skeletonData = skeletonData;
    }
}
