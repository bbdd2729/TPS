using UnityEngine;

namespace ZZZ
{
   public class PlayerOnSwitchState : PlayerMovementState
   {
      //�л�����
      public PlayerOnSwitchState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
      {
      }

      public override void Enter()
      {
         base.Enter();
         //������Ч
         movementStateMachine.player.PlaySwitchWindSound();
         movementStateMachine.player.PlaySwitchInVoice();
      }

      public override void OnAnimationExitEvent()
      {
         if (CharacterInputSystem.MainInstance.PlayerMove == Vector2.zero)
         {
            movementStateMachine.ChangeState(movementStateMachine.idlingState);
            return;
         }
         else
         {
            if (GameBlackboard.MainInstance.GetGameData<Player>(SwitchCharacter.MainInstance.currentCharacterName.ToString())
                .CanSprintOnSwitch)
            {
               movementStateMachine.ChangeState(movementStateMachine.sprintingState);
               return;
            }

            movementStateMachine.ChangeState(movementStateMachine.runningState);
         }
      }
   }
}