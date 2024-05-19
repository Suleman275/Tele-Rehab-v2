using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Online_Ball : NetworkBehaviour {
    private bool isCompleted;
    private Hand holdingHand;

    private void Update() {
        if (IsServer) {
            if (holdingHand) {
                transform.position = holdingHand.transform.position;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (IsServer) {
            if (!isCompleted) {
                if (other.gameObject.TryGetComponent<Hand>(out Hand hand) && !hand.hasBall) { //if collided with hand and hand does not have a ball
                    hand.hasBall = true;
                    holdingHand = hand;
                }
                else if (other.gameObject.TryGetComponent<MiddleWall>(out MiddleWall wall) && holdingHand != null) { //if in hand and collides with wall
                    DropBall();
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (IsServer) {
            if (holdingHand != null) {
                isCompleted = true;
                holdingHand.hasBall = false;
                holdingHand = null;

                OnlineGameManager.Instance.BallCompletedServerRPC();
            }
        }
    }

    private void DropBall() {
        if (holdingHand != null) {
            holdingHand.hasBall = false;
            holdingHand = null;

            OnlineGameManager.Instance.BallDroppedServerRPC();
        }
    }
}