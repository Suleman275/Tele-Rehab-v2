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

        var dateField = datePicker.CreateAndAddElement<DropdownField>("dateField");
        dateField.value = "Date";
        for (int i = 1; i <= 31; i++) {
            if (i < 10) {
                dateField.choices.Add("0" + i.ToString());
            }
            else {
                dateField.choices.Add(i.ToString());
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
            print("clicked");
            string formattedDateString = $"{dateField.value}/{monthField.value}/{yearField.value}";

            DateTime dateTime = DateTime.Parse(formattedDateString);
            print(dateTime.ToString()); // Output: Friday, 2024-05-17 10:30:00 AM
            
        };

        //var btn = CreateAndAddElement<Button>("btn");
        //btn.text = "Create Appointment Request";
        //btn.clicked += () => {
        //    errorText.text = "";
        //    APIManager.Instance.TryCreateAppointment(nameTF.value, timeTF.value);
        //};
    }

    private void SetupEvents() {
        APIManager.Instance.AppointmentCreated += () => {
            GetComponent<MiniComponentRouter>().Navigate(this, "PatientDashboard");
        };

        APIManager.Instance.AppointmentCreationError += error => { 
            errorText.text = error; 
        };
    } 
}