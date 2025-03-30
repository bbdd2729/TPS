using HSR.NPRShader;
using UnityEngine;
using UnityEngine.InputSystem;

public class TPS : MonoBehaviour
{
   public enum ArmState
   {
      Normal,
      Aim
   }

   public enum LocomotionState
   {
      Idle,
      Walk,
      Run
   }

   public enum PlayerPosture
   {
      Crouch,
      Stand,
      InAir,
      Idle
   }

   private static readonly int CACHE_SIZE = 3;

   public StarRailCharacterRenderingController renderingController;


   // 物理参数
   public float gravity = -9.81f; // 重力加速度
   public float maxHeight = 1.2f; // 最大跳跃高度
   public float fallMultiplier = 1.5f; // 下落速度的乘数
   public float groundCheckDistance = 0.4f; // 地面检测距离

   public Vector3 ditherAlphaPoint = Vector3.zero;

   // 移动速度参数
   private readonly float crouchSpeed = 1.5f; // 蹲伏速度


   // 状态阈值
   private readonly float crouchThreshold = 0f; // 蹲伏阈值
   private readonly float inAirThreshold = 2.1f; // 空中阈值
   private readonly float runSpeed = 5.5f; // 奔跑速度
   private readonly float standThreshold = 1f; // 站立阈值
   private readonly Vector3[] velCache = new Vector3[CACHE_SIZE]; // 速度缓存
   private readonly float walkSpeed = 2.5f; // 行走速度
   private Animator animator;
   private ArmState armState = ArmState.Normal;


   // 移动相关
   private Vector3 averageVel = Vector3.zero;
   private Transform cameraTransform;

   // 组件引用
   private CharacterController characterController;


   // 状态标志
   private bool isAiming;
   private bool isCrouching;
   private bool isGrounded;
   private bool isJumping;
   private bool isRunning;
   private bool isStanding;
   private bool isWalking;

   private Vector3 lastVelocity;
   private LocomotionState locomotionState = LocomotionState.Idle;
   private Vector2 moveInput;
   private int moveSpeedHash;
   private PlayerInput playerinput;
   private Vector3 playerMovement = Vector3.zero;

   // 状态枚举
   private PlayerPosture playerPosture = PlayerPosture.Stand;
   private Transform playerTransform;

   // 动画参数哈希
   private int postureHash;
   private int turnSpeedHash;

   // 缓存索引
   private int velCacheIndex;
   private float VerticalVelocity; // 当前垂直速度
   private int verticalVelocityHash;


   private void Start()
   {
      playerTransform = transform;
      animator = GetComponent<Animator>();

      postureHash = Animator.StringToHash("玩家姿态");
      moveSpeedHash = Animator.StringToHash("移动速度");
      turnSpeedHash = Animator.StringToHash("旋转速度");
      verticalVelocityHash = Animator.StringToHash("垂直速度");
      if (Camera.main != null) cameraTransform = Camera.main.transform;

      //renderingController = GetComponent<StarRailCharacterRenderingController>();  // 获取组件
      //if (renderingController == null)
      {
         //Debug.LogError("组件获取失败");
      }

      playerinput = GetComponent<PlayerInput>();
      characterController = GetComponent<CharacterController>();
   }

   private void Update()
   {
      CheckGround();
      CalculateGravity();
      Jump();
      CountInputDirection();
      SwitchPlayerStates();
      SetAnimator();

      //Debug.Log(GetCameraDistance());
      SetDitherAlpha(GetCameraDistance());
      //OnDrawGizous();
   }

   private void OnAnimatorMove()
   {
      if (playerPosture != PlayerPosture.InAir)
      {
         var playerDeltaMovement = animator.deltaPosition;
         playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
         characterController.Move(playerDeltaMovement);
         averageVel = AverageVel(animator.velocity);
      }
      else
      {
         var playerDeltaMovement = averageVel * Time.deltaTime;
         playerDeltaMovement.y = VerticalVelocity * Time.deltaTime;
         characterController.Move(playerDeltaMovement);
      }
   }

   private void CheckGround()
   {
      if (Physics.SphereCast(playerTransform.position + Vector3.up * groundCheckDistance, characterController.radius, Vector3.down,
             out var hit, groundCheckDistance - characterController.radius + 2 * characterController.skinWidth))
         isGrounded = true;
      else
         isGrounded = false;
   }

