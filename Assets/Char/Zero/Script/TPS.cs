using HSR.NPRShader;
using UnityEngine;
using UnityEngine.InputSystem;

public class Tps : MonoBehaviour
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

   private static readonly int CacheSize = 3;
   private static readonly int IsAttack = Animator.StringToHash("攻击状态");
   private static readonly int AttackTrigger = Animator.StringToHash("攻击触发");

   public StarRailCharacterRenderingController renderingController;


   // 物理参数
   public float gravity = -9.81f; // 重力加速度
   public float maxHeight = 1.2f; // 最大跳跃高度
   public float fallMultiplier = 1.5f; // 下落速度的乘数
   public float groundCheckDistance = 0.4f; // 地面检测距离

   public Vector3 ditherAlphaPoint = Vector3.zero;

   // 移动速度参数
   private readonly float _crouchSpeed = 1.5f; // 蹲伏速度


   // 状态阈值
   private readonly float _crouchThreshold = 0f; // 蹲伏阈值
   private readonly float _inAirThreshold = 2.1f; // 空中阈值
   private readonly float _runSpeed = 5.5f; // 奔跑速度
   private readonly float _standThreshold = 1f; // 站立阈值
   private readonly Vector3[] _velCache = new Vector3[CacheSize]; // 速度缓存
   private readonly float _walkSpeed = 2.5f; // 行走速度
   private Animator _animator;
   private ArmState _armState = ArmState.Normal;

   private bool _attack;
   private int _attackHash;
   private int _AttackTriggerHash;


   // 移动相关
   private Vector3 _averageVel = Vector3.zero;
   private Transform _cameraTransform;

   // 组件引用
   private CharacterController _characterController;


   // 状态标志
   private bool _isAiming;
   private bool _isCrouching;
   private bool _isGrounded;
   private bool _isJumping;
   private bool _isOnAttack;
   private bool _isRunning;
   private bool _isSlide;
   private bool _isStanding;
   private bool _isWalking;


   private Vector3 _lastVelocity;
   private LocomotionState _locomotionState = LocomotionState.Idle;
   private Vector2 _moveInput;
   private int _moveSpeedHash;
   private PlayerInput _playerinput;
   private Vector3 _playerMovement = Vector3.zero;

   // 状态枚举
   private PlayerPosture _playerPosture = PlayerPosture.Stand;
   private Transform _playerTransform;

   // 动画参数哈希
   private int _postureHash;
   private int _turnSpeedHash;


   // 缓存索引
   private int _velCacheIndex;
   private float _verticalVelocity; // 当前垂直速度
   private int _verticalVelocityHash;


   private void Start()
   {
      _playerTransform = transform;
      _animator = GetComponent<Animator>();

      _postureHash = Animator.StringToHash("玩家姿态");
      _moveSpeedHash = Animator.StringToHash("移动速度");
      _turnSpeedHash = Animator.StringToHash("旋转速度");
      _verticalVelocityHash = Animator.StringToHash("垂直速度");
      _attackHash = Animator.StringToHash("攻击状态");
      _AttackTriggerHash = Animator.StringToHash("攻击触发");


      if (Camera.main != null) _cameraTransform = Camera.main.transform;

      //renderingController = GetComponent<StarRailCharacterRenderingController>();  // 获取组件
      //if (renderingController == null)
      {
         //Debug.LogError("组件获取失败");
      }

      _playerinput = GetComponent<PlayerInput>();
      _characterController = GetComponent<CharacterController>();
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
      if (_playerPosture != PlayerPosture.InAir)
      {
         var playerDeltaMovement = _animator.deltaPosition;
         playerDeltaMovement.y = _verticalVelocity * Time.deltaTime;
         _characterController.Move(playerDeltaMovement);
         _averageVel = AverageVel(_animator.velocity);
      }
      else
      {
         var playerDeltaMovement = _averageVel * Time.deltaTime;
         playerDeltaMovement.y = _verticalVelocity * Time.deltaTime;
         _characterController.Move(playerDeltaMovement);
      }
   }

   private void CheckGround()
   {
      _isGrounded = Physics.SphereCast(_playerTransform.position + Vector3.up * groundCheckDistance, _characterController.radius,
         Vector3.down,
         out var hit, groundCheckDistance - _characterController.radius + 2 * _characterController.skinWidth);
   }

   private Vector3 AverageVel(Vector3 newVel)
   {
      _velCache[_velCacheIndex] = newVel;
      _velCacheIndex++;
      _velCacheIndex %= CacheSize;
      var average = Vector3.zero;
      foreach (var vel in _velCache) average += vel;

      return average / CacheSize;
   }

   private void SwitchPlayerStates()
   {
      // 修改姿势状态判断
      if (!_isGrounded)
         _playerPosture = PlayerPosture.InAir;
      else if (_isCrouching)
         _playerPosture = PlayerPosture.Crouch;
      else
         _playerPosture = PlayerPosture.Stand;

      if (_moveInput.sqrMagnitude == 0)
         _locomotionState = LocomotionState.Idle;
      else if (!_isRunning)
         _locomotionState = LocomotionState.Walk;
      else
         _locomotionState = LocomotionState.Run;

      if (_isAiming)
         _armState = ArmState.Aim;
      else
         _armState = ArmState.Normal;
   }

   private void Jump()
   {
      if (_isGrounded && _isJumping)
         _verticalVelocity = Mathf.Sqrt(-2 * maxHeight * gravity);
   }

   private void CalculateGravity()
   {
      if (_isGrounded)
         _verticalVelocity = gravity * Time.deltaTime;
      else
      {
         if (_verticalVelocity <= 0)
            _verticalVelocity += gravity * fallMultiplier * Time.deltaTime;
         else
            _verticalVelocity += gravity * Time.deltaTime;
      }
   }

   private void CountInputDirection()
   {
      var cameraForwardPj = new Vector3(_cameraTransform.forward.x, 0, _cameraTransform.forward.z).normalized;
      _playerMovement = cameraForwardPj * _moveInput.y + _cameraTransform.right * _moveInput.x;
      _playerMovement = _playerTransform.InverseTransformDirection(_playerMovement);
   }

   private void SetAnimator()
   {
      if (_playerPosture == PlayerPosture.Stand)
      {
         _animator.SetFloat(_postureHash, _standThreshold, 0.1f, Time.deltaTime);
         switch (_locomotionState)
         {
            case LocomotionState.Idle:
               _animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
               break;
            case LocomotionState.Walk:
               _animator.SetFloat(_moveSpeedHash, _playerMovement.magnitude * _walkSpeed, 0.1f, Time.deltaTime);
               break;
            case LocomotionState.Run:
               _animator.SetFloat(_moveSpeedHash, _playerMovement.magnitude * _runSpeed, 0.1f, Time.deltaTime);
               break;
         }
      }
      else if (_playerPosture == PlayerPosture.Crouch)
      {
         _animator.SetFloat(_postureHash, _crouchThreshold, 0.1f, Time.deltaTime);
         switch (_locomotionState)
         {
            case LocomotionState.Idle:
               _animator.SetFloat(_moveSpeedHash, 0, 0.1f, Time.deltaTime);
               break;
            default:
               _animator.SetFloat(_moveSpeedHash, _playerMovement.magnitude * _crouchSpeed, 0.1f, Time.deltaTime);
               break;
         }
      }
      else if (_playerPosture == PlayerPosture.InAir)
      {
         _animator.SetFloat(_postureHash, _inAirThreshold);
         _animator.SetFloat(_verticalVelocityHash, _verticalVelocity);
      }


      if (_armState == ArmState.Normal)
      {
         var rad = Mathf.Atan2(_playerMovement.x, _playerMovement.z);
         _animator.SetFloat(_turnSpeedHash, rad, 0.1f, Time.deltaTime);
         _playerTransform.Rotate(0, rad * 200 * Time.deltaTime, 0f);
         //Debug.Log(rad);
      }

      //if (_isOnAttack == true)
      {
         // _animator.SetBool(IsAttack, true);
      }
      // SetAttack();
   }

   private float GetCameraDistance()
   {
      var checkPoint = new Vector3(_playerTransform.position.x, _playerTransform.position.y + 1f, _playerTransform.position.z);
      var cameraDistance = Vector3.Distance(checkPoint, _cameraTransform.position);
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

   private void SetAttack()
   {
      Debug.Log(_isOnAttack);
      if (_isOnAttack)
      {
         _animator.SetTrigger(AttackTrigger);
         _animator.SetBool(IsAttack, true);
         _isOnAttack = false;
      }
   }


   #region 输入

   public void GetMoveInput(InputAction.CallbackContext ctx)
   {
      _moveInput = ctx.ReadValue<Vector2>();
   }

   public void GetJumpInput(InputAction.CallbackContext ctx)
   {
      _isJumping = ctx.ReadValueAsButton();
   }

   public void GetCrouchInput(InputAction.CallbackContext ctx)
   {
      _isCrouching = ctx.ReadValueAsButton();
   }

   public void GetRunInput(InputAction.CallbackContext ctx)
   {
      _isRunning = ctx.ReadValueAsButton();
   }

   public void GetAimInput(InputAction.CallbackContext ctx)
   {
      _isAiming = ctx.ReadValueAsButton();
   }

   public void GetAttackInput(InputAction.CallbackContext ctx)
   {
      if (ctx.performed)
      {
         _isOnAttack = true;
         _animator.SetBool(_attackHash, true);
         _animator.SetTrigger(_AttackTriggerHash);
      }
      else if (ctx.canceled)
         _animator.SetBool(_attackHash, false);
   }

   #endregion
}