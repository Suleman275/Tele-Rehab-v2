using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class OfflineGameUI : MiniPage {
    
    private Label gameStateLabel;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        SetupEvents();

        gameStateLabel = CreateAndAddElement<Label>("errorText");
        gameStateLabel.text = "Game Started";
    }

    private void SetupEvents() {
        OfflineGameManager.Instance.OnBallCompleted += (numCompletedBalls, totalBalls) => {
            gameStateLabel.text = $"Balls Completed {numCompletedBalls} / {totalBalls}";
        };

        OfflineGameManager.Instance.OnGameCompleted += () => {
            _router.Navigate(this, "OfflineGameCompletePage");
        };
    }
}
