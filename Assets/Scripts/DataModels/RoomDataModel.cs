using System;
using System.Collections.Generic;

[Serializable]
public class RoomDataModel {
    public string _id;
    public bool hasPatientJoined;
    public string patientName;
    public bool hasDoctorJoined;
    public string doctorName;
    public string relayCode;
    public bool gameShouldStart;
    public int wallHeight;
    public int ballCount;
    public string exerciseType;
}