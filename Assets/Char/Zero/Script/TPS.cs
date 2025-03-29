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

   public PlayerPosture playerPosture = PlayerPosture.Stand;
   public LocomotionState locomotionState = LocomotionState.Idle;
   public ArmState armState = ArmState.Normal;

   private readonly float crouchSpeed = 1.5f;

   private readonly float crouchThreshold = 0f;
   private readonly float runSpeed = 5.5f;
   private readonly float standThreshold = 1f;
   private readonly float walkSpeed = 2.5f;
   private Animator animator;
   private Transform cameraTransform;
   private float inAirThreshold = 2f;
   private bool isAiming;
   private bool isCrouching;
   private bool isGrounded;
   private bool isJumping;
   private bool isRunning;
   private bool isStanding;
   private bool isWalking;

   private Vector2 moveInput;
   private int moveSpeedHash;

   private PlayerInput playerinput;

   private Vector3 playerMovement = Vector3.zero;
   private Transform playerTransform;

   private int postureHash;
   private int turnSpeedHash;


   private void Start()
   {
      playerTransform = transform;
      animator = GetComponent<Animator>();

      postureHash = Animator.StringToHash("玩家姿态");
      moveSpeedHash = Animator.StringToHash("移动速度");
      turnSpeedHash = Animator.StringToHash("旋转速度");
      if (Camera.main != null) cameraTransform = Camera.main.transform;

      playerinput = GetComponent<PlayerInput>();
      if (playerinput != null)
      {
         playerinput.onActionTriggered += GetMoveInput;
         playerinput.onActionTriggered += GetRunInput;
         playerinput.onActionTriggered += GetJumpInput;
         playerinput.onActionTriggered += GetCrouchInput;
         playerinput.onActionTriggered += GetAimInput;
      }
   }

   // Update is called once per frame
   private void Update()
   {
      CountInputDirection();
      SwitchPlayerStates();
      SetAnimator();
   }

   public void SwitchPlayerStates()
   {
      // 修改姿势状态判断
      if (!isGrounded)
         playerPosture = PlayerPosture.Crouch;
      else
         playerPosture = PlayerPosture.Stand;

      if (moveInput.sqrMagnitude == 0)
         playerPosture = PlayerPosture.Idle;
      else if (!isRunning)
         locomotionState = LocomotionState.Walk;
      else
         locomotionState = LocomotionState.Run;

      if (isAiming)
         armState = ArmState.Aim;
      else
         armState = ArmState.Normal;
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

      if (armState == ArmState.Normal)
      {
         var rad = Mathf.Atan2(moveInput.x, moveInput.y);
         animator.SetFloat(turnSpeedHash, rad, 0.1f, Time.deltaTime);
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