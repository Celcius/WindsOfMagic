using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AmoaebaUtils;

public class ColorTypeVar : ScriptVar<ColorType>
{
    public static string GetColorTypeName(ColorType type)
    {
        switch(type)
        {
            case ColorType.PlayerColor:
                return "Player Color";
            case ColorType.PlayerBulletsColor:
                return "Player Bullet Color";
            case ColorType.EnemyColor:
                return "Enemy Color";
            case ColorType.EnemyBulletsColor:
                return "Enemy Bullet Color";
            case ColorType.PickupColor:
                return "Upgrade Color";
            case ColorType.HealthColor:
                return "Health Color";
            case ColorType.RewindColor:
                return "Rewind Color";
        }
        return "Unexpected Color";
    }

}
