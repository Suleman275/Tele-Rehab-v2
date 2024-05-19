using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;
using System.ComponentModel;

public class PastSessionsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;
    ScrollView midSection;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();
        AddStyleSheet(styles);

        SetupEvents();

        errorText = CreateAndAddElement<Label>("errorText");

        var container = CreateAndAddElement<MiniElement>("container");

        var topSection = container.CreateAndAddElement<MiniElement>("top");

        topSection.CreateAndAddElement<Label>("title").text = "Past Sessions";

        midSection = container.CreateAndAddElement<ScrollView>("middle");

        APIManager.Instance.TryGetPastSessions();

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back";
        backBtn.clicked += () => {
            _router.Navigate(this, "DoctorDashboard");
        };
    }

    private void SetupEvents() {
        APIManager.Instance.OnGetSessionsSuccess += sessions => {
            var headerRow = CreateElement<MiniElement>("header", "row");
            headerRow.CreateAndAddElement<Label>().text = "Session ID";
            headerRow.CreateAndAddElement<Label>().text = "Patient Name";
            headerRow.CreateAndAddElement<Label>().text = "Doctor Name";
            headerRow.CreateAndAddElement<Label>().text = "Exercise Type";
            headerRow.CreateAndAddElement<Label>().text = "Session Duration";
            headerRow.CreateAndAddElement<Label>().text = "";

            midSection.Add(headerRow);

            foreach (var session in sessions) {
                var row = CreateElement<MiniElement>("row");
                row.CreateAndAddElement<Label>().text = session._id;
                row.CreateAndAddElement<Label>().text = session.patientName;
                row.CreateAndAddElement<Label>().text = session.doctorName;
                row.CreateAndAddElement<Label>().text = session.exerciseType;
                row.CreateAndAddElement<Label>().text = session.sessionEndTime - session.sessionStartTime + " sec.";
                var replayBtn = row.CreateAndAddElement<Button>();
                replayBtn.text = "Replay this session";
                replayBtn.clicked += () => {
                    APIManager.Instance.TryGetPastSession(session._id);
                };

                midSection.Add(row);
            }
        };

        APIManager.Instance.OnGetSessionSuccess += sessionData => {
            ReplayManager.Instance.SetSessionData(sessionData);
            _router.Navigate(this, "ReplayUI");
            ReplayManager.Instance.StartReplay();
        };

        APIManager.Instance.OnGetSessionsError += error => errorText.text = error;
        APIManager.Instance.OnGetSessionError += error => errorText.text = error;
    }
}