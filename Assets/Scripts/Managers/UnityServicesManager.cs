using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityServicesManager : MonoBehaviour {
    public static UnityServicesManager Instance;

    private void Awake() {
        Instance = this;
    }


}