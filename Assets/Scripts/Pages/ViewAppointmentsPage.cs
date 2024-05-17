using MiniUI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ViewAppointmentsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;
    DropdownField filtersDD;

    MiniElement container;
    MiniElement topSection;
    ScrollView midSection;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        errorText = CreateAndAddElement<Label>("errorText");

        container = CreateAndAddElement<MiniElement>("container");

        topSection = container.CreateAndAddElement<MiniElement>("top");

        filtersDD = topSection.CreateAndAddElement<DropdownField>();
        filtersDD.value = "Filter Appointments by status";
        filtersDD.choices.Add("All");
        filtersDD.choices.Add("Accepted");
        filtersDD.choices.Add("Rejected");
        filtersDD.choices.Add("Pending");

        filtersDD.RegisterValueChangedCallback((e) => {
            errorText.text = string.Empty;

            if (e.newValue == "All" || e.newValue == "Filter Appointments by status") {
                APIManager.Instance.TryGetAllAppointments();
            }
            else {
                APIManager.Instance.TryGetAllAppointmentsWithStatus(e.newValue);
            }
        });

        var refreshBtn = topSection.CreateAndAddElement<Button>();
        refreshBtn.text = "Refresh";
        refreshBtn.clicked += RefreshBtn_clicked; ;

        midSection = container.CreateAndAddElement<ScrollView>("middle");
    }

    private void RefreshBtn_clicked() {
        if (filtersDD.value == "Filter Appointments by status" || filtersDD.value == "All") {
            APIManager.Instance.TryGetAllAppointments();
        }
        else {
            APIManager.Instance.TryGetAllAppointmentsWithStatus(filtersDD.value);
        }
    }

    private void SetupEvents() {
        APIManager.Instance.OnAllAppointmentsWithStatusRecieved += appointments => {
            RenderAppointmentsList(appointments);
        };

        APIManager.Instance.OnGetAllAppointmentsWithStatusError += error => errorText.text = error;

        APIManager.Instance.OnAllAppointmentsRecieved += appointments => {
            RenderAppointmentsList(appointments);
        };

        APIManager.Instance.OnGetAllAppointmentsError += error => errorText.text = error;
    }

    private void RenderAppointmentsList(List<AppointmentDataModel> appointments) {
        midSection.Clear();
        foreach (AppointmentDataModel appointment in appointments) {
            AppointmentRow row = new AppointmentRow(appointment, () => {
                _router.NavigateWithData(this, "AppointmentDetailsPage", appointment);
            });

            midSection.Add(row);
        }
    }
}

class AppointmentRow : VisualElement {
    Label id;
    Label patientName;
    Label doctorName;
    Label time;
    Label status;

    public AppointmentRow(AppointmentDataModel appointmentData, Action onClickCallback) {
        this.id = new Label(appointmentData._id);
        this.time = new Label(appointmentData.time);
        this.status = new Label(appointmentData.status);

        if (appointmentData.requestSenderRole == "Patient") {
            this.patientName = new Label(appointmentData.requestSender);
            this.doctorName = new Label(appointmentData.appointmentWith);
        }
        else {
            this.patientName = new Label(appointmentData.appointmentWith);
            this.doctorName = new Label(appointmentData.requestSender);
        }

        this.Add(this.id);
        this.Add(this.patientName);
        this.Add(this.doctorName);
        this.Add(this.time);
        this.Add(this.status);

        this.RegisterCallback<ClickEvent>((evt) => {
            onClickCallback();
        });
    }
}