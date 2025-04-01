using System;
using UnityEngine;

namespace ZZZ
{
   [Serializable]
   public class PlayerComboData
   {
      [field: SerializeField, Header("��ʽ����")]
      public PlayerComboSOData comboData { get; private set; }

      [field: SerializeField, Header("���˼��")]
      public PlayerEnemyDetectionData playerEnemyDetectionData { get; private set; }
   }
}