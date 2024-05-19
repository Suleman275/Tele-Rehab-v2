using Astra;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Online_Player : NetworkBehaviour {
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] int sensitivity;

    public static Online_Player LocalInstance;

    public string userRole;

    public override void OnNetworkSpawn() {
        if (!IsOwner) {
            return;
        }

        LocalInstance = this;

        if (UserDataManager.Instance.userRole == "Doctor") {
            DeactivateHandsServerRPC();
        }
        else {
            sensitivity = 2; //setting manually for now
        }

        if (IsServer) {
            DataRecorder.Instance.AddObjectToTrack(leftHand);
            DataRecorder.Instance.AddObjectToTrack(rightHand);
        }
    }

    [ServerRpc]
    private void DeactivateHandsServerRPC() {
        DeactivateHandsClientRPC();
    }

    [ClientRpc]
    private void DeactivateHandsClientRPC() {
        leftHand.SetActive(false);
        rightHand.SetActive(false);
    }

    void Update() {
        var body = AstraManager.Instance.GetBody(0);

        if (body == null) {
            return;
        }

        var joints = AstraManager.Instance.GetJointsFromBody(body);

        if (joints == null) {
            return;
        }

        foreach (var joint in body.Joints) {
            if (joint.Type == JointType.LeftHand) {
                leftHand.transform.localPosition = new Vector3(joint.WorldPosition.X, joint.WorldPosition.Y, joint.WorldPosition.Z) / 100 * sensitivity;
                //if (body.HandPoseInfo.LeftHand == Astra.HandPose.Grip) {
                //    //print("left hand closed");
                //}
            }
            else if (joint.Type == JointType.RightHand) {
                rightHand.transform.localPosition = new Vector3(joint.WorldPosition.X, joint.WorldPosition.Y, joint.WorldPosition.Z) / 100 * sensitivity;
                //if (body.HandPoseInfo.RightHand == Astra.HandPose.Grip) {
                //    //print("right hand closed");
                //}
            }
        }
    }
}
