using UnityEngine;

namespace ZZZ
{
   public class PlayerComboReusableData
   {
      public Transform cameraTransform { get; set; }

      public Vector3 detectionDir { get; set; }

      public Vector3 detectionOrigin { get; set; }
      public ComboContainerData currentCombo { get; set; }
      public ComboData currentSkill { get; set; }

      public int ATKIndex { get; set; }
      public int comboIndex { get; set; }
      public BindableProperty<int> currentIndex { get; set; } = new BindableProperty<int>(); //��ֹ��Ϊindex���µ���ATKת��index���ֲ���Ӧ����ֵ

      public bool canInput { get; set; } //�������������ʱ�䣬�൱���Ƿ��� Ԥ����

      public bool canATK { get; set; } //����������Сʱ�䲥�ŵĿ��أ��൱������ ��ȴʱ��

      public bool hasATKCommand { get; set; } //�ڿ��Թ����������°��¹��������� ����ָ��

      public bool canLink { get; set; } //�����ν�����

      public bool canMoveInterrupt { get; set; } //����ͨ���ƶ����
      public int executeIndex { get; set; }

      public bool canQTE { get; set; } //�������˼�����д������
   }
}