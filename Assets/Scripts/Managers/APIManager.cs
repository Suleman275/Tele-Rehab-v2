using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; // Import the necessary namespace

public class APIManager : MonoBehaviour {
    public static APIManager Instance;
    public string baseUrl = "http://localhost:3000"; // Your Express API URL
    //private string baseUrl = "https://fc60-94-122-46-79.ngrok-free.app"; // Your Express API URL
    
    //Events
    public Action<UserDataModel> UserSignedIn;
    public Action<string> UserSignInFailed;

    public Action AppointmentCreated;
    public Action<string> AppointmentCreationError;

    public Action<List<AppointmentDataModel>> OnAllAppointmentsRecieved;
    public Action<string> OnGetAllAppointmentsError;
    
    public Action OnAppointmentUpdated;
    public Action<string> OnAppointmentUpdateError;

    public Action<RoomDataModel> OnRoomDataRecieved;
    public Action<string> OnGetRoomError;

    private void Awake() {
        Instance = this;
    }

    public void TrySignUp(string email, string password, string role) {
        var jsonBody = new UserDataModel();
        jsonBody.email = email;
        jsonBody.password = password;
        jsonBody.role = role;
        
        string json = JsonUtility.ToJson(jsonBody);
        
        StartCoroutine(SendSignUpRequest(json));
    }

