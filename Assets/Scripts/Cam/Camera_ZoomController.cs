using Cinemachine;
using UnityEngine;

public class Camera_ZoomController : MonoBehaviour
{
   [Range(1, 8), SerializeField, Header("Ĭ�ϵľ���")]
   private float defaultDistance;

   [Range(0, 8), SerializeField, Header("��С�ľ���")]
   private float lookMinDistance;

   [Range(1, 8), SerializeField, Header("���ľ���")]
   private float lookMaxDistance;

   [SerializeField] private float zoomSensitivity = 1;
   [SerializeField] private float zoomSpeed = 4;
   public float ExternalSpeedVariable = 1;
   [SerializeField] public float currentDistance;


   private CinemachineFramingTransposer CinemachineFramingTransposer;
   private CinemachineInputProvider CinemachineInputProvider;

   private void Awake()
   {
      CinemachineFramingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
      CinemachineInputProvider = GetComponent<CinemachineInputProvider>();
      currentDistance = defaultDistance;
   }

   private void Update()
   {
      UpdateInput();
   }

   private void UpdateInput()
   {
      float inputZoomValue = CinemachineInputProvider.GetAxisValue(2) * zoomSensitivity;
      UpdateZoom(inputZoomValue);
   }

   private void UpdateZoom(float inputZoomValue)
   {
      currentDistance = Mathf.Clamp(currentDistance + inputZoomValue, lookMinDistance, lookMaxDistance);

      float realDistance = CinemachineFramingTransposer.m_CameraDistance;

      realDistance = Mathf.Lerp(realDistance, currentDistance, zoomSpeed * Time.deltaTime);

      CinemachineFramingTransposer.m_CameraDistance = realDistance;

      if (realDistance == currentDistance)
      {
         return;
      }
   }

   public void SetZoom(float distance, float speed)
   {
      currentDistance = distance;
      ExternalSpeedVariable = speed;
   }
}