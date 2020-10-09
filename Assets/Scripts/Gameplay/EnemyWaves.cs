using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaves : ScriptableObject
{
    [SerializeField]
    private Transform[] possibleWaves;
    public Transform[] Waves => possibleWaves;
}
