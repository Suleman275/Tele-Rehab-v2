using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;
using System.Collections.Generic;

public class UpcomingAppointmentsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    MiniElement midSection;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        var container = CreateAndAddElement<MiniElement>("container");

        var topSection = container.CreateAndAddElement<MiniElement>("top");

        var title = topSection.CreateAndAddElement<Label>();
        title.text = "Upcoming Sessions";

        midSection = container.CreateAndAddElement<MiniElement>("middle");

        APIManager.Instance.TryGetAllAppointmentsByParticipant(UserDataManager.Instance.username);

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back";
        backBtn.clicked += () => {
            _router.Navigate(this, UserDataManager.Instance.userRole == "Patient" ? "PatientDashboard" : "DoctorDashboard");
        };
    }

    private void SetupEvents() {
        APIManager.Instance.OnAllAppointmentsRecieved += appointments => {
            //print($"got {appointments.Count} appointments");
            RenderAppointmentsList(appointments);
        };
    }

    private void RenderAppointmentsList(List<AppointmentDataModel> appointments) {
        midSection.Clear();
        midSection.Add(new AppointmentRow(null, null, true, null)); // creating column headers

        foreach (AppointmentDataModel appointment in appointments) {
            AppointmentRow row = new AppointmentRow(appointment, UserDataManager.Instance.username, false, () => {
                _router.NavigateWithData(this, "AppointmentDetailsPage", appointment);
            });

            midSection.Add(row);
        }
    }
}