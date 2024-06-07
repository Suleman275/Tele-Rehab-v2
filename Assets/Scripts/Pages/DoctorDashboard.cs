using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class DoctorDashboard : MiniPage {
    [SerializeField] StyleSheet styles;

    MiniElement upcomingSessionsContainer;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        var topBar = CreateAndAddElement<MiniElement>("bar");
        topBar.CreateAndAddElement<Label>("").text = "Upcoming Sessions";

        upcomingSessionsContainer = CreateAndAddElement<MiniElement>("section");

        var middleBar = CreateAndAddElement<MiniElement>("bar");
        middleBar.CreateAndAddElement<Label>("").text = "Additional Actions";

        var actionsContainer = CreateAndAddElement<MiniElement>("section");

        var appointmentsBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        appointmentsBtn.text = "View All Appointments";
        appointmentsBtn.clicked += AppointmentsBtn_clicked; ;

        var scheduleAppointmentBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        scheduleAppointmentBtn.text = "Schedule An Appointment";
        scheduleAppointmentBtn.clicked += ScheduleAppointmentBtn_clicked;

        var pastSessionsBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        pastSessionsBtn.text = "View Past Sessions";
        pastSessionsBtn.clicked += PastSessionsBtn_clicked;

        APIManager.Instance.TryGetUpcomingAppointments(UserDataManager.Instance.username);
    }

    private void PastSessionsBtn_clicked() {
        _router.Navigate(this, "PastSessionsPage");
    }

    private void AppointmentsBtn_clicked() {
        _router.Navigate(this, "ViewAppointmentsPage"); 
    }

    private void ScheduleAppointmentBtn_clicked() {
        _router.Navigate(this, "ScheduleAppointmentPage");
    }

    private void SetupEvents() {
        APIManager.Instance.OnAllAppointmentsRecieved += (appointments) => {
            upcomingSessionsContainer.Clear();

            int count = appointments.Count > 3 ? 3 : appointments.Count; //showing only first 3

            for (int i = 0; i < count; i++) {
                var appointment = appointments[i];

                var appointmentcontainer = upcomingSessionsContainer.CreateAndAddElement<MiniElement>("appointment-card");
                appointmentcontainer.CreateAndAddElement<Label>().text = appointments[i].time.ToString("dd-MM-yyy HH:mm");
                appointmentcontainer.CreateAndAddElement<Label>().text = appointments[i].getPatientName();
                appointmentcontainer.CreateAndAddElement<Label>().text = appointments[i].status;

                appointmentcontainer.RegisterCallback<ClickEvent>((e) => {
                    _router.NavigateWithData(this, "AppointmentDetailsPage", appointment);
                });

            }
        };

        APIManager.Instance.OnGetAllAppointmentsError += (error) => {
            upcomingSessionsContainer.Clear();
            upcomingSessionsContainer.CreateAndAddElement<Label>().text = "You have no upcoming sessions!";
        };
    }
}