using MiniUI;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ScheduleAppointmentPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        errorText = CreateAndAddElement<Label>("errorText");

        var container = CreateAndAddElement<MiniElement>("container");

        container.CreateAndAddElement<Label>().text = "Enter the name of the person you want an appointment with";
        var nameTf = container.CreateAndAddElement<TextField>();

        container.CreateAndAddElement<Label>().text = "Enter Time";
        var datePicker = container.CreateAndAddElement<MiniElement>();

        var dayField = datePicker.CreateAndAddElement<DropdownField>("dateField");
        dayField.value = "Date";
        for (int i = 1; i <= 31; i++) {
            if (i < 10) {
                dayField.choices.Add("0" + i.ToString());
            }
            else {
                dayField.choices.Add(i.ToString());
            }
        }

        var monthField = datePicker.CreateAndAddElement<DropdownField>();
        monthField.value = "Month";
        for (int i = 1; i <= 12; i++) {
            if (i < 10) {
                monthField.choices.Add("0" + i.ToString());
            }
            else {
                monthField.choices.Add(i.ToString());
            }
        }

        var yearField = datePicker.CreateAndAddElement<DropdownField>();
        yearField.value = "Year";
        for (int i = 2024; i <= 2030; i++) {
            yearField.choices.Add(i.ToString());
        }

        var okayBtn = container.CreateAndAddElement<Button>("btn");
        okayBtn.text = "Create Appointment Request";
        okayBtn.clicked += () => {
            errorText.text = "";

            if (string.IsNullOrEmpty(nameTf.value)) {
                errorText.text = "Kindly enter the name of the person you want to schedule an appointment with";
                return;
            }

            string formattedDateString = $"{dayField.value}/{monthField.value}/{yearField.value} 07:30"; //setting time manually for now

            DateTime dateTime;

            try {
                dateTime = DateTime.Parse(formattedDateString);
            }
            catch (FormatException ex) {
                errorText.text = "Invalid date: " + ex.Message;
                return;
            }

            if (dateTime < DateTime.Now) {
                errorText.text = "Please select a date and time after the current date and time.";
                return;
            }

            APIManager.Instance.TryCreateAppointment(nameTf.value, dateTime);
        };

        var backBtn = CreateAndAddElement<Button>();
        backBtn.text = "Back";
        backBtn.clicked += () => {
            _router.Navigate(this, UserDataManager.Instance.userRole == "Patient" ? "PatientDashboard" : "DoctorDashboard");
        }; 
    }

    private void SetupEvents() {
        APIManager.Instance.AppointmentCreated += () => {
            errorText.text = "Appointment Created Successfuly";
            //GetComponent<MiniComponentRouter>().Navigate(this, "PatientDashboard");
        };

        APIManager.Instance.AppointmentCreationError += error => {
            errorText.text = error;
        };
    }
}