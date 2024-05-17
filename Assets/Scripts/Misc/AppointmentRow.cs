using System;
using UnityEngine.UIElements;

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

        this.patientName = new Label(appointmentData.getPatientName());
        this.doctorName = new Label(appointmentData.getDoctorName());
        
        this.Add(this.id);
        this.Add(this.patientName);
        this.Add(this.doctorName);
        this.Add(this.time);
        this.Add(this.status);

        this.RegisterCallback<ClickEvent>((evt) => {
            onClickCallback();
        });
    }

    public AppointmentRow() { //use this for colum headers
        this.id = new Label("ID");
        this.patientName = new Label("Patient Name");
        this.doctorName = new Label("Doctor Name");
        this.time = new Label("Time");
        this.status = new Label("Status");

        this.Add(this.id);
        this.Add(this.patientName);
        this.Add(this.doctorName);
        this.Add(this.time);
        this.Add(this.status);
    }
}