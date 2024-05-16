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
    }

    private void ViewAppointmentsBtn_clicked() {
        _router.Navigate(this, "ViewAppointmentsPage");
    }
}