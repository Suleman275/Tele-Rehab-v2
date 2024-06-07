using UnityEngine;
using MiniUI;
using UnityEngine.UIElements;

public class LoginPage : MiniPage {
    [SerializeField] StyleSheet styles;

    private Label errorText;
    
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        var container = CreateAndAddElement<MiniElement>("container");

        var welcomeLabel = container.CreateAndAddElement<Label>("welcome");
        welcomeLabel.text = "Welcome Back";

        errorText = container.CreateAndAddElement<Label>("errorText");

        var emailLabel = container.CreateAndAddElement<Label>();
        emailLabel.text = "Enter Your Email";

        var emailTf = container.CreateAndAddElement<TextField>();

        var passwordLabel = container.CreateAndAddElement<Label>();
        passwordLabel.text = "Enter Your Password";

        var passwordTf = container.CreateAndAddElement<TextField>();
        passwordTf.isPasswordField = true;

        var loginBtn = container.CreateAndAddElement<Button>("btn");
        loginBtn.text = "Login";
        loginBtn.clicked += () => {
            errorText.text = "";

            if (emailTf.value == "") {
                errorText.text = "Email must not be left blank";
                return;
            }
            else if (passwordTf.value == "") {
                errorText.text = "Password must not be left blank";
            }
            else {
                APIManager.Instance.TrySignIn(emailTf.value, passwordTf.value);
            }
        };

        var registerBtn = container.CreateAndAddElement<Button>("link");
        registerBtn.text = "Don't have an account? Sign Up Here";
        registerBtn.clicked += () => {
            _router.Navigate(this, "RegisterPage");
        };
    }

    private void SetupEvents() {
        APIManager.Instance.UserSignedIn += model => {
            //login to vivox as well
            //UnityServicesManager.Instance.LoginVivox(); //commenting temporarily

            if (model.role == "Patient") {
                _router.Navigate(this, "PatientDashboard");
                AstraManager.Instance.StartBodyStream();
            }
            else {
                _router.Navigate(this, "DoctorDashboard");
            }
        };

        APIManager.Instance.UserSignInFailed += errorMsg => errorText.text = errorMsg;
    }
}