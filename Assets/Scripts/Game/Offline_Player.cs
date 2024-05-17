using Astra;
using UnityEngine;

public class Offline_Player : MonoBehaviour {
    [SerializeField] private int sensitivity;

    public GameObject leftHand;
    public GameObject rightHand;

    private void Awake() {
        if (sensitivity == 0) {
            sensitivity = 2;
        }
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
