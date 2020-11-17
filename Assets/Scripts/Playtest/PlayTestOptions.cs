using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTestOptions : ScriptableObject
{
    public bool useTimeRecovery = true;
    public bool useTimePickups = true;
    public bool alwaysTimeVoyage = false;
    public bool useCustomPickupColor = false;
    public bool rewindConsumesAll = false;

    public bool shouldRevertHealthOnTimeVoyage = true;
    public bool spawnHealthOpposite = true;

    public bool difficultySpawn = true;

    public float filledTimeBars = 0.15f;
    
    [SerializeField]
    private BlendColorApplier[] pickupsToChangeColor;
    [SerializeField] ColorType[] customPickupColors;

    [SerializeField]
    private DropsDefinitionVar definition;

    [SerializeField]
    private DropsDefinition normalDefinition;

    [SerializeField]
    private DropsDefinition timeDefinition;
    

    public void ResetOptions() 
    {
        if(!Application.isPlaying)
        {
            return;
        }
        definition.Value = useTimePickups? timeDefinition : normalDefinition;

        for(int i = 0; i < customPickupColors.Length; i++)
        {
            BlendColorApplier blend = pickupsToChangeColor[i];
            blend.type = useCustomPickupColor? customPickupColors[i] : ColorType.PickupColor;
        }
    }

}
