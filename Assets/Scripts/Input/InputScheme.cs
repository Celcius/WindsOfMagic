﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputScheme : ScriptableObject
{
    public string PresetName ="";
    public bool UseJoysticks = false;
    public KeyCode PositiveXKey = KeyCode.D;
    public KeyCode NegativeXKey = KeyCode.A;
    public KeyCode PositiveYKey = KeyCode.W;
    public KeyCode NegativeYKey = KeyCode.S;
    public KeyCode PositiveShootXKey = KeyCode.LeftArrow;
    public KeyCode NegativeShootXKey = KeyCode.UpArrow;
    public KeyCode PositiveShootYKey = KeyCode.DownArrow;
    public KeyCode NegativeShootYKey = KeyCode.RightArrow;
    public KeyCode ReverseTimeKey = KeyCode.Space;

    public KeyCode PauseKey = KeyCode.Escape;

    [SerializeField]
    public bool IsPSInput;

    public enum KeyActionType
    {
        PositiveXMove,
        NegativeXKeyMove,
        PositiveYKeyMove,
        NegativeYKeyMove,
        PositiveShootX,
        NegativeShootX,
        PositiveShootY,
        NegativeShootY,
        ReverseTime,
        Pause,
    }

    public void CopyFrom(InputScheme other)
    {
        this.UseJoysticks = other.UseJoysticks;
        this.PositiveXKey = other.PositiveXKey;
        this.NegativeXKey = other.NegativeXKey;
        this.PositiveYKey = other.PositiveYKey;
        this.NegativeYKey = other.NegativeYKey;
        this.PositiveShootXKey = other.PositiveShootXKey;
        this.NegativeShootXKey = other.NegativeShootXKey;
        this.PositiveShootYKey = other.PositiveShootYKey;
        this.NegativeShootYKey = other.NegativeShootYKey;
        this.ReverseTimeKey = other.ReverseTimeKey;
        this.PauseKey = other.PauseKey;
    }

    public void SetActionKey(KeyActionType type, KeyCode code)
    {
        switch(type)
        {      
            case KeyActionType.PositiveXMove:
                PositiveXKey = code;
                break;
            case KeyActionType.NegativeXKeyMove:
                NegativeXKey = code;
                break;
            case KeyActionType.PositiveYKeyMove:
                PositiveYKey = code;
                break;
            case KeyActionType.NegativeYKeyMove:
                NegativeYKey = code;
                break;
            case KeyActionType.PositiveShootX:
                PositiveShootXKey = code;
                break;
            case KeyActionType.NegativeShootX:
                NegativeShootXKey = code;
                break;
            case KeyActionType.PositiveShootY:
                PositiveShootYKey = code;
                break;
            case KeyActionType.NegativeShootY:
                NegativeShootYKey = code;
                break;
            case KeyActionType.ReverseTime:
                ReverseTimeKey = code;
                break;
            case KeyActionType.Pause:
                PauseKey = code;
                break;
        }
    }

    public KeyCode CodeFromType(KeyActionType type)
    {
        switch(type)
        {
            case KeyActionType.PositiveXMove:
                return PositiveXKey;
            case KeyActionType.NegativeXKeyMove:
                return NegativeXKey;
            case KeyActionType.PositiveYKeyMove:
                return PositiveYKey;
            case KeyActionType.NegativeYKeyMove:
                return NegativeYKey;
            case KeyActionType.PositiveShootX:
                return PositiveShootXKey;
            case KeyActionType.NegativeShootX:
                return NegativeShootXKey;
            case KeyActionType.PositiveShootY:
                return PositiveShootYKey;
            case KeyActionType.NegativeShootY:
                return NegativeShootYKey;
            case KeyActionType.ReverseTime:
                return ReverseTimeKey;
            case KeyActionType.Pause:
                return PauseKey;
        }
        return KeyCode.None;
    }

    public string GetCodeString(KeyCode code)
    {
        if(!UseJoysticks)
        {
            return code.ToString();
        }

        switch (code)
        {
            case KeyCode.JoystickButton0:
                return IsPSInput? "Square": "A";
            case KeyCode.JoystickButton1:
                return IsPSInput? "X": "B";
            case KeyCode.JoystickButton2:
                return IsPSInput? "Circle": "X";
            case KeyCode.JoystickButton3:
                return IsPSInput? "Triangle": "Y";
            case KeyCode.JoystickButton4:
                return IsPSInput? "L1": "LB";
            case KeyCode.JoystickButton5:
                return IsPSInput? "R1": "RB";
            case KeyCode.JoystickButton6:
                return IsPSInput? "L2": code.ToString();
            case KeyCode.JoystickButton7:
                return IsPSInput? "R2": "Start";
            case KeyCode.JoystickButton8:
                return IsPSInput? "Share": code.ToString();
            case KeyCode.JoystickButton9:
                return IsPSInput? "Options": code.ToString();
            case KeyCode.JoystickButton10:
                return IsPSInput? "L3": code.ToString();
            case KeyCode.JoystickButton11:
                return IsPSInput? "R3": code.ToString();
            case KeyCode.JoystickButton12:
                return IsPSInput? "Play": code.ToString();
        }      

        return code.ToString();  
    }
}
