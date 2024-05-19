using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;
using System.ComponentModel;

public class AppointmentDetailsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;

    AppointmentDataModel appointmentData;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        appointmentData = (AppointmentDataModel)_recievedData;

        errorText = CreateAndAddElement<Label>("errorText");

        var container = CreateAndAddElement<MiniElement>("container");

        var topSection = container.CreateAndAddElement<MiniElement>("top");

        var title = topSection.CreateAndAddElement<Label>();
        title.text = "Appointment Details";

        var midSection = container.CreateAndAddElement<MiniElement>("middle");

        midSection.CreateAndAddElement<Label>().text = "Date/Time: " + appointmentData.time;
        midSection.CreateAndAddElement<Label>().text = "Patient Name: " + appointmentData.getPatientName();
        midSection.CreateAndAddElement<Label>().text = "Appointment Status: " + appointmentData.status;

        if (UserDataManager.Instance.userRole == "Doctor") {
            if (appointmentData.status == "Pending") {
                var acceptBtn = container.CreateAndAddElement<Button>("btn");
                acceptBtn.text = "Accept Appointment Request";
                acceptBtn.clicked += () => {
                    UpdateAppointmentStatus("Accepted");
                };

                var rejectBtn = container.CreateAndAddElement<Button>("btn");
                rejectBtn.text = "Reject Appointment Request";
                rejectBtn.clicked += () => {
                    UpdateAppointmentStatus("Rejected");
                };
            }
            else if (appointmentData.status == "Accepted") {
                var startBtn = container.CreateAndAddElement<Button>("btn");
                startBtn.text = "Start this session";
                startBtn.clicked += () => {
                    _router.NavigateWithData(this, "RoomDetailsPage", appointmentData.roomId);
                };

                var cancelBtn = container.CreateAndAddElement<Button>("btn");
                cancelBtn.text = "Cancel this appointment";
                cancelBtn.clicked += () => {
                    UpdateAppointmentStatus("Cancelled");
                };
            }
        }
        else {
            if (appointmentData.status == "Accepted") {
                var startBtn = container.CreateAndAddElement<Button>("btn");
                startBtn.text = "Join this session";
                startBtn.clicked += () => {
                    _router.NavigateWithData(this, "RoomDetailsPage", appointmentData.roomId);
                };
            }
            }

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back";
        backBtn.clicked += () => {
            _router.Navigate(this, UserDataManager.Instance.userRole == "Patient" ? "PatientDashboard" : "DoctorDashboard");
        };
    }

    private void UpdateAppointmentStatus(string status) {
        print("trying to update appointment status");
        appointmentData.status = status;
        APIManager.Instance.TryUpdateAppointmentData(appointmentData);
    }

    private void SetupEvents() {
        APIManager.Instance.OnAppointmentUpdateError += error => errorText.text = error;

        APIManager.Instance.OnAppointmentUpdated += () => {
            _recievedData = appointmentData;
            ReRenderPage();
            errorText.text = "Your appointment information has been updated";
        };
    }
}