using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTestOptions : ScriptableObject
{
    public bool useTimeRecovery = true;
    public bool alwaysTimeVoyage = false;

    [SerializeField]
    private DropsDefinitionVar definition;

    [SerializeField]
    private DropsDefinition normalDefinition;

    [SerializeField]
    private DropsDefinition timeDefinition;

    [SerializeField]
    public float filledTimeBars = 1.0f;

    [SerializeField]
    private RollbackTimer rollbackTimer;

    public void ResetOptions() 
    {
        if(!Application.isPlaying)
        {
            return;
        }
        definition.Value = useTimeRecovery? timeDefinition : normalDefinition;
        rollbackTimer.SetPercentage(filledTimeBars);
    }

}
