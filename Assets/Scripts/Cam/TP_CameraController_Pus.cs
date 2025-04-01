using UnityEngine;

public class TP_CameraController_Pus : MonoBehaviour
{
   ///   <summary>
   ///  ����������߼�⣬�����ⲻ�����ǣ�
   ///  ������ʼ���������֮��Ѱ�Ҽ����㣬
   ///  ֱ���ҵ����Կ������ǵĵ�
   /// ����Ϸ����ҿ������������������ת
   ///   </summary>
   public Transform player; //��ɫλ����Ϣ

   public int num; //�����ʱ��ĸ���
   public Vector3 start; //�����ʼʱ��λ��
   public Vector3 end; //���û���ҵ�����ʱ��λ��
   public float speed; //����ƶ��ٶ�
   Quaternion angel; //�������Ŀ�����תֵ
   Vector3 tagetPostion; //��������Ŀ���
   Vector3[] v3; //����Զ���Ѱ��λ�õ�
   Vector3 ve3; //ƽ�������ref����

   void Start()
   {
      //��縳ֵ���鳤��
      v3 = new Vector3[num];
   }

   void LateUpdate()
   {
      //��¼�����ʼλ��
      start = player.position + player.up * 2.0f - player.forward * 3.0f;
      //��¼�������λ��
      end = player.position + player.up * 5.0f;
      //�������������ת
      if (Input.GetMouseButton(1))
      {
         //��¼����ĳ�ʼλ�ú���ת�Ƕ�
         Vector3 pos = transform.position;
         Vector3 rot = transform.eulerAngles;
         //���������ָ��������ת
         transform.RotateAround(transform.position, Vector3.up, Input.GetAxis("Mouse X") * 10);
         transform.RotateAround(transform.position, Vector3.left, -Input.GetAxis("Mouse Y") * 10);
         //�����������X��ת�ĽǶ�
         if (transform.eulerAngles.x < -60 || transform.eulerAngles.x > 60)
         {
            transform.position = pos;
            transform.eulerAngles = rot;
         }

         return;
      }

      //���Ŀ��λ�ã���ʼ���ڳ�ʼλ��
      tagetPostion = start;
      v3[0] = start;
      v3[num - 1] = end;
      //��̬��ȡ����ļ�����
      for (int i = 1; i < num; i++)
      {
         v3[i] = Vector3.Lerp(start, end, i / num);
      }

      //�ж�������Ǹ�����Կ�������
      for (int i = 0; i < num; i++)
      {
         if (Function(v3[i]))
         {
            tagetPostion = v3[i];
            break;
         }

         if (i == num - 1)
         {
            tagetPostion = end;
         }
      }

      //���ǵ��ƶ��Ϳ���
      transform.position = Vector3.SmoothDamp(transform.position, tagetPostion, ref ve3, 0);
      angel = Quaternion.LookRotation(player.position - tagetPostion);
      transform.rotation = Quaternion.Slerp(transform.rotation, angel, speed);
   }

   ///   <summary>
   ///  ���߼�⣬����Ƿ����յ�����
   ///   </summary>
   ///   <param name=" v3 "> �������߷���ķ��� </param>
   ///   <returns> �Ƿ��⵽ </returns>
   bool Function(Vector3 v3)
   {
      RaycastHit hit;
      if (Physics.Raycast(v3, player.position - v3, out hit))
      {
         if (hit.collider.tag == "Player")
         {
            return true;
         }
      }

      return false;
   }
}