using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PatientDashboard : MiniPage {
    [SerializeField] StyleSheet styles;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        var topBar = CreateAndAddElement<MiniElement>("bar");
        topBar.CreateAndAddElement<Label>("").text = "Upcoming Sessions";

        var upcomingSessionsContainer = CreateAndAddElement<MiniElement>("section");

        var middleBar = CreateAndAddElement<MiniElement>("bar");
        middleBar.CreateAndAddElement<Label>("").text = "Additional Actions";

        var actionsContainer = CreateAndAddElement<MiniElement>("section");

        var practiceBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        practiceBtn.text = "Practice";
        practiceBtn.clicked += PracticeBtn_clicked;

        var scheduleAppointmentBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        scheduleAppointmentBtn.text = "Schedule An Appointment";
        scheduleAppointmentBtn.clicked += ScheduleAppointmentBtn_clicked;

        //APIManager.Instance.TryGetUpcomingAppointments(UserDataManager.Instance.userEmail);
    }

    private void ScheduleAppointmentBtn_clicked() {
        _router.Navigate(this, "ScheduleAppointmentPage");
    }

    private void PracticeBtn_clicked() {
        _router.Navigate(this, "OfflinePreGamePage");
    }

    private void SetupEvents() {
        //APIManager.Instance.OnAllAppointmentsRecieved += (appointments) => {
        //    print("All upcoming appointments recieved");
        //    print(appointments.Count);
        //};
    }
}