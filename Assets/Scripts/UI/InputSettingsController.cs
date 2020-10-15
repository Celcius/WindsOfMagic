using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using AmoaebaUtils;
public class InputSettingsController : MonoBehaviour
{       
    [SerializeField]
    private InputHandler inputHandler;

    [SerializeField]
    private InputSchemeVar currentScheme;
    
    [SerializeField]
    private InputScheme customScheme;

    [SerializeField]
    private Transform[] joystickItems;
    
    [SerializeField]
    private Transform[] nonJoystickItems;

    [SerializeField]
    private Toggle joystickToggle;

    [SerializeField]
    private InputScheme[] presets;

    [SerializeField]
    private IntVar currentPreset;

    [SerializeField]
    private TextMeshProUGUI presetLabel;

    private bool isCyclingPreset = false;

    // Start is called before the first frame update
    private void Start()
    {
        currentScheme.OnChange += OnInputChange;    
        currentScheme.Value = presets[currentPreset.Value];
    }

    private void OnDestroy() 
    {
        currentScheme.OnChange -= OnInputChange; 
    }
    
    private void OnInputChange(InputScheme oldVal, InputScheme newVal)
    {
        UpdateInputItems();
        UpdatePresets();
    }

    private void UpdateInputItems()
    {
        bool isJoystick = joystickToggle.isOn = currentScheme.Value.UseJoysticks;
        
        foreach(Transform obj in joystickItems)
        {
            obj.gameObject.SetActive(isJoystick);
        }

        foreach(Transform obj in nonJoystickItems)
        {
            obj.gameObject.SetActive(!isJoystick);
        }
    }

    public void OnJoystickToggle()
    {
        if(!isCyclingPreset)
        {
            bool isOn = joystickToggle.isOn;
            inputHandler.CopyCurrentSchemeToCustom(true);
            joystickToggle.isOn = customScheme.UseJoysticks = isOn;
        }
        
        UpdateInputItems();
    }

    public void CyclePreset(int increment)
    {
        isCyclingPreset = true;
        currentPreset.Value = MathUtils.NegMod(currentPreset.Value + increment, presets.Length);
        currentScheme.Value = presets[currentPreset.Value];
        isCyclingPreset = false;
    }

    private void UpdatePresets()
    {
        presetLabel.text = currentScheme.Value.PresetName;
        for(int i = 0; i < presets.Length; i++)
        {
            if(presets[i].GetInstanceID() == currentScheme.Value.GetInstanceID())
            {
                currentPreset.Value = i;
                break;
            }
        }
    }
}
