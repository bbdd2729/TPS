using System;
using Cinemachine;
using UnityEngine;

[Serializable]
public class PlayerCameraUtility
{
   [field: SerializeField] public CinemachineVirtualCamera virtualCamera { get; private set; }
   [field: SerializeField] public float DefaultHorizontalWaitTime { get; private set; } = 0f;

   [field: SerializeField] public float DefaultHorizontalRecenteringTime { get; private set; } = 0.5f;

   private CinemachinePOV cinemachinePOV;

   public void EnableRecentering(float waitTime = -1f, float recenteringTime = -1f)
   {
      cinemachinePOV.m_HorizontalRecentering.m_enabled = true;
      //Debug.Log("ˮƽ�������������"+ cinemachinePOV.m_HorizontalRecentering.m_enabled);

      if (waitTime == -1f)
      {
         cinemachinePOV.m_HorizontalRecentering.m_WaitTime = DefaultHorizontalWaitTime;
      }

      if (recenteringTime == -1f)
      {
         cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = DefaultHorizontalRecenteringTime;
      }

      cinemachinePOV.m_HorizontalRecentering.m_WaitTime = DefaultHorizontalWaitTime;
      cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime = DefaultHorizontalRecenteringTime;
      //Debug.Log("waitTime��"+cinemachinePOV.m_HorizontalRecentering.m_WaitTime);
      //Debug.Log("recenteringTime��" + cinemachinePOV.m_HorizontalRecentering.m_RecenteringTime);
   }

   public void DisableRecentering()
   {
      cinemachinePOV.m_HorizontalRecentering.m_enabled = false;
      //Debug.Log("ˮƽ��������رգ�");
   }

   public void Init()
   {
      cinemachinePOV = virtualCamera.GetCinemachineComponent<CinemachinePOV>();
   }
}