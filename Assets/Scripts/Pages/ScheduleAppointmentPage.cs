using MiniUI;
using UnityEngine.UIElements;

public class ScheduleAppointmentPage : MiniPage {

    Label errorText;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        SetupEvents();

        errorText = CreateAndAddElement<Label>("errorText");

        var nameTF = CreateAndAddElement<TextField>();
        var timeTF = CreateAndAddElement<TextField>();

        var btn = CreateAndAddElement<Button>("btn");
        btn.text = "Create Appointment Request";
        btn.clicked += () => {
            errorText.text = "";
            APIManager.Instance.TryCreateAppointment(nameTF.value, timeTF.value);
        };
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