using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class RoomDetailsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;
    Label otherAttendeeLabel;

    MiniElement container;
    MiniElement topSection;
    MiniElement midSection;

    string currentUserRole;
    string roomId;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        currentUserRole = UserDataManager.Instance.userRole;
        roomId = (string) _recievedData;

        errorText = CreateAndAddElement<Label>("errorText");

        container = CreateAndAddElement<MiniElement>("container");

        topSection = container.CreateAndAddElement<MiniElement>("top");

        midSection = container.CreateAndAddElement<MiniElement>("middle");

        otherAttendeeLabel = topSection.CreateAndAddElement<Label>();

        if (currentUserRole == "Patient") {
            RenderPatientView();
        }
        else {
            RenderDoctorView();
        }

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back";
        backBtn.clicked += BackBtn_clicked;

        RoomManager.Instance.TryJoinRoom(roomId, currentUserRole);

        SetupEvents();
    }

    private void BackBtn_clicked() {
        //_router.Navigate(this, "UpcomingAppointmentsPage");
        if (RoomManager.Instance.currentRoom != null) {
            RoomManager.Instance.TryLeaveRoom(roomId, currentUserRole);
        }
        else {
            _router.Navigate(this, "UpcomingAppointmentsPage");
        }
    }

    private void RenderPatientView() {
        otherAttendeeLabel.text = "Waiting for doctor to join...";
    }

    private void RenderDoctorView() {
        otherAttendeeLabel.text = "Waiting for patient to join...";

        var ballsDD = midSection.CreateAndAddElement<DropdownField>();
        ballsDD.value = "Select Number Of Balls";
        ballsDD.choices.Add("3");
        ballsDD.choices.Add("5");
        ballsDD.choices.Add("7");
        ballsDD.choices.Add("9");

        var wallDD = midSection.CreateAndAddElement<DropdownField>();
        wallDD.value = "Select Wall Height";
        wallDD.choices.Add("3");
        wallDD.choices.Add("4");
        wallDD.choices.Add("5");
        wallDD.choices.Add("6");

        var handDD = midSection.CreateAndAddElement<DropdownField>();
        handDD.value = "Which hand would you like to exercise?";
        handDD.choices.Add("Left");
        handDD.choices.Add("Right");

        var btn = midSection.CreateAndAddElement<Button>("btn");
        btn.text = "Start Game";
        btn.clicked += () => {
            //TODO: handle for error checking
            if (RoomManager.Instance.currentRoom.hasPatientJoined) {
                RoomManager.Instance.TrySetRoomData(int.Parse(wallDD.value), int.Parse(ballsDD.value), handDD.value);
            }
            else {
                errorText.text = "Patient has not joined. Cannot start session";
            }
        };
    }

    private void SetupEvents() {
        RoomManager.Instance.OnRoomJoined += roomData => {
            //print("joined room" + roomData._id);
            
            if (currentUserRole == "Patient" && roomData.hasDoctorJoined) {
                otherAttendeeLabel.text = roomData.doctorName + " is here";
            }
            else if (currentUserRole == "Doctor" && roomData.hasPatientJoined) {
                otherAttendeeLabel.text = roomData.patientName + " is here";
            }
        };

        RoomManager.Instance.OnRoomJoinError += error => {
            errorText.text = error;
        };

        RoomManager.Instance.OnRoomLeft += () => {
            _router.Navigate(this, "UpcomingAppointmentsPage");
        };

        RoomManager.Instance.OnRoomLeaveError += error => {
            errorText.text = error;
        };

        if (currentUserRole == "Patient") {
            RoomManager.Instance.OnDoctorJoined += () => {
                otherAttendeeLabel.text = RoomManager.Instance.currentRoom.doctorName+ " is here";
            };

            RoomManager.Instance.OnDoctorLeft += () => {
                otherAttendeeLabel.text = "Doctor has left";
            };

            RoomManager.Instance.OnGameShouldStart += () => {
                errorText.text = "Starting game"; // add relay code
            };

            UnityServicesManager.Instance.onHostStarted += (code) => {
                _router.Navigate(this, "OfflineGameUI"); //might be wrong
            };
        }

        else if (currentUserRole == "Doctor") {
            RoomManager.Instance.OnPatientJoined += () => {
                otherAttendeeLabel.text = RoomManager.Instance.currentRoom.patientName + " is here";
            };

            RoomManager.Instance.OnPatientLeft += () => {
                otherAttendeeLabel.text = "Patient has left";
            };

            RoomManager.Instance.OnSettingGameDataError += error => {
                errorText.text = error;
            };

            RoomManager.Instance.OnGameDataSet += () => {
                errorText.text = "Game Data Set";
            };

            UnityServicesManager.Instance.onClientStarted += () => {
                _router.Navigate(this, "OfflineGameUI"); //might be wrong
            };
        }
    }
}