using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class ReplayCompletePage : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        SetupEvents();

        var replayBtn = CreateAndAddElement<Button>("btn");
        replayBtn.text = "Play session recording again";
        replayBtn.clicked += ReplayBtn_clicked;

        var exitBtn = CreateAndAddElement<Button>("btn");
        exitBtn.text = "Exit to Dashboard";
        exitBtn.clicked += ExitBtn_clicked;
    }

    private void ExitBtn_clicked() {
        ReplayManager.Instance.StopReplay();
        _router.Navigate(this, "DoctorDashboard");
    }

    private void ReplayBtn_clicked() {
        enabled = false;
        ReplayManager.Instance.StartReplay();
    }

    private void SetupEvents() {
        ReplayManager.Instance.OnReplayFinished += () => { 
            enabled = true;
        };
    }


}