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

    public Color GetColor(ColorType type)
    {
        switch(type)
        {
            case ColorType.None:
                return Color.magenta;
            case ColorType.PlayerColor:
                return PlayerColor;
            case ColorType.PlayerBulletsColor:
                return PlayerBulletsColor;
            case ColorType.EnemyColor:
                return EnemyColor;
            case ColorType.EnemyBulletsColor:
                return EnemyBulletsColor;
            case ColorType.PickupColor:
                return PickupColor;
            case ColorType.HealthColor:
                return HealthColor;
            case ColorType.RewindColor:
                return RewindColor;
        }
        return Color.magenta;
    }
}
