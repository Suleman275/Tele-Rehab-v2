using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;
using System.ComponentModel;

public class AppointmentDetailsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;

    AppointmentDataModel data;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        data = (AppointmentDataModel)_recievedData;

        errorText = CreateAndAddElement<Label>("errorText");

        var container = CreateAndAddElement<MiniElement>("container");

        var topSection = container.CreateAndAddElement<MiniElement>("top");

        var title = topSection.CreateAndAddElement<Label>();
        title.text = "Appointment Details";

        var midSection = container.CreateAndAddElement<MiniElement>("middle");

        midSection.CreateAndAddElement<Label>().text = "Date/Time: " + data.time;
        midSection.CreateAndAddElement<Label>().text = "Patient Name: " + data.requestSenderRole == "Patient"? data.requestSender : data.appointmentWith;
        midSection.CreateAndAddElement<Label>().text = "Appointment Status: " + data.status;

        if (data.status == "Pending") {
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
        else if (data.status == "Accepted") {
            var startBtn = container.CreateAndAddElement<Button>("btn");
            startBtn.text = "Start this session";
            startBtn.clicked += () => { };

            var cancelBtn = container.CreateAndAddElement<Button>("btn");
            cancelBtn.text = "Cancel this appointment";
            cancelBtn.clicked += () => {
                UpdateAppointmentStatus("Cancelled");
            };
        }

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back to all appointments";
        backBtn.clicked += () => {
            _router.Navigate(this, "ViewAppointmentsPage");
        };
    }

    private void UpdateAppointmentStatus(string status) {
        print("trying to update appointment status");
        data.status = status;
        APIManager.Instance.TryUpdateAppointmentData(data);
    }

    private void SetupEvents() {
        APIManager.Instance.OnAppointmentUpdateError += error => errorText.text = error;

        APIManager.Instance.OnAppointmentUpdated += () => {
            _recievedData = data;
            ReRenderPage();
            errorText.text = "Your appointment information has been updated"; //should probably have a success text label instead
        };
    }
}