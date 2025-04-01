using System;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
   public enum SFXStyle
   {
      Null = 0,

      //UI��Ч:1-50
      UISound = 1,
      Click,
      Cancel,

      //��ɫ��Ч��51-100
      CharacterSound = 51,
      jump1,
      jump2,
      Dodge,
      Doge1,
      Doge2,

      //�Ų���Ч��101-120
      FootSound = 101,
      AFoot,
      BFoot,

      //BGM��121-150
      BGM = 121,
      BGM_1,

      //��������151-180
      EnvironmentSound = 151,
      Environment_1,

      //�����������������ֶ����õ���Ч181
      OtherSound = 181,
      CharacterVoice,
      EnemyVoice,
   } //�������õķ����ԣ�������Ч�����С����ܽ�ɫ���������������е��ڴ˷ֿ���
   //Ԥ���صĲ����Ƕ�ȡ���صĽ�ɫ��PlayerSO���������SO������Ԥ����

   [CreateAssetMenu(menuName = "Asset/Audio/SFXData")]
   public class SFXData : ScriptableObject
   {
      public List<SFXDataInfo> SFXDataInfoList = new List<SFXDataInfo>();

      [Serializable]
      public class SFXDataInfo
      {
         public SFXStyle sfxStyle;
         public AudioClip[] audioClips;
         public float spatialBlend; //0��ȫΪ2D��1��ȫΪ3D
         public float audioVolume; //0Ĭ��ΪĬ������
         public float lifeTime; //0Ĭ�ϲ����Զ��ر�
         public float pitch; //0Ĭ��ΪĬ�����ã�����
      } //����ȫ��Ϊ��Ч����AudioMixer���õĶ���Effect����Ч�㣩
   }
}