    private IEnumerator SendSignUpRequest(string json) {
        string url = $"{baseUrl}/signup";

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                Debug.Log("User created successfully");
            }
            else {
                Debug.LogError("Error creating user: " + request.error);
            }
        }
    }
    
    public void TrySignIn(string email, string password) {
        var jsonBody = new UserDataModel();
        jsonBody.email = email;
        jsonBody.password = password;
    
        // Convert the JSON object to a string
        string json = JsonUtility.ToJson(jsonBody);

        StartCoroutine(SendSignInRequest(json));
    }

    private IEnumerator SendSignInRequest(string json) {
        string url = $"{baseUrl}/signin";

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                if (responseJson == "Invalid Credentials") {
                    //Debug.LogError("Invalid Credentials");
                    UserSignInFailed?.Invoke("Invalid Credentials");
                }
                else {
                    var userData = JsonUtility.FromJson<UserDataModel>(responseJson);
                    UserSignedIn?.Invoke(userData);
                }
            }
            else {
                //Debug.LogError("Error signing in: " + request.error);
                UserSignInFailed?.Invoke(request.error);
            }
        }
    }

    public void TryCreateAppointment(string name, string time) {
        AppointmentDataModel data = new AppointmentDataModel {
            _id = Guid.NewGuid().ToString(),
            requestSender = UserDataManager.Instance.userEmail,
            requestSenderRole = UserDataManager.Instance.userRole,
            appointmentWith = name,
            time = time,
            status = "Pending"
        };

        string jsonData = JsonUtility.ToJson(data);
        StartCoroutine(SendCreateAppointmentRequest(jsonData));
    }

    IEnumerator SendCreateAppointmentRequest(string jsonData) {
        string url = $"{baseUrl}/appointments/";

        using (UnityWebRequest webRequest = UnityWebRequest.Post(url, jsonData, "application/json")) {
            yield return webRequest.SendWebRequest();

            if (webRequest.result != UnityWebRequest.Result.Success) {
                //Debug.LogError($"Error: {webRequest.error}");
                AppointmentCreationError?.Invoke(webRequest.error);
            }
            else {
                //Debug.Log($"Received: {webRequest.downloadHandler.text}");
                if (webRequest.downloadHandler.text == "Internal Server Error") {
                    AppointmentCreationError?.Invoke("Internal Server Error");
                }
                else if (webRequest.downloadHandler.text == "That user could not be found") {
                    AppointmentCreationError?.Invoke("That user could not be found");
                }
                else if (webRequest.downloadHandler.text == "not a valid date") {
                    AppointmentCreationError?.Invoke("Not a valid date");
                }
                else {
                    AppointmentCreated?.Invoke();
                }
            }
        }
    }

    public void TryGetAllAppointments() {
        StartCoroutine(SendGetAllAppointmentsRequest());
    }

    private IEnumerator SendGetAllAppointmentsRequest() {
        string url = $"{baseUrl}/appointments/";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                //print("all appointments received");
                string responseJson = request.downloadHandler.text;

                var appointments = JsonConvert.DeserializeObject<List<AppointmentDataModel>>(responseJson);

                OnAllAppointmentsRecieved?.Invoke(appointments);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);
                OnGetAllAppointmentsError?.Invoke(request.error);
            }
        }
    }

    public void TryGetAllAppointmentsByParticipant(string participantName) {
        StartCoroutine(SendGetAppointmentsByParticipantRequest(participantName));
    }

    IEnumerator SendGetAppointmentsByParticipantRequest(string participantName) {
        string url = $"{baseUrl}/appointments/participant={participantName}/";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                //Debug.Log("Appointments fetched successfully: " + jsonResponse);

                var appointments = JsonConvert.DeserializeObject<List<AppointmentDataModel>>(jsonResponse);

                OnAllAppointmentsRecieved?.Invoke(appointments);
            }
            else {
                Debug.LogError("Error fetching appointments: " + request.responseCode);
                Debug.LogError("Error fetching appointments: " + request.error);

                OnGetAllAppointmentsError?.Invoke(request.error);
            }
        }
    }

    public void TryGetAllAppointmentsWithStatus(string status) {
        StartCoroutine(SendGetAllAppointmentsWithStatusRequest(status));
    }

    private IEnumerator SendGetAllAppointmentsWithStatusRequest(string status) {
        string url = $"{baseUrl}/appointments/status={status}/";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                //print($"all appointments with status: {status} received");
                string responseJson = request.downloadHandler.text;

                var appointments = JsonConvert.DeserializeObject<List<AppointmentDataModel>>(responseJson);

                OnAllAppointmentsRecieved?.Invoke(appointments);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);
                OnGetAllAppointmentsError?.Invoke(request.error);
            }
        }
    }

    public void TryUpdateAppointmentData(AppointmentDataModel updatedAppointmentData) {
        StartCoroutine(SendUpdateAppointmentDataRequest(updatedAppointmentData));
    }

    private IEnumerator SendUpdateAppointmentDataRequest(AppointmentDataModel updatedAppointmentData) {
        string url = $"{baseUrl}/appointments/{updatedAppointmentData._id}";

        string json = JsonConvert.SerializeObject(updatedAppointmentData);

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            print("sending web request");
            yield return request.SendWebRequest();

            print("sent web request with result:" + request.result);

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                print("recieved response: " + responseJson);

                if (responseJson == "Appointment not found") {
                    print("appointment not found error");
                    OnAppointmentUpdateError?.Invoke(responseJson);
                }
                else {
                    //invoke success event
                    print("appointment updated");
                    OnAppointmentUpdated?.Invoke();
                }
            }
            else {
                Debug.LogError("Error posting data in: " + request.error);
                OnAppointmentUpdateError?.Invoke(request.error);
            }
        }
    }

    public void TryPostSessionData(SessionDataModel sessionData) {
        var json = JsonConvert.SerializeObject(
            sessionData,
            Formatting.Indented,
            new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

        StartCoroutine(SendSessionData(json));
    }

    IEnumerator SendSessionData(string json) {
        string url = $"{baseUrl}/sessions";

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                print(responseJson);
            }
            else {
                Debug.LogError("Error posting data in: " + request.error);
            }
        }
    }

    public void TryGetPastSessions(Action<string> callback) {
        StartCoroutine(SendPastSessionsGetRequest(callback));
    }

    IEnumerator SendPastSessionsGetRequest(Action<string> callback) {
        string url = $"{baseUrl}/sessions";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                callback(responseJson);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);
            }
        }
    }

    public void TryGetPastSession(string sessionId, Action<string> callback) {
        StartCoroutine(SendGetSessionDataRequest(sessionId, callback));
    }

    IEnumerator SendGetSessionDataRequest(string sessionId, Action<string> callback) {
        string url = $"{baseUrl}/sessions/{sessionId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                callback(responseJson);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);
            }
        }
    }
}