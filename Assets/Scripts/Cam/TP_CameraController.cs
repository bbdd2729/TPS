using UnityEngine;

public class TP_CameraController : MonoBehaviour
{
   //��ȡ��������
   //Ӧ����ת
   //Ӧ��λ��
   [SerializeField, Header("Ŀ�����")] private Transform camLookTarget;

   [SerializeField, Header("�������ľ���")]
   private float camToTargetDistance;

   [SerializeField, Header("X&Y������")] private Vector2 mouseInputSpeed;

   [Range(0.02f, 0.5f), SerializeField, Header("��תƽ��ʱ��")]
   private float camSmoothTime;

   [SerializeField, Header("��ͷ�ƶ������ٶ�")]
   private float camLerpSpeedOnMouce;

   [SerializeField, Header("��ͷδ�ƶ������ٶ�")]
   private float camLerpSpeedOnNormal;

   [SerializeField] private float currentCamLerpSpeed;
   [SerializeField, Header("��ֱ���ƽ�")] private Vector2 camClampRange;
   private Transform Cam;
   private Vector3 camEulerAngles;
   private Vector3 camRotationPos;
   private float Pitch;
   private Vector3 rotaionCurrentVelocity = Vector3.zero;
   private float Yaw;

   private void Awake()
   {
      Cam = Camera.main.transform;
   }

   private void Start()
   {
      currentCamLerpSpeed = camLerpSpeedOnNormal;
   }

   private void Update()
   {
      GetMouceInput();
      UpdateCursor();
      // SetCamLerpSpeed();
   }

   private void LateUpdate()
   {
      CameraController();
   }

   private void GetMouceInput()
   {
      Yaw += CharacterInputSystem.MainInstance.CameraLook.x * mouseInputSpeed.x;
      Pitch -= CharacterInputSystem.MainInstance.CameraLook.y * mouseInputSpeed.y;
      Pitch = Mathf.Clamp(Pitch, camClampRange.x, camClampRange.y);
   }

   private void CameraController()
   {
      //�����ת��
      camEulerAngles = Vector3.SmoothDamp(camEulerAngles, new Vector3(Pitch, Yaw), ref rotaionCurrentVelocity, camSmoothTime);
      transform.eulerAngles = camEulerAngles;
      //�����λ��
      camRotationPos = camLookTarget.position - transform.forward * camToTargetDistance;
      transform.position = Vector3.Lerp(transform.position, camRotationPos, currentCamLerpSpeed * Time.deltaTime);
   }

   private void SetCamLerpSpeed()
   {
      if (Mathf.Abs(CharacterInputSystem.MainInstance.CameraLook.x) > 0.9f)
      {
         currentCamLerpSpeed = Mathf.Lerp(currentCamLerpSpeed, camLerpSpeedOnMouce, Time.deltaTime * 10);
      }
      else
      {
         currentCamLerpSpeed = Mathf.Lerp(currentCamLerpSpeed, camLerpSpeedOnNormal, Time.deltaTime * 10);
      }
   }

   private void UpdateCursor()
   {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
   }
}