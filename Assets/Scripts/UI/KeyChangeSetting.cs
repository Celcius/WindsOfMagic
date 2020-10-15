using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteInEditMode]
public class KeyChangeSetting : MonoBehaviour
{
    [SerializeField]
    private InputHandler inputHandler;

    [SerializeField]
    private InputSchemeVar currentScheme;

    [SerializeField]
    private InputScheme.KeyActionType actionType;

    [SerializeField]
    private Button button;

    [SerializeField]
    private TextMeshProUGUI keyLabel;

    [SerializeField]
    private TextMeshProUGUI actionLabel;

    private void Start()
    {
        currentScheme.OnChange += OnInputChange;
        UpdateLabels();
    }

    private void OnDestroy()
    {
        currentScheme.OnChange -= OnInputChange;
    }

    private void OnInputChange(InputScheme oldVal, InputScheme newVal)
    {
        UpdateLabels();
    }

    public void ChangeSetting()
    {
        button.interactable = false;
        keyLabel.text = "Press Any Key...";
        inputHandler.GetNextKey(OnKey, OnCancelAction);
    }

    public void OnKey(KeyCode key)
    {
        inputHandler.CopyCurrentSchemeToCustom(true);
        currentScheme.Value.SetActionKey(actionType, key);
        UpdateLabels();
        button.interactable = true;
    }

    private void UpdateLabels()
    {
        keyLabel.text = currentScheme.Value.CodeFromType(actionType).ToString();
        string actionLabelText = "";

        switch(actionType)
        {      
            case InputScheme.KeyActionType.PositiveXMove:
                actionLabelText = "Move Right";
                break;
            case InputScheme.KeyActionType.NegativeXKeyMove:
                actionLabelText = "Move Left";
                break;
            case InputScheme.KeyActionType.PositiveYKeyMove:
                actionLabelText = "Move Up";
                break;
            case InputScheme.KeyActionType.NegativeYKeyMove:
                actionLabelText = "Move Down";
                break;
            case InputScheme.KeyActionType.PositiveShootX:
                actionLabelText = "Shoot Right";
                break;
            case InputScheme.KeyActionType.NegativeShootX:
                actionLabelText = "Shoot Left";            
                break;
            case InputScheme.KeyActionType.PositiveShootY:
                actionLabelText = "Shoot Up";
                break;
            case InputScheme.KeyActionType.NegativeShootY:
                actionLabelText = "Shoot Down";                
                break;
            case InputScheme.KeyActionType.ReverseTime:
                actionLabelText = "Reverse Time";
                break;
            case InputScheme.KeyActionType.Pause:
                actionLabelText = "Pause";
                break;
        }
        actionLabel.text = actionLabelText;
    }

    public void OnCancelAction()
    {
        button.interactable = true;
    }

    

}
