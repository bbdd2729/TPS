using System;
using UnityEngine;

namespace ZZZ
{
   [Serializable]
   public class PlayerIdleData
   {
      [field: SerializeField]
      [field: Range(0.1f, 80)]
      public float inputMult { get; private set; } = 0f;

      [field: SerializeField] public float rotationTime { get; private set; } = 0.04f;
   }
}