using UnityEngine;
using UnityEngine.InputSystem;

namespace ZZZ
{
   public class PlayerWalkingState : PlayerMovementState
   {
      GameTimer gameTimer;

      public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
      {
      }

      public override void Enter()
      {
         base.Enter();
         reusableDate.rotationTime = playerMovementData.walkData.rotationTime;

         animator.SetBool(AnimatorID.HasInputID, true);

         reusableDate.inputMult = playerMovementData.walkData.inputMult;
      }

      public override void Update()
      {
         base.Update();
      }

      #region ת��Running

      protected override void OnWalkStart(InputAction.CallbackContext context)
      {
         base.OnWalkStart(context);

         movementStateMachine.ChangeState(movementStateMachine.runningState);
      }

      #endregion

      #region ת��Idling

      protected override void AddInputActionCallBacks()
      {
         base.AddInputActionCallBacks();

         CharacterInputSystem.MainInstance.inputActions.Player.Movement.canceled += OnBufferToIdle;
      }

      protected override void RemoveInputActionCallBacks()
      {
         base.RemoveInputActionCallBacks();
         Debug.Log("�Ƴ�run��idle��ί��");
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.canceled -= OnBufferToIdle;
      }

      private void OnBufferToIdle(InputAction.CallbackContext context)
      {
         gameTimer = TimerManager.MainInstance.GetTimer(playerMovementData.bufferToIdleTime, IdleStart);
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started += OnUnregisterBufferTimer;
      }


      private void IdleStart()
      {
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started -= OnUnregisterBufferTimer;
         movementStateMachine.ChangeState(movementStateMachine.idlingState);
         //movementStateMachine.ChangeState(movementStateMachine.idlingState);
      }

      private void OnUnregisterBufferTimer(InputAction.CallbackContext context)
      {
         Debug.Log("ע��Timer");
         TimerManager.MainInstance.UnregisterTimer(gameTimer);
      }

      #endregion
   }
}