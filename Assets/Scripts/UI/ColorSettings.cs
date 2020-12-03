using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;
using TMPro;

public class ColorSettings : MonoBehaviour
{
    [SerializeField]
    private ColorScheme currentColorScheme;
    
    [SerializeField]
    private ColorScheme customColorScheme;

    [SerializeField]
    private ColorScheme[] colorSchemes;

    [SerializeField]
    private IntVar currentColorSchemeIndex;

    [SerializeField]
    private ColorScheme defaultScheme;

    [SerializeField]
    private TextMeshProUGUI colorLabel;

    private void Start()
    {
        UpdateColorScheme();
    }

    public void ResetColors()
    {
        customColorScheme.CopyFrom(defaultScheme);
        PrepareCustomColor();
        currentColorScheme.CopyFrom(defaultScheme);
    }

    public void CycleColors(int increment)
    {
        currentColorSchemeIndex.Value = MathUtils.NegMod(currentColorSchemeIndex.Value +increment, colorSchemes.Length);
        UpdateColorScheme();
    }

    public void PrepareCustomColor()
    {
        currentColorSchemeIndex.Value = colorSchemes.Length-1;
        colorSchemes[currentColorSchemeIndex.Value].CopyFrom(currentColorScheme);
        UpdateColorScheme();
    }

    private void UpdateColorScheme()
    {
        ColorScheme scheme = colorSchemes[currentColorSchemeIndex.Value];
        currentColorScheme.CopyFrom(scheme);
        colorLabel.text = scheme.schemeName;
    }



}
