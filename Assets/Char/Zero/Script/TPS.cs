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

   public PlayerPosture playerPosture = PlayerPosture.Stand;
   public LocomotionState locomotionState = LocomotionState.Idle;
   public ArmState armState = ArmState.Normal;
   public CharacterController characterController;

   public float gravity = -9.81f;


   public float jumpVelocity = 5f;

   private readonly float crouchSpeed = 1.5f;

   private readonly float crouchThreshold = 0f;
   private readonly float inAirThreshold = 2.1f;
   private readonly float runSpeed = 5.5f;
   private readonly float standThreshold = 1f;
   private readonly Vector3[] velCache = new Vector3[CACHE_SIZE];
   private readonly float walkSpeed = 2.5f;
   private Animator animator;
   private Vector3 averageVel = Vector3.zero;
   private Transform cameraTransform;
   private bool isAiming;
   private bool isCrouching;
   private bool isGrounded;
   private bool isJumping;
   private bool isRunning;
   private bool isStanding;
   private bool isWalking;

   private Vector3 lastVelocity;

   private Vector2 moveInput;
   private int moveSpeedHash;

   private PlayerInput playerinput;

   private Vector3 playerMovement = Vector3.zero;
   private Transform playerTransform;

   private int postureHash;
   private int turnSpeedHash;
   private int velCacheIndex;
   private float VerticalVelocity;
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

      playerinput = GetComponent<PlayerInput>();
      characterController = GetComponent<CharacterController>();
   }

   // Update is called once per frame
   private void Update()
   {
      CalculateGravity();
      Jump();
      CountInputDirection();
      SwitchPlayerStates();
      SetAnimator();
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


   private Vector3 AverageVel(Vector3 newVel)
   {
      velCache[velCacheIndex] = newVel;
      velCacheIndex++;
      velCacheIndex %= CACHE_SIZE;
      var average = Vector3.zero;
      foreach (var vel in velCache) average += vel;

      return average / CACHE_SIZE;
   }


   public void SwitchPlayerStates()
   {
      // 修改姿势状态判断
      if (!characterController.isGrounded)
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
      if (characterController.isGrounded && isJumping)
         VerticalVelocity = jumpVelocity;
   }


   private void CalculateGravity()
   {
      if (characterController.isGrounded)
         VerticalVelocity = gravity * Time.deltaTime;
      else
         VerticalVelocity += gravity * Time.deltaTime;
   }


   public void CountInputDirection()
   {
      var cameraForwardPj = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
      playerMovement = cameraForwardPj * moveInput.y + cameraTransform.right * moveInput.x;
      playerMovement = playerTransform.InverseTransformDirection(playerMovement);
   }

   public void SetAnimator()
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
         animator.SetFloat(postureHash, inAirThreshold, 0.1f, Time.deltaTime);
         animator.SetFloat(verticalVelocityHash, VerticalVelocity, 0.1f, Time.deltaTime);
      }


      if (armState == ArmState.Normal)
      {
         float rad = Mathf.Atan2(playerMovement.x, playerMovement.z);
         animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
         playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0f);
         //Debug.Log(rad);
      }
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