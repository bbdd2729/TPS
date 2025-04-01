namespace ZZZ
{
   public class PlayerComboStateMachine : StateMachine
   {
      public PlayerComboStateMachine(Player player)
      {
         Player = player;

         ReusableData = new PlayerComboReusableData();

         ATKIngState = new PlayerATKIngState(this);

         NullState = new PlayerNullState(this);

         SkillState = new PlayerSkillState(this);
      }

      public Player Player { get; } //ֻ���ڹ��캯�����޸�{get;private set}ֻ���ڱ������޸�
      public PlayerATKIngState ATKIngState { get; }
      public PlayerNullState NullState { get; }
      public PlayerComboReusableData ReusableData { get; }

      public PlayerSkillState SkillState { get; }
   }
}