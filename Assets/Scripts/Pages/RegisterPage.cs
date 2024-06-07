using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class RegisterPage : MiniPage {
    [SerializeField] StyleSheet styles;

    Label errorText;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        SetupEvents();

        var container = CreateAndAddElement<ScrollView>("container");

        var welcomeLabel = CreateElement<Label>("welcome");
        welcomeLabel.text = "Welcome!";
        container.Add(welcomeLabel);

        errorText = CreateElement<Label>("errorText");
        container.Add(errorText);

        var emailLabel = CreateElement<Label>();
        emailLabel.text = "Enter Your Email";
        container.Add(emailLabel);

        var emailTf = CreateElement<TextField>();
        container.Add(emailTf);

        var usernameLabel = CreateElement<Label>();
        usernameLabel.text = "Enter Your Username";
        container.Add(usernameLabel);

        var usernameTf = CreateElement<TextField>();
        container.Add(usernameTf);

        var passwordLabel = CreateElement<Label>();
        passwordLabel.text = "Enter Your Password";
        container.Add(passwordLabel);

        var passwordTf = CreateElement<TextField>();
        passwordTf.isPasswordField = true;
        container.Add(passwordTf);

        var passwordLabel2 = CreateElement<Label>();
        passwordLabel2.text = "Re-Enter Your Password";
        container.Add(passwordLabel2);

        var passwordTf2 = CreateElement<TextField>();
        passwordTf2.isPasswordField = true;
        container.Add(passwordTf2);

        var roleDD = CreateElement<DropdownField>();
        roleDD.value = "Select your role";
        roleDD.choices.Add("Doctor");
        roleDD.choices.Add("Patient");
        container.Add(roleDD);

        var registerBtn = CreateElement<Button>("btn");
        registerBtn.text = "Sign Up";
        registerBtn.clicked += () => {
            errorText.text = "";

            if (emailTf.value == "") {
                errorText.text = "Email must not be left blank";
                return;
            }
            else if (passwordTf.value == "") {
                errorText.text = "Password must not be left blank";
            }
            else if (passwordTf.value != passwordTf2.value) {
                errorText.text = "Passwords do not match";
            }
            else if (!IsValidEmail(emailTf.value)) {
                errorText.text = "Please enter a valid email";
            }
            else if (roleDD.value == "Select your role" || string.IsNullOrEmpty(roleDD.value)) {
                errorText.text = "Please select a role";
            }
            else if (string.IsNullOrEmpty(usernameTf.value)) {
                errorText.text = "Please enter a username";
            }
            else {
                APIManager.Instance.TrySignUp(emailTf.value, usernameTf.value, passwordTf.value, roleDD.value);
            }
        };
        container.Add(registerBtn);

        var loginBtn = CreateElement<Button>("link");
        loginBtn.text = "Already have an account? Login here";
        loginBtn.clicked += () => {
            _router.Navigate(this, "LoginPage");
        };
        container.Add(loginBtn);
    }

    private void SetupEvents() {
        APIManager.Instance.UserRegistered += (model) => {
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

        APIManager.Instance.UserRegisterError += errorMsg => errorText.text = errorMsg;
    }

    bool IsValidEmail(string email) {
        var trimmedEmail = email.Trim();

        if (trimmedEmail.EndsWith(".")) {
            return false; // suggested by @TK-421
        }
        try {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == trimmedEmail;
        }
        catch {
            return false;
        }
    }
}