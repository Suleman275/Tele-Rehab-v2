//using ParrelSync;
using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Vivox;
using UnityEngine;

public class UnityServicesManager : MonoBehaviour {
    [SerializeField] UnityTransport transport;

    public static UnityServicesManager Instance;

    public Action<string> onHostStarted;
    public Action<string> onHostStartingError;
    
    public Action onClientStarted;
    public Action<string> onClientStartingError;

    private void Awake() {
        Instance = this;

        InitServices();
    }

    private void Start() {
        APIManager.Instance.UserSignedIn += (model) => {
            if (model.role == "Patient") {
                RoomManager.Instance.OnGameShouldStart += () => {
                    print("starting game");
                    StartRelayHost();
                };
            }
            else {
                RoomManager.Instance.OnRelayCodeAdded += (joinCode) => {
                    StartRelayClient(joinCode);
                };
            }
        };

        //removed for local testing
        //RoomManager.Instance.OnRoomJoined += (room) => {
        //    JoinAudioChannel(room._id);
        //};

        RoomManager.Instance.OnRoomLeft += () => { 
            LeaveAllAudioChannels();
        };
    }

    private async void InitServices() {
//        var options = new InitializationOptions();
//#if UNITY_EDITOR
//        options.SetProfile(ClonesManager.IsClone() ? ClonesManager.GetArgument() : "Primary");
//#endif
//        await UnityServices.InitializeAsync(options);

        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        await VivoxService.Instance.InitializeAsync();
    }

    private async void StartRelayHost() {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            print("Unity Services Not Initialised");
            onHostStartingError?.Invoke("Unity Services Not Initialised");
            return;
        }

        if (!AuthenticationService.Instance.IsSignedIn) {
            print("not signed into unity");
            onHostStartingError?.Invoke("not signed into unity");
            return;
        }

        print("starting host");

        try {
            //print("creating allocation");
            Allocation a = await RelayService.Instance.CreateAllocationAsync(2);
            //print("Created allocation");
            var relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);
            //print("got join code: " + relayJoinCode);
            transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);
            //print("set transport");
            NetworkManager.Singleton.StartHost();

            //print("host started");

            onHostStarted?.Invoke(relayJoinCode);
        } 
        catch (RelayServiceException e) {
            onHostStartingError?.Invoke(e.Message);
        }
    }

    private async void StartRelayClient(string joinCode) {
        if (UnityServices.State != ServicesInitializationState.Initialized) {
            print("Unity Services Not Initialised");
            onClientStartingError?.Invoke("Unity Services Not Initialised");
            return;
        }

        //print("starting client");

        try {
            JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(joinCode);
            transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);
            NetworkManager.Singleton.StartClient();

            print("client started");

            onClientStarted?.Invoke();
        }
        catch (RelayServiceException e) {
            onClientStartingError?.Invoke(e.Message);
        }
    }

    private async void JoinAudioChannel(string channelName) {
        await VivoxService.Instance.JoinGroupChannelAsync(channelName, ChatCapability.AudioOnly);
    }

    private async void LeaveAllAudioChannels() {
        await VivoxService.Instance.LeaveAllChannelsAsync();
    }
}