using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Offline_Ball : MonoBehaviour {
    public bool isCompleted;
    private Hand holdingHand;

    private void Update() {
        if (holdingHand) {
            transform.position = holdingHand.transform.position;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
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

    private void OnTriggerEnter2D(Collider2D other) {
        if (holdingHand != null) {
            isCompleted = true;
            holdingHand.hasBall = false;
            holdingHand = null;

            OfflineGameManager.Instance.BallCompleted();
        }
    }

    private void DropBall() {
        if (holdingHand != null) {
            holdingHand.hasBall = false;
            holdingHand = null;

            OfflineGameManager.Instance.BallDropped();
        }
    }
}
