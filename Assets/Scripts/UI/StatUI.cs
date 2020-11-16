using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class StatUI : MonoBehaviour
{
    [SerializeField]
    private Image statImage;

    [SerializeField]
    private Image[] ticks;

    [SerializeField]
    private Color enabledColor;

    [SerializeField]
    private Color disabledColor;

    [SerializeField]
    private StatTierList currentTiers;

    [SerializeField]
    private PickupRepresentationArr pickups;

    [SerializeField]
    private PlayerStatType type;

    private void Start()
    {
        UpdateTicks();
        currentTiers.OnStatsChangeEvent += UpdateTicks;
    }

    private void OnDestroy() 
    {
        currentTiers.OnStatsChangeEvent -= UpdateTicks;
    }

#if UNITY_EDITOR

    private void OnValidate() 
    {
        UpdateTicks();
    }
#endif

    private void UpdateTicks()
    {
        if(currentTiers == null)
        {
            return;
        }

        PickupRepresentation[] representations = pickups.Pickups;
        foreach(PickupRepresentation rep in representations)
        {
            if(type == rep.type)
            {
                statImage.sprite = rep.image;
                break;
            }
        }

        int tier = currentTiers.GetTier(type);
        for(int i = 0; i < ticks.Length; i++)
        {
            bool isActive = (i+1) <= tier;
            ticks[i].color = isActive? enabledColor : disabledColor;
        }
    }
}
