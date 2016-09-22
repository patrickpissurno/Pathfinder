using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CustomSlider : Slider {
    [SerializeField]
    private Text valueText;
    protected override void Start()
    {
        base.Start();
        this.onValueChanged.AddListener((float f) =>
        {
            if (valueText != null)
                valueText.text = f.ToString();
        });
    }
}
