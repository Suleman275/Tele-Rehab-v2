using System.Collections;
using System.Collections.Generic;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;

public class OnlineGameManager : MonoBehaviour {
    public static OnlineGameManager Instance;

    private void Awake() {
        Instance = this;
    }
}
