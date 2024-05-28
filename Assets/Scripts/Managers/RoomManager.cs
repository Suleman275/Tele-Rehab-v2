using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RoomManager : MonoBehaviour {
    public static RoomManager Instance;

    private float roomPollTimer;

    public RoomDataModel currentRoom;

    public Action<RoomDataModel> OnRoomJoined;
    public Action<string> OnRoomJoinError;
    
    public Action OnRoomLeft;
    public Action<string> OnRoomLeaveError;

    public Action OnPatientJoined;
    public Action OnPatientLeft;

    public Action OnDoctorJoined;
    public Action OnDoctorLeft;

    public Action<string> OnRelayCodeAdded;
    public Action<string> OnRelayCodeAddingError;

    public Action OnGameDataSet;
    public Action<string> OnSettingGameDataError;

    public Action OnGameShouldStart;

    private void Awake() {
        Instance = this;

        currentRoom = null;
    }

    private void Start() {
        UnityServicesManager.Instance.onHostStarted += TryAddRelayCode;
    }

    private void Update() {
        HandleRoomPollForUpdates();
    }

    private void HandleRoomPollForUpdates() {
        if (currentRoom != null && currentRoom._id != null) {
            roomPollTimer -= Time.deltaTime;

            if (roomPollTimer < 0) {
                roomPollTimer = 4;

                TryGetRoom(currentRoom._id);
            }
        }
    }

    public void TryJoinRoom(string roomId, string userRole) {
        //print("try join room");
        StartCoroutine(SendJoinRoomRequest(roomId, userRole));
    }

    IEnumerator SendJoinRoomRequest(string roomId, string userRole) {
        //print("send join room");
        string baseUrl = APIManager.Instance._baseUrl;

        //'/rooms/:id-:userRole-:userName-:reqType'

        string url = $"{baseUrl}/rooms/{roomId}-{userRole}-{UserDataManager.Instance.userEmail}-Join";

        using (UnityWebRequest request = UnityWebRequest.Post(url, "", "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;
                //print(responseJson);

                if (responseJson == "Room not found") {
                    OnRoomJoinError?.Invoke("Room not found");
                }
                else if (responseJson == "This user is already in the room") {
                    OnRoomJoinError?.Invoke("This user is already in the room");
                }
                else {
                    var recievedRoomData = JsonConvert.DeserializeObject<RoomDataModel>(responseJson);

                    currentRoom = recievedRoomData;

                    OnRoomJoined?.Invoke(recievedRoomData);
                }
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

                OnRoomJoinError?.Invoke(request.error);
            }
        }
    }

    public void TryLeaveRoom(string roomId, string userRole) {
        StartCoroutine(SendLeaveRoomRequest(roomId, userRole));
    }

    IEnumerator SendLeaveRoomRequest(string roomId, string userRole) {
        string baseUrl = APIManager.Instance._baseUrl;

        //'/rooms/:id-:userRole-:userName-:reqType'

        string url = $"{baseUrl}/rooms/{roomId}-{userRole}-{UserDataManager.Instance.userEmail}-Leave";

        using (UnityWebRequest request = UnityWebRequest.Post(url, "", "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;
                print(responseJson);

                if (responseJson == "This user is not in this room") {
                    OnRoomJoinError?.Invoke("This user is not in this room");
                }
                else {
                    currentRoom = null;

                    OnRoomLeft?.Invoke();

                    StopAllCoroutines();
                }
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

                OnRoomLeaveError?.Invoke(request.error);
            }
        }
    }

    private void TryGetRoom(string roomId) {
        StartCoroutine(SendGetRoomRequest(roomId));
    }

    IEnumerator SendGetRoomRequest(string roomId) {
        //print($"Trying to get room");
        string url = $"{APIManager.Instance._baseUrl}/rooms/{roomId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                //print("updated room data received");
                string responseJson = request.downloadHandler.text;
                //print(responseJson);

                var updatedRoomData = JsonConvert.DeserializeObject<RoomDataModel>(responseJson);

                ApplyChangesAndSendEvents(updatedRoomData);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

            }
        }
    }

    public void TrySetRoomData(int wallHeight, int ballCount, string exerciseType) {
        var updatedRoomData = currentRoom;
        updatedRoomData.wallHeight = wallHeight;
        updatedRoomData.ballCount = ballCount;
        updatedRoomData.exerciseType = exerciseType;

        StartCoroutine(SendSetRoomDataRequest(updatedRoomData));
    }

    IEnumerator SendSetRoomDataRequest(RoomDataModel data) {
        string url = $"{APIManager.Instance._baseUrl}/rooms/{data._id}/update";

        string json = JsonConvert.SerializeObject(data);

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;
                print(responseJson);

                OnGameDataSet?.Invoke();
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

                OnSettingGameDataError?.Invoke(request.error);
            }
        }
    }

    public void TryAddRelayCode(string relayCode) {
        StartCoroutine(SendAddRelayCodeRequest(relayCode));
    }

    IEnumerator SendAddRelayCodeRequest(string relayCode) {
        string url = $"{APIManager.Instance._baseUrl}/rooms/{currentRoom._id}/relay/{relayCode}";

        using (UnityWebRequest request = UnityWebRequest.Post(url, "", "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;
                print(responseJson);

                print("relay code added");
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

                OnRelayCodeAddingError?.Invoke(request.error);
            }
        }
    }

    private void ApplyChangesAndSendEvents(RoomDataModel updatedRoom) {
        if (!currentRoom.hasPatientJoined && updatedRoom.hasPatientJoined) {
            currentRoom = updatedRoom;
            OnPatientJoined?.Invoke();
        }
        if (currentRoom.hasPatientJoined && !updatedRoom.hasPatientJoined) {
            currentRoom = updatedRoom;
            OnPatientLeft?.Invoke();
        }
        if (!currentRoom.hasDoctorJoined && updatedRoom.hasDoctorJoined) {
            currentRoom = updatedRoom;
            OnDoctorJoined?.Invoke();
        }
        if (currentRoom.hasDoctorJoined && !updatedRoom.hasDoctorJoined) {
            currentRoom = updatedRoom;
            OnDoctorLeft?.Invoke();
        }
        if (currentRoom.relayCode == "" && updatedRoom.relayCode != "") {
            print("relay added");
            currentRoom = updatedRoom;
            OnRelayCodeAdded?.Invoke(currentRoom.relayCode);
        }
        if (!currentRoom.gameShouldStart && updatedRoom.gameShouldStart) {
            currentRoom = updatedRoom;
            OnGameShouldStart?.Invoke();
        }
        else {
            currentRoom = updatedRoom;
        }
    }

    private void OnApplicationQuit() {
        if (currentRoom != null) {
            TryLeaveRoom(currentRoom._id, UserDataManager.Instance.userRole);
        }
    }
}
