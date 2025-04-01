using System;
using UnityEngine;

[Serializable]
public class HealthData
{
   [field: SerializeField] public float maxHP { get; private set; }
   [field: SerializeField] public float maxStrength { get; private set; }

   [field: SerializeField] public float maxDefenseValue { get; private set; }
}