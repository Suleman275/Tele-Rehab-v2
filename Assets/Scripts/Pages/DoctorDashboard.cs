using MiniUI;
using UnityEngine.UIElements;

public class DoctorDashboard : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        var viewAppointmentsBtn = CreateAndAddElement<Button>("btn");
        viewAppointmentsBtn.text = "View Appointments";
        viewAppointmentsBtn.clicked += ViewAppointmentsBtn_clicked;

        //var scheduleBtn = CreateAndAddElement<Button>("btn");
        //scheduleBtn.text = "Schedule a meeting";
        //scheduleBtn.clicked += ScheduleBtn_clicked;

        var upcomingAppointmentsBtn = CreateAndAddElement<Button>("btn");
        upcomingAppointmentsBtn.text = "View Upcoming Appointments";
        upcomingAppointmentsBtn.clicked += UpcomingAppointmentsBtn_clicked;
    }

    private void UpcomingAppointmentsBtn_clicked() {
        _router.Navigate(this, "UpcomingAppointmentsPage");
    }

    private void ViewAppointmentsBtn_clicked() {
        _router.Navigate(this, "ViewAppointmentsPage");
    }
}