using ZZZ;

public class PlayerSkillState : PlayerComboState
{
   public PlayerSkillState(PlayerComboStateMachine comboStateMachine) : base(comboStateMachine)
   {
   }

   /// <summary>
   /// ͨ�������¼���ִ����������
   /// </summary>
   public override void Enter()
   {
      base.Enter();
      comboStateMachine.Player.movementStateMachine.ChangeState(comboStateMachine.Player.movementStateMachine.playerMovementNullState);
      //����״̬���-�����޸�һ�´���StateDriveCameras�������������ȡ
      CameraSwitcher.MainInstance.ActiveStateCamera(player.characterName, reusableData.currentSkill.attackStyle);
   }

   public override void Update()
   {
      characterCombo.UpdateAttackLookAtEnemy();
   }

   public override void Exit()
   {
      CameraSwitcher.MainInstance.UnActiveStateCamera(player.characterName, reusableData.currentSkill.attackStyle);
      base.Exit();
   }

   /// <summary>
   /// ͨ�������ű������������������˳�
   /// </summary>
   public override void OnAnimationExitEvent()
   {
      comboStateMachine.ChangeState(comboStateMachine.NullState);
   }

   /// <summary>
   /// ����switchout�����˳�
   /// </summary>
   /// <param name="state"></param>
   public override void OnAnimationTranslateEvent(IState state)
   {
      comboStateMachine.ChangeState(state);
   }
}