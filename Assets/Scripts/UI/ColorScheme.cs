using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColorScheme : ScriptableObject
{
    public delegate void OnValueChange();
    public event OnValueChange OnChange;

    [SerializeField]
    private Color PlayerColor;

    [SerializeField]
    private Color PlayerBulletsColor;
    
    [SerializeField]
    private Color EnemyColor;
    
    [SerializeField]
    private Color EnemyBulletsColor;
    
    [SerializeField]
    private Color PickupColor;
    
    [SerializeField]
    private Color HealthColor;

    [SerializeField]
    private Color RewindColor;

    public void CopyFrom(ColorScheme other)
    {
        this.PlayerColor = other.PlayerColor;
        this.PlayerBulletsColor = other.PlayerBulletsColor;
        this.EnemyColor = other.EnemyColor;
        this.EnemyBulletsColor = other.EnemyBulletsColor;
        this.PickupColor = other.PickupColor;
        this.HealthColor = other.HealthColor;
        this.RewindColor = other.RewindColor;

        OnChange?.Invoke();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        OnChange?.Invoke();
    }
#endif

    public void SetColor(ColorType colorType, Color color)
    {
        switch(colorType)
        {
            case ColorType.PlayerColor:
                this.PlayerColor = color;
                break;
            case ColorType.PlayerBulletsColor:
                this.PlayerBulletsColor = color;
                break;
            case ColorType.EnemyColor:
                this.EnemyColor = color;
                break;
            case ColorType.EnemyBulletsColor:
                this.EnemyBulletsColor = color;
                break;
            case ColorType.PickupColor:
                this.PickupColor = color;
                break;
            case ColorType.HealthColor:
                this.HealthColor = color;
                break;
            case ColorType.RewindColor:
                this.RewindColor = color;
                break;
        }

        OnChange?.Invoke();
    }

    public Color GetColor(ColorType type)
    {
        Color color = Color.magenta;
        switch(type)
        {
            case ColorType.None:
            case ColorType.PlayerColor:
                color = PlayerColor;
                break;
            case ColorType.PlayerBulletsColor:
                color = PlayerBulletsColor;
                break;
            case ColorType.EnemyColor:
                color = EnemyColor;
                break;
            case ColorType.EnemyBulletsColor:
                color = EnemyBulletsColor;
                break;
            case ColorType.PickupColor:
                color = PickupColor;
                break;
            case ColorType.HealthColor:
                color = HealthColor;
                break;
            case ColorType.RewindColor:
                color = RewindColor;
                break;
        }
        color.a = 1.0f;
        return color;
    }
}
