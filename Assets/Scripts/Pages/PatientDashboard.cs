using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PatientDashboard : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        var practiceBtn = CreateAndAddElement<Button>("btn");
        practiceBtn.text = "Practice";
        practiceBtn.clicked += PracticeBtn_clicked;
    }

    private void PracticeBtn_clicked() {
        _router.Navigate(this, "OfflinePreGamePage");
    }
}