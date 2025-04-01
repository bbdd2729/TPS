using System;
using UnityEngine;

namespace ZZZ
{
   [Serializable]
   public class PlayerComboSOData
   {
      [field: SerializeField, Header("�ṥ������")]
      public ComboContainerData lightCombo { get; private set; }

      [field: SerializeField, Header("�ع�������")]
      public ComboContainerData heavyCombo { get; private set; }

      [field: SerializeField, Header("����")]
      public ComboContainerData executeCombo { get; private set; }

      [field: SerializeField, Header("����")]
      public ComboData skillCombo { get; private set; }

      [field: SerializeField, Header("�ռ�����")]
      public ComboData finishSkillCombo { get; private set; }

      [field: SerializeField, Header("���˴���")]
      public ComboData switchSkill { get; private set; }
   }
}