using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class OnlineGameCompletePage : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        SetupEvents();

        if (UserDataManager.Instance.userRole == "Doctor") {
            var playAgainBtn = CreateAndAddElement<Button>("btn");
            playAgainBtn.text = "Play Again";
            playAgainBtn.clicked += PlayAgainBtn_clicked;
        }


        var exitBtn = CreateAndAddElement<Button>("btn");
        exitBtn.text = "Quit Game";
        exitBtn.clicked += ExitBtn_clicked;
    }

    private void PlayAgainBtn_clicked() {
        OnlineGameManager.Instance.RestartGameServerRPC();
        _router.Navigate(this, "OfflineGameUI");
    }

    private void ExitBtn_clicked() {
        RoomManager.Instance.TryLeaveRoom(RoomManager.Instance.currentRoom._id, UserDataManager.Instance.userRole);
    }

    private void SetupEvents() {
        RoomManager.Instance.OnRoomLeft += () => {
            OnlineGameManager.Instance.StopGame();
            if (UserDataManager.Instance.userRole == "Patient") {
                _router.Navigate(this, "PatientDashboard");
            }
            else {
                _router.Navigate(this, "DoctorDashboard");
            }
        };

        OnlineGameManager.Instance.OnGameStarted += () => {
            _router.Navigate(this, "OfflineGameUI");
        };
    }
}