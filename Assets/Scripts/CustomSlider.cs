using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomSlider : Slider {
    private Text valueText;
    protected override void Start()
    {
        base.Start();
        this.valueText = GetComponentInChildren<Text>();
        this.onValueChanged.AddListener((float f) =>
        {
            if (valueText != null)
                valueText.text = f.ToString();
        });
    }
}
