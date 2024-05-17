using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;

public class UpcomingAppointmentsPage : MiniPage {
    [SerializeField] StyleSheet styles;

    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AddStyleSheet(styles);

        var container = CreateAndAddElement<MiniElement>("container");

        var topSection = container.CreateAndAddElement<MiniElement>("top");

        var title = topSection.CreateAndAddElement<Label>();
        title.text = "Upcoming Sessions";

        var midSection = container.CreateAndAddElement<MiniElement>("middle");


    }
}