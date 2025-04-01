using GGG.Tool;
using UnityEngine;

namespace ZZZ
{
   public class CharacterComboBase
   {
      public CharacterComboBase(Animator animator, Transform playerTransform, Transform cameraTransform,
         PlayerComboReusableData reusableData, PlayerComboSOData playerComboSOData, PlayerEnemyDetectionData playerEnemyDetectionData,
         Player player)
      {
         //get�����������ڹ��캯�����渳ֵ���������������ط���ֵ
         this.animator = animator;
         this.playerTransform = playerTransform;
         this.reusableData = reusableData;
         comboData = playerComboSOData;
         enemyDetectionData = playerEnemyDetectionData;
         reusableData.cameraTransform = cameraTransform;
         this.player = player;

         if (comboData.heavyCombo != null)
         {
            comboData.heavyCombo.Init();
         }

         if (comboData.lightCombo != null)
         {
            //��ʼ����ʵ���ǻ�������
            comboData.lightCombo.Init();
         }
      }

      protected Animator animator { get; }
      protected Transform playerTransform { get; }
      protected PlayerComboReusableData reusableData { get; }
      protected PlayerComboSOData comboData { get; }
      protected PlayerEnemyDetectionData enemyDetectionData { get; }

      public Player player { get; }

      public void AddEventAction()
      {
         reusableData.currentIndex.OnValueChanged += ReSetATKIndex;
         GameEventsManager.MainInstance.AddEventListening<Transform>("�ﵽQTE����", CanSwitchSkill);
      }


      public void RemoveEventActon()
      {
         reusableData.currentIndex.OnValueChanged -= ReSetATKIndex;
         GameEventsManager.MainInstance.ReMoveEvent<Transform>("�ﵽQTE����", CanSwitchSkill);
      }


      public virtual bool CanBaseComboInput()
      {
         if (!reusableData.canInput)
         {
            return false;
         }

         if (animator.AnimationAtTag("Hit")) return false;
         if (animator.AnimationAtTag("Parry")) return false;
         if (animator.AnimationAtTag("Execute")) return false;
         if (animator.AnimationAtTag("Skill"))
         {
            return false;
         }

         return true;
      }

      protected virtual void UpdateComboInfo()
      {
         reusableData.comboIndex++;
         if (reusableData.comboIndex > reusableData.currentCombo.GetComboMaxCount() - 1)
         {
            reusableData.comboIndex = 0;
         }
      }


      //ע��ת���˺��Ķ����¼�
      public void ATK()
      {
         AttackTrigger();
      }

      protected int UpdateExecuteIndex(ComboContainerData containerData)
      {
         return Random.Range(0, containerData.GetComboMaxCount());
      }

      #region �����˺�

