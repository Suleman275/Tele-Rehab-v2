using System;

[Serializable]
public class AppointmentDataModel {
    public string _id;
    public string requestSender;
    public string requestSenderRole;
    public string appointmentWith;
    public string time;
    public string status;
    public string roomId;

    public string getPatientName() {
        if (requestSenderRole == "Patient") {
            return requestSender;
        }
        else {
            return appointmentWith;
        }
    }

    public string getDoctorName() {
        if (requestSenderRole == "Doctor") {
            return requestSender;
        }
        else {
            return appointmentWith;
        }
    }
}
