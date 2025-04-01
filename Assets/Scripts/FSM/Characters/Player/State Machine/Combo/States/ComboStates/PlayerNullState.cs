namespace ZZZ
{
   public class PlayerNullState : PlayerComboState
   {
      //�������ʵ���˴��εĹ��캯������ô����ҲҪʵ��һ����ֵ���ø�����Ĺ��캯��
      public PlayerNullState(PlayerComboStateMachine comboStateMachine) : base(comboStateMachine)
      {
      }

      public override void Enter()
      {
         base.Enter();
         characterCombo.ReSetComboInfo();
      }

      public override void Update()
      {
         base.Update();
      }

      public override void OnAnimationTranslateEvent(IState state)
      {
         comboStateMachine.ChangeState(state);
      }
   }
}