      private void AttackTrigger()
      {
         if (animator.AnimationAtTag("ATK")) //����ͨ���������˺��Ϳ��ܶ����������ܻ�����
         {
            UpdateATKIndex();
            // Debug.Log("��ʱATKֵΪ��"+ reusableData.ATKIndex) ;
            CameraHitFeel.MainInstance.CameraShake(
               reusableData.currentCombo.GetComboShakeForce(reusableData.currentIndex.Value, reusableData.ATKIndex));
            Debug.Log(reusableData.currentCombo);
            if (!AttackDetection(reusableData.currentCombo))
            {
               return;
            }

            GameEventsManager.MainInstance.CallEvent("�����˺�",
               reusableData.currentCombo.GetComboDamage(reusableData.currentIndex.Value),
               reusableData.currentCombo.GetComboHitName(reusableData.currentIndex.Value),
               reusableData.currentCombo.GetComboParryName(reusableData.currentIndex.Value),
               playerTransform, GameBlackboard.MainInstance.GetEnemy(),
               this);

            CameraHitFeel.MainInstance.PF(
               reusableData.currentCombo.GetPauseFrameTime(reusableData.currentIndex.Value, reusableData.ATKIndex));
            //Debug.Log("QTE��������" + reusableData.canQTE);
            //Debug.Log("�����ATK����ֵ��"+reusableData.ATKIndex);
            //Debug.Log("���õ����ATK����Ϊ��" + reusableData.currentCombo.GetComboATKCount(reusableData.currentIndex.Value));

            #region ����QTE

            if (reusableData.canQTE && reusableData.ATKIndex >= reusableData.currentCombo.GetComboATKCount(reusableData.currentIndex.Value))
            {
               TriggerSwitchSkill();
            }

            #endregion
         }
         else if (animator.AnimationAtTag("Skill"))
         {
            UpdateATKIndex();

            if (!SkillDetection(reusableData.currentSkill))
            {
               return;
            }

            GameEventsManager.MainInstance.CallEvent("�����˺�", reusableData.currentSkill.comboDamage, reusableData.currentSkill.hitName,
               reusableData.currentSkill.parryName, playerTransform, GameBlackboard.MainInstance.GetEnemy(), this);

            #region ��֡

            if (reusableData.currentSkill.pauseFrameTimeList != null && reusableData.currentSkill.pauseFrameTimeList.Length > 0 &&
                reusableData.ATKIndex <= reusableData.currentSkill.pauseFrameTimeList.Length)
            {
               CameraHitFeel.MainInstance.PF(reusableData.currentSkill.pauseFrameTimeList[reusableData.ATKIndex - 1]);
            }
            else
            {
               CameraHitFeel.MainInstance.PF(reusableData.currentSkill.pauseFrameTime);
            }

            #endregion

            #region ����QTE

            if (reusableData.canQTE && reusableData.ATKIndex >= reusableData.currentSkill.ATKCount)
            {
               TriggerSwitchSkill();
            }

            #endregion

            #region ����

            if (reusableData.currentSkill.shakeForce == null ||
                reusableData.ATKIndex >
                reusableData.currentSkill.shakeForce.Length) //����û����������ATK��force�����¸�����ATKindex��������force����ֵ
            {
               return;
            }

            CameraHitFeel.MainInstance.CameraShake(reusableData.currentSkill.shakeForce[reusableData.ATKIndex - 1]);

            #endregion
         }
         else //����ֻ��һ���ܻ����������ǿ����ж���˺�
         {
            if (!AttackDetection(comboData.executeCombo))
            {
               return;
            }

            GameEventsManager.MainInstance.CallEvent("�����˺�", comboData.executeCombo.GetComboDamage(reusableData.executeIndex));
         }
      }

      #endregion

      public void UpdateAttackLookAtEnemy()
      {
         if (GameBlackboard.MainInstance.GetEnemy() == null)
         {
            return;
         }

         if ((animator.AnimationAtTag("ATK") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.3f) ||
             animator.AnimationAtTag("Skill"))
         {
            if (DevelopmentToos.DistanceForTarget(playerTransform, GameBlackboard.MainInstance.GetEnemy()) > 6.5f) return;
            if (DevelopmentToos.DistanceForTarget(playerTransform, GameBlackboard.MainInstance.GetEnemy()) < 0.09f) return;
            playerTransform.Look(GameBlackboard.MainInstance.GetEnemy().position, 60);
         }
      }

      public void CheckMoveInterrupt()
      {
         if (reusableData.canMoveInterrupt == false)
         {
            return;
         }

         if (CharacterInputSystem.MainInstance.PlayerMove.sqrMagnitude != 0)
         {
            animator.CrossFadeInFixedTime("Locomotion", 0.155f, 0);
            reusableData.canMoveInterrupt = false;
         }
      }

      public void CheckCanLinkCombo()
      {
         if (!reusableData.canLink || CharacterInputSystem.MainInstance.Run)
         {
            ReSetComboInfo();
         }
      }

      #region һ�㹥��

      public virtual void LightComboInput()
      {
         if (comboData.lightCombo == null)
         {
            return;
         }

         if (reusableData.currentCombo != comboData.lightCombo || reusableData.currentCombo == null)
         {
            reusableData.currentCombo = comboData.lightCombo;
            ReSetComboInfo();
         }

         //ȷ���ṥ���ǵ�һ���ṥ��
         reusableData.currentCombo.ResetComboDates();

         ExecuteBaseCombo();
      }

      public virtual void HeavyComboInput()
      {
         if (comboData.heavyCombo == null)
         {
            return;
         }

         if (reusableData.currentCombo != comboData.heavyCombo || reusableData.currentCombo == null)
         {
            reusableData.currentCombo = comboData.heavyCombo;

            ReSetComboInfo();
         }

         ExecuteBaseCombo();
      }

      public virtual void NormalDodgeCombo()
      {
         if (comboData.lightCombo == null)
         {
            return;
         }

         if (reusableData.currentCombo != comboData.lightCombo || reusableData.currentCombo == null)
         {
            reusableData.currentCombo = comboData.lightCombo;
         }

         reusableData.currentCombo.SwitchDodgeATK();
         ReSetComboInfo();
         ReSetATKIndex(0);
         ExecuteBaseCombo();
      }

