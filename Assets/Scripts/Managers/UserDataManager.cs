using System;
using UnityEngine;

public class UserDataManager : MonoBehaviour {
    public static UserDataManager Instance;

    public string userEmail;
    public string userRole;
    public string userId;

    public string joinedDoctorName;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        SetupEvents();
    }


    private void SetupEvents() {
        APIManager.Instance.UserSignedIn += model => {
            userEmail = model.email;
            userRole = model.role;
            userId = model._id;
        };
    }
}