using Astra;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AstraManager : MonoBehaviour {
    public static AstraManager Instance;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        // Ensure that the AstraSDKManager exists
        if (AstraSDKManager.Instance == null) {
            Debug.LogError("AstraSDKManager is not assigned!");
            return;
        }

        // Subscribe to the OnInitializeSuccess event
        AstraSDKManager.Instance.OnInitializeSuccess.AddListener(OnAstraSDKInitializeSuccess);

        AstraSDKManager.Instance.OnInitializeFailed.AddListener(OnAstraSDKInitializeFailed);
    }

    void OnAstraSDKInitializeSuccess() {
        Debug.Log("Astra SDK initialized successfully!");
    }

    void OnAstraSDKInitializeFailed() {
        Debug.Log("Astra SDK failed to initialize");
    }

    public void StartBodyStream() {
        AstraSDKManager.Instance.IsBodyOn = true;
    }

    public void StopBodyStream() {
        AstraSDKManager.Instance.IsBodyOn = false;
    }

    //public Astra.Joint[] GetBodyJoints(int bodyIndex) {
    //    if (AstraSDKManager.Instance.Initialized && AstraSDKManager.Instance.IsBodyOn) {
    //        Body[] bodies = AstraSDKManager.Instance.Bodies;

    //        if (bodies.Length > 0) {
    //            Body body = bodies[bodyIndex];

    //            if (body.Status == BodyStatus.Tracking) {
    //                return body.Joints;
    //            }
    //        }
    //    } 

    //    return null;
    //}

    public Body GetBody(int bodyIndex) {
        if (AstraSDKManager.Instance.Initialized && AstraSDKManager.Instance.IsBodyOn) {
            Body[] bodies = AstraSDKManager.Instance.Bodies;

            if (bodies.Length > 0) {
                return bodies[bodyIndex];
            }
        }

        return null;
    }

    public Astra.Joint[] GetJointsFromBody(Body body) {
        if (body.Status == BodyStatus.Tracking) {
            return body.Joints;
        }
        return null;
    }
}