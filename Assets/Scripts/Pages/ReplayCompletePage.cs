using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class ReplayCompletePage : MiniPage {
    [SerializeField] StyleSheet styles;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        SetupEvents();

        AddStyleSheet(styles);

        var container = CreateAndAddElement<MiniElement>("Container");

        var replayBtn = container.CreateAndAddElement<Button>("btn");
        replayBtn.text = "Play session recording again";
        replayBtn.clicked += ReplayBtn_clicked;

        var exitBtn = container.CreateAndAddElement<Button>("btn");
        exitBtn.text = "Exit to Dashboard";
        exitBtn.clicked += ExitBtn_clicked;
    }

    private void ExitBtn_clicked() {
        ReplayManager.Instance.StopReplay();
        _router.Navigate(this, "DoctorDashboard");
    }

    private void ReplayBtn_clicked() {
        _router.Navigate(this, "ReplayUI");
        ReplayManager.Instance.StartReplay();
    }

    private void SetupEvents() {
        ReplayManager.Instance.OnReplayFinished += () => { 
            enabled = true;
        };
    }


}