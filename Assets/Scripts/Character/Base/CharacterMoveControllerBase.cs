using GGG.Tool;
using UnityEngine;

public class CharacterMoveControllerBase : MonoBehaviour
{
   [SerializeField, Header("����")] private float characterGravity = -9;
   [SerializeField] protected float maxVerticalSpeed = 20;
   [SerializeField] protected float minVerticalSpeed = -3;
   [SerializeField] protected float verticalSpeed;
   [SerializeField, Header("������")] private float GroundDetectionRadius;
   [SerializeField] private float GroundDetectionOffset;
   [SerializeField] private LayerMask whatIsGround;
   [SerializeField] protected bool isOnGround;

   [SerializeField, Header("������")] private float SlopDetectionLenth = 1;

   [Range(0.2f, 100), SerializeField, Header("�ƶ�λ�Ʊ���")]
   private float moveMult;

   [Range(0.2f, 60), SerializeField, Header("����λ�Ʊ���")]
   private float dodgeMult;

   protected CharacterController characterController;
   protected float fallOutdeltaTimer;
   protected float fallOutTimer = 0.2f;
   private Vector3 groundDetectionOrigin;
   private ColliderHit groundHit;

   protected Vector3 verticalVelocity;

   //����
   //������
   //������
   //�ƶ��ӿ�
   public Animator characterAnimator { get; private set; }

   protected virtual void Awake()
   {
      characterAnimator = GetComponent<Animator>();
      characterController = GetComponent<CharacterController>();
   }

   protected virtual void Start()
   {
      fallOutdeltaTimer = fallOutTimer;
   }

   protected virtual void Update()
   {
      GroundDetecion();
      UpdateChracterGravity();
      UpDateVerticalVelocity();
      UpDateVerticalVelocity();
   }

   /// <summary>
   /// ���¶�����λ��
   /// </summary>
   protected virtual void OnAnimatorMove()
   {
      //������ɫ��RootMotion
      characterAnimator.ApplyBuiltinRootMotion();

      UpdateCharacterVelocity(characterAnimator.deltaPosition);
      // UpdateCharacterVelocity(characterAnimator.deltaPosition);
   }

   /// <summary>
   /// ���Ƶ�����
   /// </summary>
   private void OnDrawGizmos()
   {
      if (isOnGround)
      {
         Gizmos.color = Color.green;
      }
      else
      {
         Gizmos.color = Color.red;
      }

      Gizmos.DrawWireSphere(new Vector3(transform.position.x, transform.position.y - GroundDetectionOffset, transform.position.z),
         GroundDetectionRadius);
   }

   /// <summary>
   /// ������
   /// </summary>
   protected void GroundDetecion()
   {
      groundDetectionOrigin = new Vector3(transform.position.x, transform.position.y - GroundDetectionOffset, transform.position.z);
      isOnGround = Physics.CheckSphere(groundDetectionOrigin, GroundDetectionRadius, whatIsGround, QueryTriggerInteraction.Ignore);
   }

   /// <summary>
   /// ����
   /// </summary>
   protected void UpdateChracterGravity()
   {
      if (isOnGround)
      {
         fallOutdeltaTimer = fallOutTimer;

         if (verticalSpeed < 0)
         {
            verticalSpeed = -2;
         }
      }
      else
      {
         if (fallOutdeltaTimer >= 0)
         {
            fallOutdeltaTimer -= Time.deltaTime;
         }
         else
         {
            if (verticalSpeed < maxVerticalSpeed && verticalSpeed > minVerticalSpeed)
            {
               verticalSpeed += characterGravity * Time.deltaTime;
            }
         }
      }
   }

   /// <summary>
   /// ���´�ֱ�ٶ�
   /// </summary>
   protected void UpDateVerticalVelocity()
   {
      verticalVelocity.Set(0, verticalSpeed, 0);
      characterController.Move(verticalVelocity * Time.deltaTime);
   }

   /// <summary>
   /// ������
   /// </summary>
   /// <param name="characterVelosity"></param>
   protected Vector3 ResetVelocityOnSlop(Vector3 characterVelosity)
   {
      if (Physics.Raycast(transform.position, Vector3.down, out var groundHit, SlopDetectionLenth, whatIsGround))
      {
         float newAngle = Vector3.Dot(Vector3.up, groundHit.normal);
         //�����컨��ͽ�ɫ��������ʱ
         if (newAngle != -1 && verticalSpeed <= 0)
         {
            return Vector3.ProjectOnPlane(characterVelosity, groundHit.normal);
         }
      }

      return characterVelosity;
   }

   /// <summary>
   ///���½�ɫ�ٶ�
   /// </summary>
   protected virtual void UpdateCharacterVelocity(Vector3 movement)
   {
      Vector3 dir = ResetVelocityOnSlop(movement);
      if (characterAnimator.AnimationAtTag("Movement"))
      {
         characterController.Move(dir * Time.deltaTime * moveMult);
      }
      else if (characterAnimator.AnimationAtTag("Dodge"))
      {
         characterController.Move(dir * Time.deltaTime * dodgeMult);
      }
   }
}