   private Vector3 AverageVel(Vector3 newVel)
   {
      velCache[velCacheIndex] = newVel;
      velCacheIndex++;
      velCacheIndex %= CACHE_SIZE;
      var average = Vector3.zero;
      foreach (var vel in velCache) average += vel;

      return average / CACHE_SIZE;
   }

   private void SwitchPlayerStates()
   {
      // 修改姿势状态判断
      if (!isGrounded)
         playerPosture = PlayerPosture.InAir;
      else if (isCrouching)
         playerPosture = PlayerPosture.Crouch;
      else
         playerPosture = PlayerPosture.Stand;

      if (moveInput.sqrMagnitude == 0)
         locomotionState = LocomotionState.Idle;
      else if (!isRunning)
         locomotionState = LocomotionState.Walk;
      else
         locomotionState = LocomotionState.Run;

      if (isAiming)
         armState = ArmState.Aim;
      else
         armState = ArmState.Normal;
   }

   private void Jump()
   {
      if (isGrounded && isJumping)
         VerticalVelocity = Mathf.Sqrt(-2 * maxHeight * gravity);
   }

   private void CalculateGravity()
   {
      if (isGrounded)
         VerticalVelocity = gravity * Time.deltaTime;
      else
      {
         if (VerticalVelocity <= 0)
            VerticalVelocity += gravity * fallMultiplier * Time.deltaTime;
         else
            VerticalVelocity += gravity * Time.deltaTime;
      }
   }

   private void CountInputDirection()
   {
      var cameraForwardPj = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
      playerMovement = cameraForwardPj * moveInput.y + cameraTransform.right * moveInput.x;
      playerMovement = playerTransform.InverseTransformDirection(playerMovement);
   }

   private void SetAnimator()
   {
      if (playerPosture == PlayerPosture.Stand)
      {
         animator.SetFloat(postureHash, standThreshold, 0.1f, Time.deltaTime);
         switch (locomotionState)
         {
            case LocomotionState.Idle:
               animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
               break;
            case LocomotionState.Walk:
               animator.SetFloat(moveSpeedHash, playerMovement.magnitude * walkSpeed, 0.1f, Time.deltaTime);
               break;
            case LocomotionState.Run:
               animator.SetFloat(moveSpeedHash, playerMovement.magnitude * runSpeed, 0.1f, Time.deltaTime);
               break;
         }
      }
      else if (playerPosture == PlayerPosture.Crouch)
      {
         animator.SetFloat(postureHash, crouchThreshold, 0.1f, Time.deltaTime);
         switch (locomotionState)
         {
            case LocomotionState.Idle:
               animator.SetFloat(moveSpeedHash, 0, 0.1f, Time.deltaTime);
               break;
            default:
               animator.SetFloat(moveSpeedHash, playerMovement.magnitude * crouchSpeed, 0.1f, Time.deltaTime);
               break;
         }
      }
      else if (playerPosture == PlayerPosture.InAir)
      {
         animator.SetFloat(postureHash, inAirThreshold);
         animator.SetFloat(verticalVelocityHash, VerticalVelocity);
      }


      if (armState == ArmState.Normal)
      {
         float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
         animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
         playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0f);
         //Debug.Log(rad);
      }
   }

   private float GetCameraDistance()
   {
      var checkPoint = new Vector3(playerTransform.position.x, playerTransform.position.y + 1f, playerTransform.position.z);
      var cameraDistance = Vector3.Distance(checkPoint, cameraTransform.position);
      return cameraDistance;
   }

   private void SetDitherAlpha(float cameraDistance)
   {
      renderingController.DitherAlpha = cameraDistance;
   }

   private void OnDrawGizous()
   {
      Gizmos.color = Color.red;
      Gizmos.DrawSphere(ditherAlphaPoint, 0.1f);
   }


   #region 输入

   public void GetMoveInput(InputAction.CallbackContext ctx)
   {
      moveInput = ctx.ReadValue<Vector2>();
   }

   public void GetJumpInput(InputAction.CallbackContext ctx)
   {
      isJumping = ctx.ReadValueAsButton();
   }

   public void GetCrouchInput(InputAction.CallbackContext ctx)
   {
      isCrouching = ctx.ReadValueAsButton();
   }

   public void GetRunInput(InputAction.CallbackContext ctx)
   {
      isRunning = ctx.ReadValueAsButton();
   }

   public void GetAimInput(InputAction.CallbackContext ctx)
   {
      isAiming = ctx.ReadValueAsButton();
   }

   #endregion
}