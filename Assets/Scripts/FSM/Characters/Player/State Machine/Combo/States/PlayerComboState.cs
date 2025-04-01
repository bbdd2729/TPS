using GGG.Tool;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ZZZ
{
   public class PlayerComboState : IState
   {
      public PlayerComboState(PlayerComboStateMachine comboStateMachine)
      {
         this.comboStateMachine = comboStateMachine;

         if (player == null)
         {
            player = comboStateMachine.Player;
         }

         if (animator == null)
         {
            animator = comboStateMachine.Player.characterAnimator;
         }

         if (playerComboData == null)
         {
            playerComboData = player.playerSO.ComboData;
         }

         if (reusableData == null)
         {
            reusableData = this.comboStateMachine.ReusableData;
         }

         if (characterCombo == null)
         {
            characterCombo = new CharacterCombo(animator, player.transform, player.camera, reusableData, playerComboData.comboData,
               playerComboData.playerEnemyDetectionData, player);
         }
      }

      protected Player player { get; }
      protected PlayerComboStateMachine comboStateMachine { get; }
      protected CharacterCombo characterCombo { get; }
      protected PlayerComboReusableData reusableData { get; }
      protected PlayerComboData playerComboData { get; }
      protected Animator animator { get; }

      public virtual void Enter()
      {
         AddInputActionEvent();
      }

      public virtual void Exit()
      {
         RemoveInputActionEvent();
      }

      public virtual void HandInput()
      {
      }

      public virtual void OnAnimationExitEvent()
      {
      }

      public virtual void OnAnimationTranslateEvent(IState state)
      {
      }

      public virtual void Update()
      {
         characterCombo.UpdateComboAnimation();
         characterCombo.UpdateEnemy();
         characterCombo.CheckCanLinkCombo();
      }

      protected virtual void AddInputActionEvent()
      {
         CharacterInputSystem.MainInstance.inputActions.Player.L_AtK.started += OnAttackInput;
         CharacterInputSystem.MainInstance.inputActions.Player.FinishSkill.started += OnFinishSkill;
         CharacterInputSystem.MainInstance.inputActions.Player.Execute.started += OnSkill;
         characterCombo.AddEventAction();
      }


      protected virtual void RemoveInputActionEvent()
      {
         CharacterInputSystem.MainInstance.inputActions.Player.L_AtK.started -= OnAttackInput;
         CharacterInputSystem.MainInstance.inputActions.Player.FinishSkill.started -= OnFinishSkill;
         CharacterInputSystem.MainInstance.inputActions.Player.Execute.started -= OnSkill;
         characterCombo.RemoveEventActon();
      }

      private void OnAttackInput(InputAction.CallbackContext context)
      {
         if (player.characterName != SwitchCharacter.MainInstance.newCharacterName.Value)
         {
            return;
         }

         if (characterCombo.CanBaseComboInput())
         {
            if (player.currentMovementState == "PlayerSprintingState" || animator.AnimationAtTag("Dodge"))
            {
               characterCombo.DodgeComboInput();
               Debug.Log("���ܹ���");
            }
            else
            {
               characterCombo.LightComboInput();
            }
         }
      }

      private void OnFinishSkill(InputAction.CallbackContext context)
      {
         if (player.characterName != SwitchCharacter.MainInstance.newCharacterName.Value)
         {
            return;
         }

         if (characterCombo.CanFinishSkillInput())
         {
            characterCombo.FinishSkillInput();
            comboStateMachine.ChangeState(comboStateMachine.SkillState);
         }
      }

      private void OnSkill(InputAction.CallbackContext context)
      {
         if (player.characterName != SwitchCharacter.MainInstance.newCharacterName.Value)
         {
            return;
         }

         if (characterCombo.CanSkillInput())
         {
            characterCombo.SkillInput();
            comboStateMachine.ChangeState(comboStateMachine.SkillState);
         }
      }

      public void SwitchSkill()
      {
         characterCombo.SwitchSkill(player.characterName);
         //�л�������״̬
         comboStateMachine.ChangeState(comboStateMachine.SkillState);
      }
   }
}