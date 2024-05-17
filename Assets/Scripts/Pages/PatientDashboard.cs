using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PatientDashboard : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        var practiceBtn = CreateAndAddElement<Button>("btn");
        practiceBtn.text = "Practice";
        practiceBtn.clicked += PracticeBtn_clicked;

        var scheduleBtn = CreateAndAddElement<Button>("btn");
        scheduleBtn.text = "Schedule a meeting";
        scheduleBtn.clicked += ScheduleBtn_clicked;

        var upcomingAppointmentsBtn = CreateAndAddElement<Button>("btn");
        upcomingAppointmentsBtn.text = "View Upcoming Appointments";
        upcomingAppointmentsBtn.clicked += UpcomingAppointmentsBtn_clicked; ;
    }

    private void UpcomingAppointmentsBtn_clicked() {
        _router.Navigate(this, "UpcomingAppointmentsPage");
    }

    private void ScheduleBtn_clicked() {
        _router.Navigate(this, "ScheduleAppointmentPage");
    }

    private void PracticeBtn_clicked() {
        _router.Navigate(this, "OfflinePreGamePage");
    }
}