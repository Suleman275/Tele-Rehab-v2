using System;
using UnityEngine;

[Serializable]
public class DataPoint {
    public string objectId;
    public string objectName;
    public float timeStamp;
    public Vector3 position;

    public DataPoint(string objectId, string objectName, float timeStamp, Vector3 position) {
        this.objectId = objectId;
        this.objectName = objectName;
        this.timeStamp = timeStamp;
        this.position = position;
    }

    public override string ToString() {
        return $"{objectName} = Timestamp: {timeStamp}; Position: ({position.x}, {position.y}, {position.z})";
    }
}