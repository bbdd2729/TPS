using UnityEngine;

namespace ZZZ
{
   public class PlayerOnSwitchOutState : PlayerMovementState
   {
      public PlayerOnSwitchOutState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
      {
      }

      public override void Enter()
      {
         //��ȡ��ǰ�������͵�����
         Debug.Log(movementStateMachine.player.characterName + "��״̬Ϊ" + GetType().Name);
      }

      public override void Update()
      {
         //��ɫ������ת
      }

      //���״̬��Ψһ��·ʱSwitchIn��������˳�����ͨ��д��Animation�ĺ�������
   }
}