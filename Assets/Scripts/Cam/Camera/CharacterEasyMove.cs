using UnityEngine;

public class CharacterEasyMove : MonoBehaviour
{
   [SerializeField] private float speed;
   private Transform Cam;
   CharacterController controller;
   Vector3 targetDirection;

   private void Awake()
   {
      Cam = Camera.main.transform;
      controller = GetComponent<CharacterController>();
   }

   private void Update()
   {
      CharacterMove();
   }

   private void LateUpdate()
   {
      if (CharacterInputSystem.MainInstance.PlayerMove != Vector2.zero)
      {
         //  CharacterRotation();
         CharacterRotation();
      }
   }

   private void OnDrawGizmos()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawLine(transform.position, (transform.position + targetDirection * 5));
   }

   //private Vector3 InputDir()
   //{
   //    Vector3 camForwardDir =new Vector3(Cam.forward.x, 0, Cam.forward.z).normalized;
   //    return  (camForwardDir * CharacterInputSystem.MainInstance.PlayerMove.y + Cam.right * CharacterInputSystem.MainInstance.PlayerMove.x).normalized;
   //}
   //private void CharacterRotation()
   //{
   //     float targetAngle = Mathf.Atan2(InputDir().x, InputDir().z) * Mathf.Rad2Deg;
   //     transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, targetAngle, 0),Time.deltaTime*speed);
   //}
   private void CharacterMove()
   {
      controller.Move(transform.forward * (CharacterInputSystem.MainInstance.PlayerMove != Vector2.zero ? 0.08f : 0));
   }

   private void CharacterRotation()
   {
      if (CharacterInputSystem.MainInstance.PlayerMove == Vector2.zero)
      {
         return;
      }

      // ��ȡ�������ǰ��������ˮƽƽ���ϵ�ͶӰ
      Vector3 camForward = new Vector3(Cam.forward.x, 0, Cam.forward.z).normalized;

      // ����������뷽��
      float targetAngle = Mathf.Atan2(CharacterInputSystem.MainInstance.PlayerMove.x, CharacterInputSystem.MainInstance.PlayerMove.y) *
                          Mathf.Rad2Deg;

      // ����Ŀ�귽��
      targetDirection = Quaternion.Euler(0, targetAngle, 0) * camForward;

      // ƽ������ת��ɫ��Ŀ�귽��
      Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
      transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
   }
}