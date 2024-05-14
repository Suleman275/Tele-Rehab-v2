using MiniUI;
using UnityEngine.UIElements;
using UnityEngine;

public class OfflinePreGamePage : MiniPage {
    [SerializeField] StyleSheet styles;
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        var main = CreateAndAddElement<MiniElement>("main");

        var ballsDD = main.CreateAndAddElement<DropdownField>();
        ballsDD.value = "Select Number Of Balls";
        ballsDD.choices.Add("3");
        ballsDD.choices.Add("5");
        ballsDD.choices.Add("7");
        ballsDD.choices.Add("9");

        var wallDD = main.CreateAndAddElement<DropdownField>();
        wallDD.value = "Select Wall Height";
        wallDD.choices.Add("3");
        wallDD.choices.Add("4");
        wallDD.choices.Add("5");
        wallDD.choices.Add("6");
        
        var handDD = main.CreateAndAddElement<DropdownField>();
        handDD.value = "Which hand would you like to exercise?";
        handDD.choices.Add("Left");
        handDD.choices.Add("Right");

        var btn = main.CreateAndAddElement<Button>("btn");
        btn.text = "Start Game";
        btn.clicked += () => {
            //TODO: handle for error checking
            OfflineGameManager.Instance.StartGame(handDD.value, int.Parse(ballsDD.value), int.Parse(wallDD.value));

            _router.Navigate(this, "OfflineGameUI");
        };
    }
}