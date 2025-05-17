using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// 全局输入管理器（支持动态键位重绑定+多角色控制模式）
/// </summary>
[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
   // ==================== 初始化 ====================
   private void InitializeInput()
   {
      _playerInput = GetComponent<PlayerInput>();

      if (_inputActions != null)
      {
         _gameplayMap = _inputActions.FindActionMap(_gameplayMapName);
         _uiMap = _inputActions.FindActionMap(_uiMapName);
      }

      RegisterCallbacks();
      SwitchInputMap(_currentMode);
   }

   private void RegisterCallbacks()
   {
      // 移动输入（持续检测）
      _playerInput.actions["Move"].performed += ctx =>
         OnMoveInput?.Invoke(ctx.ReadValue<Vector2>());
      _playerInput.actions["Move"].canceled += _ =>
         OnMoveInput?.Invoke(Vector2.zero);

      // 离散动作
      _playerInput.actions["Jump"].performed += _ => OnJumpPressed?.Invoke();
      _playerInput.actions["Interact"].performed += _ => OnInteractPressed?.Invoke();

      // 技能输入（带缓冲）
      _playerInput.actions["Skill1"].performed += _ => BufferSkillInput(0);
      _playerInput.actions["Skill2"].performed += _ => BufferSkillInput(1);
      _playerInput.actions["Skill3"].performed += _ => BufferSkillInput(2);

      // 角色切换
      _playerInput.actions["SwitchCharacter1"].performed += _ => OnCharacterSwitch?.Invoke(0);
      _playerInput.actions["SwitchCharacter2"].performed += _ => OnCharacterSwitch?.Invoke(1);
      _playerInput.actions["SwitchCharacter3"].performed += _ => OnCharacterSwitch?.Invoke(2);
   }

   // ==================== 核心功能 ====================

   #region 输入模式控制

   public void SwitchInputMap(InputMode newMode)
   {
      _currentMode = newMode;

      switch (newMode)
      {
         case InputMode.Gameplay:
            _uiMap?.Disable();
            _gameplayMap?.Enable();
            Cursor.lockState = CursorLockMode.Locked;
            break;

         case InputMode.UI:
            _gameplayMap?.Disable();
            _uiMap?.Enable();
            Cursor.lockState = CursorLockMode.None;
            break;

         case InputMode.Dialogue:
            _gameplayMap?.Disable();
            _uiMap?.Disable();
            Cursor.lockState = CursorLockMode.None;
            break;
      }
   }

   #endregion

   #region Singleton

   public static InputHandler Instance { get; private set; }
   private void Awake() => InitializeSingleton();

   private void InitializeSingleton()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
         InitializeInput();
      }
      else
      {
         Destroy(gameObject);
      }
   }

   #endregion

   #region 输入配置

   [Header("输入配置")] [SerializeField] private InputActionAsset _inputActions;

   [SerializeField] private string _gameplayMapName = "Gameplay";
   [SerializeField] private string _uiMapName = "UI";

   // 当前输入模式
   public enum InputMode
   {
      Gameplay,
      UI,
      Dialogue
   }

   private InputMode _currentMode = InputMode.Gameplay;

   // 输入缓冲设置
   private float _inputBufferTime = 0.2f;
   private float _lastSkillInputTime;

   #endregion

   #region 输入事件（UnityEvent）

   [Header("角色移动")] public UnityEvent<Vector2> OnMoveInput; // WASD/摇杆输入

   [Header("基础动作")] public UnityEvent OnJumpPressed;

   public UnityEvent OnInteractPressed;

   [Header("技能系统")] public UnityEvent<int> OnSkillTriggered; // 参数为技能槽位(0-3)

   [Header("角色切换")] public UnityEvent<int> OnCharacterSwitch; // 参数为角色索引

   #endregion

   #region 运行时变量

   private PlayerInput _playerInput;
   private InputActionMap _gameplayMap;
   private InputActionMap _uiMap;
   private BaseCharacter _currentCharacter;

   #endregion

   #region 技能输入缓冲

   private void BufferSkillInput(int skillSlot)
   {
      _lastSkillInputTime = Time.time;

      // 直接尝试触发
      if (TryTriggerSkill(skillSlot)) return;

      // 缓冲期内持续检测
      StartCoroutine(SkillBufferCoroutine(skillSlot));
   }

   private IEnumerator SkillBufferCoroutine(int skillSlot)
   {
      while (Time.time - _lastSkillInputTime <= _inputBufferTime)
      {
         if (TryTriggerSkill(skillSlot)) yield break;
         yield return null;
      }
   }

   private bool TryTriggerSkill(int skillSlot)
   {
      if (_currentCharacter == null) return false;

      var ability = _currentCharacter.GetAbility(skillSlot);
      if (ability != null && ability.CanActivate())
      {
         OnSkillTriggered?.Invoke(skillSlot);
         ability.Activate();
         return true;
      }

      return false;
   }

   #endregion

   #region 角色控制绑定

   public void BindCharacter(BaseCharacter character)
   {
      _currentCharacter = character;

      // 更新角色专属输入设置
      if (character.InputOverrides != null)
      {
         foreach (var

   override in character.InputOverrides)
   {
      _playerInput.actions[override.actionName]
   .ApplyBindingOverride(override.bindingIndex, override.path);
}

}
}

#endregion

#region 键位重绑定

public void RebindAction(string actionName, int bindingIndex, Action<string> onComplete)
{
   var action = _playerInput.actions[actionName];
   if (action == null) return;

   action.Disable();
   action.PerformInteractiveRebinding(bindingIndex)
      .WithControlsExcluding("Mouse")
      .OnMatchWaitForAnother(0.1f)
      .OnComplete(operation =>
      {
         string newBinding = operation.selectedControl.path;
         action.ApplyBindingOverride(bindingIndex, newBinding);
         onComplete?.Invoke(newBinding);
         operation.Dispose();
         action.Enable();
      })
      .Start();
}

#endregion

// ==================== 调试工具 ====================
#if UNITY_EDITOR
[Header("Debug")]
[SerializeField]
private bool _logInputEvents

;

private void LogInput(string context)
{
   if (_logInputEvents)
      Debug.Log($"[Input] {context} | Mode: {_currentMode}");
}
#endif
}