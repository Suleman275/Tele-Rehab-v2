using UnityEngine;
using UnityEngine.UIElements;
using MiniUI;
using System.ComponentModel;

public class AppointmentDetailsPage : MiniPage {
    protected override void RenderPage() {
        InheritStylesFromComponentRouter();

        AppointmentDataModel data = (AppointmentDataModel)_recievedData;

        var errorText = CreateAndAddElement<Label>("errorText");

        var container = CreateAndAddElement<MiniElement>("container");


    }
}