using System;
using System.Collections.Generic;

[Serializable]
public class RoomDataModel {
    public string _id;
    public bool hasPatientJoined;
    public bool hasDoctorJoined;
    public string relayCode;
}