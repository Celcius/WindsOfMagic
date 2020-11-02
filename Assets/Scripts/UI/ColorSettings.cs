using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorSettings : MonoBehaviour
{
    [SerializeField]
    private ColorScheme currentColorScheme;

    [SerializeField]
    private ColorScheme baseColorScheme;

    public void ResetColors()
    {
        currentColorScheme.CopyFrom(baseColorScheme);
    }


}
