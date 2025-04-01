using UnityEngine;
using UnityEngine.InputSystem;
using ZZZ;

namespace TPF
{
   public class PlayerIdlingState : PlayerMovementState
   {
      //���ø���Ĺ��캯��
      public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
      {
      }

      GameTimer GameTimer { get; set; }

      public override void Enter()
      {
         base.Enter();
         reusableDate.rotationTime = playerMovementData.idleData.rotationTime;
         animator.SetBool(AnimatorID.HasInputID, false);
         reusableDate.inputMult = playerMovementData.idleData.inputMult;
      }

      public override void Update()
      {
         base.Update();
         ////û��������˳�
         //if (CharacterInputSystem.MainInstance.PlayerMove==Vector2.zero)
         //{
         //    return;
         //}
         //else //������ִ����ת
         //{

         //}
      }

      protected override void AddInputActionCallBacks()
      {
         base.AddInputActionCallBacks();
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started += bufferToRun;
      }

      protected override void RemoveInputActionCallBacks()
      {
         base.RemoveInputActionCallBacks();
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started -= bufferToRun;
         TimerManager.MainInstance.UnregisterTimer(GameTimer);
      }

      private void bufferToRun(InputAction.CallbackContext context)
      {
         GameTimer = TimerManager.MainInstance.GetTimer(0.11f, CheckMoveInput);
      }

      private void CheckMoveInput()
      {
         //��Ϊ�����ɫû��Walk����Run����Run_Start_End
         if (CharacterInputSystem.MainInstance.PlayerMove == Vector2.zero)
         {
            animator.CrossFadeInFixedTime("Run_Start_End", 0.13f);
         }
         else
         {
            Move();
         }
      }

      private void Move()
      {
         if (movementStateMachine.reusableDate.shouldWalk)
         {
            //�л���Walk״̬
            movementStateMachine.ChangeState(movementStateMachine.walkingState);
            return;
         }

         //����ִ��Run�ƶ�
         movementStateMachine.ChangeState(movementStateMachine.runningState);
      }

      public override void HandInput()
      {
         base.HandInput();
      }

      public override void Exit()
      {
         base.Exit();
      }
   }
}