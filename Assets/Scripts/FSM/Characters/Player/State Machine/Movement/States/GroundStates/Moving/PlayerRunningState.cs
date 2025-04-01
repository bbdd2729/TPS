using UnityEngine.InputSystem;
using ZZZ;

namespace TPF
{
   public class PlayerRunningState : PlayerMovementState
   {
      GameTimer gameTimer = null;

      public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
      {
      }

      public override void Enter()
      {
         base.Enter();
         animator.CrossFadeInFixedTime("WalkStart", 0.14f);

         reusableDate.rotationTime = playerMovementData.runData.rotationTime;

         animator.SetBool(AnimatorID.HasInputID, true);

         reusableDate.inputMult = playerMovementData.runData.inputMult;
      }

      public override void Update()
      {
         base.Update();
      }

      #region ת��Walking

      protected override void OnWalkStart(InputAction.CallbackContext context)
      {
         base.OnWalkStart(context);

         movementStateMachine.ChangeState(movementStateMachine.walkingState);
      }

      #endregion

      #region ת��Idling

      protected override void AddInputActionCallBacks()
      {
         base.AddInputActionCallBacks();
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.canceled += OnEnterIdle;
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started += OnKeepRunning;
      }

      protected override void RemoveInputActionCallBacks()
      {
         base.RemoveInputActionCallBacks();
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.canceled -= OnEnterIdle;
         CharacterInputSystem.MainInstance.inputActions.Player.Movement.started -= OnKeepRunning;
      }

      private void OnEnterIdle(InputAction.CallbackContext context)
      {
         gameTimer = TimerManager.MainInstance.GetTimer(playerMovementData.bufferToIdleTime, StartToIdle);
      }

      private void StartToIdle()
      {
         movementStateMachine.ChangeState(movementStateMachine.idlingState);
      }

      private void OnKeepRunning(InputAction.CallbackContext context)
      {
         //ע��ԭ���ļ�ʱ��
         TimerManager.MainInstance.UnregisterTimer(gameTimer);
         //��������
         animator.CrossFadeInFixedTime("WalkStart", 0.14f);
      }

      #endregion
   }
}