using System;
using System.Collections.Generic;
using Cinemachine;
using HuHu;
using UnityEngine;
using ZZZ;

public class CameraSwitcher : Singleton<CameraSwitcher>
{
   [SerializeField, Header("����ʱ�����")] private CinemachineVirtualCamera switchCharacterSkillCamera;
   [SerializeField, Header("���������")] private List<CharacterStateCameraInfo> stateCameraInfoList = new List<CharacterStateCameraInfo>();
   private CinemachineBrain brain;

   private Dictionary<CharacterNameList, Dictionary<AttackStyle, CinemachineStateDrivenCamera>> stateCameraPool =
      new Dictionary<CharacterNameList, Dictionary<AttackStyle, CinemachineStateDrivenCamera>>();
   //���������ַ�ʽ����ʵ�֣��ֵ�������дһ���ֵ䣻�ֵ�����д�Զ�������ݽṹ���ڶ��ָ����,����û���ֵ�ʡ����


   protected override void Awake()
   {
      base.Awake();
      brain = Camera.main.GetComponent<CinemachineBrain>();
   }

   private void Start()
   {
      InitSwitchCamera();
      InitSkillCamera();
   }

   private void OnEnable()
   {
      brain.m_CameraActivatedEvent.AddListener(OnCameraActivated);
   }

   private void OnDisable()
   {
      brain.m_CameraActivatedEvent.RemoveListener(OnCameraActivated);
   }

   private void InitSwitchCamera()
   {
      if (stateCameraInfoList.Count == 0)
      {
         return;
      }

      for (int i = 0; i < stateCameraInfoList.Count; i++)
      {
         if (stateCameraInfoList[i].stateCameraList.Count == 0)
         {
            continue;
         } //������ǰԪ�� 

         stateCameraPool.Add(stateCameraInfoList[i].characterName, new Dictionary<AttackStyle, CinemachineStateDrivenCamera>());
         for (int j = 0; j < stateCameraInfoList[i].stateCameraList.Count; j++)
         {
            stateCameraInfoList[i].stateCameraList[j].stateCamera.Priority = 0;
            //���뵽�ֵ�����
            stateCameraPool[stateCameraInfoList[i].characterName].Add(stateCameraInfoList[i].stateCameraList[j].AttackStyle,
               stateCameraInfoList[i].stateCameraList[j].stateCamera);
         }
      }
   }

   private void InitSkillCamera()
   {
      switchCharacterSkillCamera.Priority = 0;
   }

   public void ActiveStateCamera(CharacterNameList characterName, AttackStyle attackStyle)
   {
      if (stateCameraPool.TryGetValue(characterName, out var stateCameraList))
      {
         //Ȼ�����б�������ҷ���Ҫ���Ԫ����
         if (stateCameraList.TryGetValue(attackStyle, out var stateDrivenCamera))
         {
            stateDrivenCamera.Priority = 20;
         }
      }
   }

   public void UnActiveStateCamera(CharacterNameList characterName, AttackStyle attackStyle)
   {
      if (stateCameraPool.TryGetValue(characterName, out var stateCameraList))
      {
         //Ȼ�����б�������ҷ���Ҫ���Ԫ����
         if (stateCameraList.TryGetValue(attackStyle, out var stateDrivenCamera))
         {
            stateDrivenCamera.Priority = 0;
         }
      }
   }

   public void ActiveSwitchCamera(bool applySwitchCamera)
   {
      if (applySwitchCamera)
      {
         switchCharacterSkillCamera.Priority = 20;
      }
      else
      {
         switchCharacterSkillCamera.Priority = 0;
      }
   }


   /// <summary>
   /// ����˳�ʱ�Ĵ���
   /// </summary>
   /// <param name="newCamera"></param>
   /// <param name="oldCamera"></param>
   private void OnCameraActivated(ICinemachineCamera newCamera, ICinemachineCamera oldCamera)
   {
   }

   [Serializable]
   public class CharacterStateCameraInfo
   {
      public CharacterNameList characterName;
      public List<StateCameraInfo> stateCameraList = new List<StateCameraInfo>();
   }

   [Serializable]
   public class StateCameraInfo
   {
      public AttackStyle AttackStyle;
      public CinemachineStateDrivenCamera stateCamera;
   }
}