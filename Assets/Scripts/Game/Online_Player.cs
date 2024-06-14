using Astra;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Online_Player : NetworkBehaviour {
    [SerializeField] GameObject leftHand;
    [SerializeField] GameObject rightHand;
    [SerializeField] int sensitivity;
    [SerializeField] GameObject jointPrefab;
    [SerializeField] GameObject bonePrefab;

    public static Online_Player LocalInstance;

    private Dictionary<JointType, GameObject> jointsMap = new Dictionary<JointType, GameObject>();

    private Dictionary<string, GameObject> bonesMap = new Dictionary<string, GameObject>();

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
    
    public override void OnDestroy() {
        if (!IsOwner) return;

        foreach (var joint in jointsMap.Values) { 
            Destroy(joint);
        }
        jointsMap.Clear();

        foreach (var bone in bonesMap.Values) {
            Destroy(bone); 
        }
        bonesMap.Clear();
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
        if (!IsOwner) return;

        if (UserDataManager.Instance.userRole == "Doctor") return;

        var body = AstraManager.Instance.GetBody(0);

        if (body == null) {
            return;
        }

        var joints = AstraManager.Instance.GetJointsFromBody(body);

        if (joints == null) {
            return;
        }

        foreach (var joint in joints) {
            //only hands
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

            //skeleton
            DrawJoint(joint);
        }

        //draw bones
        DrawBone(JointType.Head, JointType.ShoulderSpine);
        DrawBone(JointType.ShoulderSpine, JointType.LeftShoulder);
        DrawBone(JointType.ShoulderSpine, JointType.RightShoulder);
        DrawBone(JointType.LeftShoulder, JointType.LeftElbow);
        DrawBone(JointType.LeftElbow, JointType.LeftHand);
        DrawBone(JointType.RightShoulder, JointType.RightElbow);
        DrawBone(JointType.RightElbow, JointType.RightHand);
        DrawBone(JointType.ShoulderSpine, JointType.MidSpine);
        DrawBone(JointType.MidSpine, JointType.BaseSpine);
        DrawBone(JointType.BaseSpine, JointType.LeftHip);
        DrawBone(JointType.BaseSpine, JointType.RightHip);
        DrawBone(JointType.LeftHip, JointType.LeftKnee);
        DrawBone(JointType.LeftKnee, JointType.LeftFoot);
        DrawBone(JointType.RightHip, JointType.RightKnee);
        DrawBone(JointType.RightKnee, JointType.RightFoot);
    }

    private void DrawBone(JointType startJoint, JointType endJoint) {
        string key = startJoint.ToString() + "-" + endJoint.ToString();

        if (jointsMap.ContainsKey(startJoint) && jointsMap.ContainsKey(endJoint)) {
            Vector3 startPos = jointsMap[startJoint].transform.localPosition;
            Vector3 endPos = jointsMap[endJoint].transform.localPosition;

            DrawBoneServerRPC(key, startPos, endPos);
        }
        else {
            print("key not found");
        }
    }

    [ServerRpc]
    public void DrawBoneServerRPC(string key, Vector3 startPos, Vector3 endPos) {
        DrawBoneClientRPC(key, startPos, endPos);    
    }

    [ClientRpc]
    public void DrawBoneClientRPC(string key, Vector3 startPos, Vector3 endPos) {
        if (UserDataManager.Instance.userRole == "Patient") return; //only want this to run on doctor

        if (bonesMap.ContainsKey(key)) {
            var boneRenderer = bonesMap[key].GetComponent<LineRenderer>();

            boneRenderer.SetWidth(0.1f, 0.1f);
            boneRenderer.material = new Material(Shader.Find("Diffuse"));
            boneRenderer.SetPositions(new Vector3[] { startPos, endPos });
        }
        else {
            var bone = Instantiate(bonePrefab);

            bonesMap.Add(key, bone);
        }
    }

    private void DrawJoint(Astra.Joint joint) {
        DrawJointServerRpc(joint.Type, joint.WorldPosition.X, joint.WorldPosition.Y, joint.WorldPosition.Z);
    }

    [ServerRpc]
    public void DrawJointServerRpc(JointType jointType, float xPos, float yPos, float zPos) {
        DrawJointClientRPC(jointType, xPos, yPos, zPos);
    }

    [ClientRpc]
    public void DrawJointClientRPC(JointType jointType, float xPos, float yPos, float zPos) {
        if (jointsMap.ContainsKey(jointType)) {
            jointsMap[jointType].transform.localPosition = new Vector3(xPos, yPos, zPos) / 200 * 2;
        }
        else {
            var jointGO = Instantiate(jointPrefab); //Joint Game Object

            jointsMap.Add(jointType, jointGO);

            if (UserDataManager.Instance.userRole == "Patient") jointGO.SetActive(false); //only want doctor to see joints
        }
    }
}