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
        text.text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla pulvinar, nulla nec feugiat sollicitudin, augue leo finibus libero, eget tempus odio nulla sit amet ex. Sed egestas ex arcu, nec fringilla nulla eleifend id. Proin facilisis volutpat tortor non semper. In maximus odio risus, et congue felis rutrum sed. In hac habitasse platea dictumst. Sed eget interdum nisi. Mauris aliquam ante id consectetur accumsan. Morbi non fringilla libero, eget venenatis turpis. Nam egestas sapien rutrum, dignissim augue vitae, venenatis nisi. Nam sit amet neque orci. Vivamus sit amet nisi eros.";

        var btn = rightPanel.CreateAndAddElement<Button>("btn");
        btn.text = "Get Started";
        btn.clicked += () => {
            _router.Navigate(this, "LoginPage");
        };
    }
}