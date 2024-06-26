using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking; // Import the necessary namespace

public class APIManager : MonoBehaviour {
    public static APIManager Instance;
    //public string _baseUrl = "https://dcac-94-122-35-221.ngrok-free.app"; // Your Express API URL
    //public string _baseUrl = "localhost:3000"; // Your Express API URL
    public string _baseUrl = ""; // Your Express API URL



    //Events
    public Action<UserDataModel> UserSignedIn;
    public Action<string> UserSignInFailed;
    
    public Action<UserDataModel> UserRegistered;
    public Action<string> UserRegisterError;

    public Action AppointmentCreated;
    public Action<string> AppointmentCreationError;

    public Action<List<AppointmentDataModel>> OnAllAppointmentsRecieved;
    public Action<string> OnGetAllAppointmentsError;
    
    public Action OnAppointmentUpdated;
    public Action<string> OnAppointmentUpdateError;

    public Action<RoomDataModel> OnRoomDataRecieved;
    public Action<string> OnGetRoomError;

    public Action<List<SessionDataModel>> OnGetSessionsSuccess;
    public Action<string> OnGetSessionsError;
    
    public Action<SessionDataModel> OnGetSessionSuccess;
    public Action<string> OnGetSessionError;

    private void Awake() {
        Instance = this;
    }

    public void TrySignUp(string email, string username, string password, string role) {
        var jsonBody = new UserDataModel();
        jsonBody._id = Guid.NewGuid().ToString();
        jsonBody.email = email;
        jsonBody.userName = username;
        jsonBody.password = password;
        jsonBody.role = role;
        
        string json = JsonUtility.ToJson(jsonBody);
        
        StartCoroutine(SendSignUpRequest(json));
    }

    private IEnumerator SendSignUpRequest(string json) {
        string url = $"{_baseUrl}/signup";

        using (UnityWebRequest request = UnityWebRequest.Post(url, json, "application/json")) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                //Debug.Log("User created successfully");
                if (request.downloadHandler.text == "Email already exists") {
                    UserRegisterError?.Invoke("Email already in use");
                }
                else if (request.downloadHandler.text == "Username already exists") {
                    UserRegisterError?.Invoke("Username already in use");
                }
                else {
                    var user = JsonConvert.DeserializeObject<UserDataModel>(json);
                    UserRegistered?.Invoke(user);
                }
            }
            else {
                //Debug.LogError("Error creating user: " + request.error);
                UserRegisterError?.Invoke(request.error);
            }
        }
    }
    
    public void TrySignIn(string email, string password) {
        print("sending sign in" + _baseUrl);
        var jsonBody = new UserDataModel();
        jsonBody.email = email;
        jsonBody.password = password;
    
        // Convert the JSON object to a string
        string json = JsonUtility.ToJson(jsonBody);

        StartCoroutine(SendSignInRequest(json));
    }

    private IEnumerator SendSignInRequest(string json) {
        string url = $"{_baseUrl}/signin";

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

    public void TryCreateAppointment(string name, DateTime time) {
        AppointmentDataModel data = new AppointmentDataModel {
            _id = Guid.NewGuid().ToString(),
            requestSender = UserDataManager.Instance.username,
            requestSenderRole = UserDataManager.Instance.userRole,
            appointmentWith = name,
            time = time,
            status = "Pending"
        };

        string jsonData = JsonConvert.SerializeObject(data);
        StartCoroutine(SendCreateAppointmentRequest(jsonData));
    }

    IEnumerator SendCreateAppointmentRequest(string jsonData) {
        string url = $"{_baseUrl}/appointments/";

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
        string url = $"{_baseUrl}/appointments/";

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
        string url = $"{_baseUrl}/appointments/participant={participantName}/";

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

    public void TryGetUpcomingAppointments(string participantName) {
        //print("upcoming");
        StartCoroutine(SendGetUpcomingAppointmentsRequest(participantName));
    }

    IEnumerator SendGetUpcomingAppointmentsRequest(string participantName) {
        string url = $"{_baseUrl}/appointments/participant={participantName}/upcoming";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string jsonResponse = request.downloadHandler.text;
                
                //Debug.Log("Appointments fetched successfully: " + jsonResponse);

                if (jsonResponse == "No upcoming appointments") {
                    OnGetAllAppointmentsError?.Invoke("No upcoming appointments");
                }
                else {
                    var appointments = JsonConvert.DeserializeObject<List<AppointmentDataModel>>(jsonResponse);

                    OnAllAppointmentsRecieved?.Invoke(appointments);
                }
            }
            else { 
                Debug.LogError("Error fetching appointments: " + request.error);

                OnGetAllAppointmentsError?.Invoke(request.error);
            }
        }
    }

    public void TryGetAllAppointmentsWithStatus(string status) {
        StartCoroutine(SendGetAllAppointmentsWithStatusRequest(status));
    }

    private IEnumerator SendGetAllAppointmentsWithStatusRequest(string status) {
        string url = $"{_baseUrl}/appointments/status={status}/";

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
        string url = $"{_baseUrl}/appointments/{updatedAppointmentData._id}";

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
        string url = $"{_baseUrl}/sessions";

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

    public void TryGetPastSessions() {
        StartCoroutine(SendPastSessionsGetRequest());
    }

    IEnumerator SendPastSessionsGetRequest() {
        string url = $"{_baseUrl}/sessions";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                var sessions = JsonConvert.DeserializeObject<List<SessionDataModel>>(responseJson);

                OnGetSessionsSuccess?.Invoke(sessions);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);
                OnGetSessionsError?.Invoke(request.error);
            }
        }
    }

    public void TryGetPastSession(string sessionId) {
        StartCoroutine(SendGetSessionDataRequest(sessionId));
    }

    IEnumerator SendGetSessionDataRequest(string sessionId) {
        string url = $"{_baseUrl}/sessions/{sessionId}";

        using (UnityWebRequest request = UnityWebRequest.Get(url)) {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseJson = request.downloadHandler.text;

                var session = JsonConvert.DeserializeObject<SessionDataModel>(responseJson);

                OnGetSessionSuccess?.Invoke(session);
            }
            else {
                Debug.LogError("Error getting data: " + request.error);

                OnGetSessionError?.Invoke(request.error);
            }
        }
    }
}