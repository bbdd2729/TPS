using System;
using UnityEngine;

namespace ZZZ
{
   [Serializable]
   public class PlayerEnemyDetectionData
   {
      [field: SerializeField, Header("���˼��")]
      public float detectionRadius { get; private set; }

      [field: SerializeField] public float detectionLength { get; private set; }

      [field: SerializeField] public LayerMask WhatIsEnemy { get; private set; }
   }
}