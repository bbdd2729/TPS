using ZZZ;

public class CharacterHeath : CharacterHealthBase
{
   /// <summary>
   /// ���˵��˺�����
   /// </summary>
   /// <param name="damage"></param>
   /// <param name="hitName"></param>
   /// <param name="parryName"></param>
   protected override void CharacterHitAction(float damage, string hitName, string parryName)
   {
      base.CharacterHitAction(damage, hitName, parryName);
      if (healthInfo.hasStrength.Value) //��
      {
         healthInfo.TakeStrength(damage);
         animator.CrossFadeInFixedTime(parryName, 0.1f, 0);
         // SFX_PoolManager.MainInstance.TryGetSoundPool("PARRY",transform.position,Quaternion.identity);
      }
      else //����
      {
         healthInfo.TakeDamage(damage);
         animator.CrossFadeInFixedTime(hitName, 0.1f, 0);
         //SFX_PoolManager.MainInstance.TryGetSoundPool("HIT", transform.position, Quaternion.identity);
      }

      healthInfo.TakeDefenseValue(damage);
   }

   /// <summary>
   /// �������˵��Ʒ��߼�   
   /// </summary>
   /// <param name="value"></param>
   protected override void OnUpdateDefenseValue(float value)
   {
      base.OnUpdateDefenseValue(value);
      if (currentEnemy == null)
      {
         return;
      }

      if (value <= 0)
      {
         GameEventsManager.MainInstance.CallEvent("�ﵽQTE����", currentEnemy);
         healthInfo.ReDefenseValue();
         return;
      }
   }

   //�����ܻ���Ч
   protected override void SetHitSFX(CharacterNameList characterNameList)
   {
      SFX_PoolManager.MainInstance.TryGetSoundPool(SoundStyle.HIT, characterNameList.ToString(), transform.position);
   }
}