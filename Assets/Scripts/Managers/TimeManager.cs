using System;
using UnityEngine;

public class TimeManager : MonoBehaviour {
    public static TimeManager Instance;

    public float timeSinceApplicationStart;

    private void Awake() {
        Instance = this;

        timeSinceApplicationStart = 0;
    }

    private void Update() {
        timeSinceApplicationStart += Time.unscaledDeltaTime;
    }
}