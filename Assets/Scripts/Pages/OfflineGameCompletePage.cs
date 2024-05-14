using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniUI;
using UnityEngine.UIElements;

public class OfflineGameCompletePage : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        var playAgainBtn = CreateAndAddElement<Button>("btn");
        playAgainBtn.text = "Play Again";
        playAgainBtn.clicked += PlayAgainBtn_clicked;
        

        var exitBtn = CreateAndAddElement<Button>("btn");
        exitBtn.text = "Quit Game";
        exitBtn.clicked += ExitBtn_clicked;
    }

    private void PlayAgainBtn_clicked() {
        OfflineGameManager.Instance.RestartGame();
        _router.Navigate(this, "OfflineGameUI");
    }

    private void ExitBtn_clicked() {
        OfflineGameManager.Instance.StopGame();
        _router.Navigate(this, "PatientDashboard");
    }
}
