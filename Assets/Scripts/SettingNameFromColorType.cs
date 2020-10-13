using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SettingNameFromColorType : MonoBehaviour
{
    [SerializeField]
    private ColorType type;

    [SerializeField]
    private TextMeshProUGUI label;

    [SerializeField]
    private Image colorImage;

    [SerializeField]
    private ColorTypeVar colorPickerVar;

    [SerializeField]
    private ColorScheme currentColorScheme;

    private void Start() 
    {
        label.text = ColorTypeVar.GetColorTypeName(type);
        currentColorScheme.OnChange += OnColorChange;
        OnColorChange();
    }

    private void OnDestroy() 
    {
        currentColorScheme.OnChange -= OnColorChange;
    }

    private void OnColorChange()
    {
        Color color = currentColorScheme.GetColor(type);
        colorImage.color = color;
    }


    public void OnClick()
    {
        colorPickerVar.Value = type;
    }
}
