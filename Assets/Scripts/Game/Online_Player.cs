using Astra;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class Online_Player : NetworkBehaviour {
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] int sensitivity;
    [SerializeField] GameObject jointPrefab;

    public static Online_Player LocalInstance;

    private Dictionary<JointType, GameObject> jointsMap = new Dictionary<JointType, GameObject>();

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
        if (UserDataManager.Instance.userRole == "Doctor") return;

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

            if (jointsMap.ContainsKey(joint.Type)) {
                jointsMap[joint.Type].transform.localPosition = new Vector3(joint.WorldPosition.X, joint.WorldPosition.Y, joint.WorldPosition.Z) / 200 * 2;
            }
            else {
                var jointGO = Instantiate(jointPrefab);

                var jointNO = jointGO.GetComponent<NetworkObject>();

                jointNO.Spawn();
                jointNO.TrySetParent(this.transform);

                jointsMap.Add(joint.Type, jointGO);
            }
        }
    }
}
