using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using TMPro;


[RequireComponent(typeof(FlexibleColorPicker))]
public class FlexibleColorSetup : MonoBehaviour
{
    [SerializeField]
    private ColorTypeVar pickerColor;
    
    [SerializeField]
    private ColorScheme currentColorScheme;
    
    [SerializeField]
    private ColorScheme defaultColorScheme;

    [SerializeField]
    private TextMeshProUGUI variableName;

    private FlexibleColorPicker picker = null;

    private void Start() 
    {
        picker = GetComponent<FlexibleColorPicker>();
    }

    private void OnEnable()
    {
        if(picker == null)
        {
            picker = GetComponent<FlexibleColorPicker>();
        }
        Assert.IsFalse(pickerColor.Value == ColorType.None);

        variableName.text = ColorTypeVar.GetColorTypeName(pickerColor.Value);
        picker.color = currentColorScheme.GetColor(pickerColor.Value);
    }

    public void Reset()
    {
        Color color = defaultColorScheme.GetColor(pickerColor.Value);
        picker.color = color;
        currentColorScheme.SetColor(pickerColor.Value, color);
    }

    public void Confirm()
    {
        Color color = picker.color;
        color.a = 1.0f;
        currentColorScheme.SetColor(pickerColor.Value, color);
        picker.transform.parent.gameObject.SetActive(false);
    }
}
