using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour {
    public static RoomManager Instance;

    public RoomDataModel currentRoom;

    private void Awake() {
        Instance = this;
    }

    private void Start() {

    }

    private void Update() {
        HandleRoomPollForUpdates();
    }

    private void HandleRoomPollForUpdates() {

    }
}
