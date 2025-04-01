using UnityEngine;

namespace ZZZ
{
   [CreateAssetMenu(fileName = "Player", menuName = "Create/Character/Player")]
   public class PlayerSO : ScriptableObject
   {
      //field���ֶ���ʾ�ڱ༭����ϣ�������һ��˽���ֶ�
      [field: SerializeField] public PlayerMovementData movementData { get; private set; }

      [field: SerializeField] public PlayerComboData ComboData { get; private set; }
   }
}