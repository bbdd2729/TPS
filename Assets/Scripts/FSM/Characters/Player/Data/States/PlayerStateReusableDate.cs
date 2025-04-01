using UnityEngine;

public class PlayerStateReusableDate
{
   public float inputMult { get; set; }

   public bool shouldWalk { get; set; }

   public bool canDash { get; set; } = true;

   public float poseThreshold { get; set; }

   public Vector2 inputDirection { get; set; }

   public float rotationTime { get; set; }

   public float targetAngle { get; set; }

   //�������ֻ��newһ�Σ���ô���һ�ȡ�����Ա��ʱ����ʵ�����ǻ�ȡ���������Ա�����ã�����ϣ�����new����࣬�������޸�ԭ���ĳ�Ա��������ref+���Է�װ�ֶΣ��Ӷ�����������͵�����
}