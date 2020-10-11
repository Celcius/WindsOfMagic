using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BlendModes;

[RequireComponent(typeof(BlendModeEffect))]
[ExecuteInEditMode]
public class BlendColorApplier : MonoBehaviour
{
    [SerializeField]
    private ColorType type = ColorType.None;

    [SerializeField]
    private ColorScheme currentColors;
    
    private BlendModeEffect modeEffect;
    
    private void Start()
    {
        if(currentColors != null)
        {
            currentColors.OnChange -= this.ApplyColor;  
            currentColors.OnChange += this.ApplyColor;
        }
        ApplyColor();
    }

    private void OnDestroy() 
    {
        currentColors.OnChange -= this.ApplyColor;    
    }

    private void ApplyColor()
    {
        if(currentColors == null)
        {
            return;
        }
        if(modeEffect == null)
        {
            modeEffect = GetComponent<BlendModeEffect>();
        }
        Color color = currentColors.GetColor(type);
        color.a = 1.0f;
        modeEffect.OverlayColor = color;
    }

#if UNITY_EDITOR
    private void OnAwake()
    {
        currentColors.OnChange -= this.ApplyColor;  
        currentColors.OnChange += this.ApplyColor;
        ApplyColor();
    }

    private void OnValidate ()
    {
        ApplyColor();
    }
#endif
}
