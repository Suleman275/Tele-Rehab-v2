using MiniUI;
using UnityEngine.UIElements;
using UnityEngine;

public class WelcomePage : MiniPage {
    [SerializeField] StyleSheet styles;
    [SerializeField] Texture2D doctorImage;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter(); //common styles

        AddStyleSheet(styles); //page specific stylesheet

        var navbar = CreateAndAddElement<MiniElement>("navbar");
        navbar.CreateAndAddElement<Label>("title").text = "Tele-Rehabilitation";

        var main = CreateAndAddElement<MiniElement>("main");

        var leftPanel = main.CreateAndAddElement<MiniElement>("leftPanel");
        var rightPanel = main.CreateAndAddElement<MiniElement>("rightPanel");

        var img = leftPanel.CreateAndAddElement<Image>("image");
        img.image = doctorImage;


        var text = rightPanel.CreateAndAddElement<Label>("text");
        text.text = "Revolutionize your rehabilitation journey! Our innovative tele-rehabilitation platform empowers you to connect with qualified therapists and engage in effective exercise programs from the comfort of your home. Experience the flexibility and convenience of virtual therapy sessions, while receiving personalized guidance and real-time feedback to achieve your wellness goals.";

        var btn = rightPanel.CreateAndAddElement<Button>("btn");
        btn.text = "Get Started";
        btn.clicked += () => {
            _router.Navigate(this, "LoginPage");
        };
    }
}