      protected virtual void ExecuteBaseCombo()
      {
         if (reusableData.currentCombo == null)
         {
            return;
         }

         reusableData.hasATKCommand = true;
         reusableData.canInput = false;
      }

      public virtual void UpdateComboAnimation()
      {
         if (!reusableData.canATK)
         {
            return;
         }

         if (!reusableData.hasATKCommand)
         {
            return;
         }

         reusableData.currentIndex.Value = reusableData.comboIndex;
         string comboName = reusableData.currentCombo.GetComboName(reusableData.currentIndex.Value);
         animator.CrossFade(comboName, 0.111f, 0);
         //��������
         PlayCharacterVoice(reusableData.currentCombo.comboDates[reusableData.currentIndex.Value]);
         StartPlayWeapon();

         UpdateComboInfo();


         reusableData.hasATKCommand = false;
         reusableData.canATK = false;
      }


      public virtual void ReSetComboInfo()
      {
         reusableData.comboIndex = 0;
         reusableData.canInput = true;
         reusableData.canLink = true;
         reusableData.canMoveInterrupt = false;
         reusableData.canATK = true;
      }

      #endregion

      #region ��������

      protected virtual void CanSwitchSkill(Transform transform)
      {
      }

      protected virtual void TriggerSwitchSkill()
      {
      }

      #endregion


      #region �����¼�

      public void DisConnectCombo() //�¼�����
      {
         reusableData.canLink = false;
      }

      public void CanMoveInterrupt()
      {
         reusableData.canMoveInterrupt = true;
      }

      public void CanInput()
      {
         reusableData.canInput = true;
      }

      public void CanATK()
      {
         reusableData.canATK = true;
      }

      public void PlayComboFX()
      {
         SFX_PoolManager.MainInstance.TryGetSoundPool(reusableData.currentCombo.GetComboSoundStyle(reusableData.currentIndex.Value),
            playerTransform.position, Quaternion.identity);
      }

      #endregion

      #region �˺����

      protected bool AttackDetection(ComboContainerData comboContainerData)
      {
         //����
         //����
         //�Ƕ�
         if (GameBlackboard.MainInstance.GetEnemy() == null)
         {
            return false;
         }

         // Debug.Log("������������");
         if (DevelopmentToos.DistanceForTarget(GameBlackboard.MainInstance.GetEnemy(), playerTransform) >
             comboContainerData.GetComboDistance(reusableData.currentIndex.Value))
         {
            return false;
         }

         // Debug.Log("��������");
         if (DevelopmentToos.GetAngleForTargetDirection(GameBlackboard.MainInstance.GetEnemy(), playerTransform) < 80)
         {
            return false;
         }

         // Debug.Log("�Ƕ���������");
         return true;
      }

      protected bool SkillDetection(ComboData comboData)
      {
         if (GameBlackboard.MainInstance.GetEnemy() == null)
         {
            return false;
         }

         if (DevelopmentToos.DistanceForTarget(GameBlackboard.MainInstance.GetEnemy(), playerTransform) > comboData.attackDistance)
         {
            return false;
         }

         if (DevelopmentToos.GetAngleForTargetDirection(GameBlackboard.MainInstance.GetEnemy(), playerTransform) < 135)
         {
            return false;
         }

         return true;
      }

      #endregion

      #region �����˺���

      /// <summary>
      /// �����˺���ļ���
      /// </summary>
      /// <param name="�������û����"></param>
      public void ReSetATKIndex(int index) //����ÿһ����Ҫִ���ֶ����㣬���չ��ǹ�������ֵ�����仯�����㣬������AҲҪ�ֶ����㣬��Ϊ��A����������ֵʼ��Ϊ0
      {
         reusableData.ATKIndex = 0;
      }

      public void UpdateATKIndex()
      {
         reusableData.ATKIndex++;
      }

      #endregion

      #region ������Ч

      private void StartPlayWeapon()
      {
         PlayWeaponSound(reusableData.currentCombo.comboDates[reusableData.currentIndex.Value]);
      }

      protected void PlayCharacterVoice(ComboData comboData)
      {
         SFX_PoolManager.MainInstance.TryGetSoundPool(SoundStyle.ComboVoice, comboData.comboName, playerTransform.position);
      }

      protected void PlayWeaponSound(ComboData comboData)
      {
         SFX_PoolManager.MainInstance.TryGetSoundPool(SoundStyle.WeaponSound, comboData.comboName, playerTransform.position);
      }

      #endregion
   }
}