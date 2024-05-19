using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class ReplayUI : MiniPage {
    protected override void RenderPage() {
        //InheritStylesFromComponentRouter();

        var pauseBtn = CreateAndAddElement<Button>();
        pauseBtn.text = "Pause Replay";
        pauseBtn.clicked += () => {
            pauseBtn.text = "Resume Replay";
            ReplayManager.Instance.TogglePause();
        };

        ReplayManager.Instance.OnReplayFinished += () => {
            _router.Navigate(this, "ReplayCompletePage");
        };
    }
}