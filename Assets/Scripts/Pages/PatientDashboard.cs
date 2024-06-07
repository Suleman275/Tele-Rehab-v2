using MiniUI;
using UnityEngine;
using UnityEngine.UIElements;

public class PatientDashboard : MiniPage {
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

        var practiceBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        practiceBtn.text = "Practice";
        practiceBtn.clicked += PracticeBtn_clicked;

        var scheduleAppointmentBtn = actionsContainer.CreateAndAddElement<Button>("btn");
        scheduleAppointmentBtn.text = "Schedule An Appointment";
        scheduleAppointmentBtn.clicked += ScheduleAppointmentBtn_clicked;

        APIManager.Instance.TryGetUpcomingAppointments(UserDataManager.Instance.username);
    }

    private void ScheduleAppointmentBtn_clicked() {
        _router.Navigate(this, "ScheduleAppointmentPage");
    }

    private void PracticeBtn_clicked() {
        _router.Navigate(this, "OfflinePreGamePage");
    }

    private void SetupEvents() {
        APIManager.Instance.OnAllAppointmentsRecieved += (appointments) => {

            //print(appointments.Count);
            upcomingSessionsContainer.Clear();

            int count = appointments.Count > 3 ? 3 : appointments.Count; //showing only first 3

            for (int i = 0; i < count; i++) { 
                var appointment = appointments[i];

                var appointmentcontainer = upcomingSessionsContainer.CreateAndAddElement<MiniElement>("appointment-card");
                appointmentcontainer.CreateAndAddElement<Label>().text = appointments[i].time.ToString("dd-MM-yyy HH:mm");
                appointmentcontainer.CreateAndAddElement<Label>().text = appointments[i].getDoctorName();
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