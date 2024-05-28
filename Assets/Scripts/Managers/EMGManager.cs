using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EMGManager : MonoBehaviour {
    public static EMGManager Instance;

    string deviceID;
    string serviceUuid = "{00001815-0000-1000-8000-00805f9b34fb}";
    string[] characteristicUuids = { "{00002a58-0000-1000-8000-00805f9b34fb}" };

    BLE ble = new BLE();

    public bool isEMGConnected;
    public bool isReading;
    public ushort currentVal;

    private Thread readThread; // Declare a Thread variable
    private bool isReadingThreadRunning = false; // Flag to control thread execution

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        BLE.BLEScan scan = BLE.ScanDevices();

        scan.Found = (deviceId, deviceName) => {
            if (deviceName == "ANR Corp M40") {
                try {
                    ble.Connect(deviceId,
                        serviceUuid,
                        characteristicUuids);
                }
                catch (Exception e) {
                    Debug.Log("Could not establish connection to device with ID " + deviceId + "\n" + e);
                }

                if (ble.isConnected) {
                    isEMGConnected = true;
                    StartReadThread(); // Start the thread when connected
                }
            }
        };
    }

    private void Update() {
        if (ble.isConnected) {
            // No need to call ReadAndSaveData() here anymore
        }
    }

    private void StartReadThread() {
        isReadingThreadRunning = true; // Set flag to true

        // Create a new Thread object and pass the ReadAndSaveData function as its delegate
        readThread = new Thread(ReadAndSaveData);
        readThread.Start();
    }

    private void StopReadThread() {
        isReadingThreadRunning = false; // Set flag to false to signal thread termination
    }

    private void ReadAndSaveData() {
        while (isReadingThreadRunning) // Loop until the flag is set to false
        {
            byte[] packageReceived = BLE.ReadBytes();

            byte[] data = new byte[2] { packageReceived[0], packageReceived[1] };

            var value = BitConverter.ToUInt16(data, 0);

            //print("Received val: " + value);

            currentVal = value;

            // Add logic for data processing or storage here (if needed)

            // You can introduce a delay between readings here (if desired)
            // Thread.Sleep(milliseconds);
        }

        Debug.Log("Read thread stopped."); // Optional: Log termination message
    }

    private void OnApplicationQuit() {
        StopReadThread(); // Ensure thread stops before closing
        ble.Close();
    }
}
