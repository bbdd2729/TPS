using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterManager : MonoBehaviour
{
   #region 事件系统

   /// <summary>
   ///    角色切换事件（参数：新角色GameObject）
   /// </summary>
   public event Action<GameObject> OnCharacterSwitched;

   #endregion

   #region Singleton

   public static CharacterManager Instance { get; private set; }

   private void Awake()
   {
      if (Instance == null)
      {
         Instance = this;
         DontDestroyOnLoad(gameObject);
      }
      else
      {
         Destroy(gameObject);
      }
   }

   #endregion

   #region 公开属性

   [Header("角色设置")] [Tooltip("角色预制体列表（顺序对应切换数字键）")]
   public List<GameObject> characterPrefabs = new();

   [Tooltip("初始激活的角色索引")] public int initialCharacterIndex;

   [Header("输入设置")] public PlayerInput playerInput;

   public InputActionReference switchCharacterAction;

   [Header("相机设置")] public CinemachineVirtualCamera mainCamera;

   // 当前控制的角色（只读）
   public GameObject CurrentCharacter { get; private set; }
   public int CurrentCharacterIndex { get; private set; }

   #endregion

   #region 私有变量

   private readonly List<GameObject> _instantiatedCharacters = new();
   private BaseCharacter _currentCharacterScript;

   #endregion

   #region 生命周期

   private void Start()
   {
      InitializeCharacters();
      SwitchCharacter(initialCharacterIndex);

      // 绑定切换输入
      switchCharacterAction.action.performed += OnSwitchCharacterInput;
   }

   private void OnDestroy()
   {
      switchCharacterAction.action.performed -= OnSwitchCharacterInput;
   }

   #endregion

   #region 角色管理

   /// <summary>
   ///    初始化所有角色实例
   /// </summary>
   private void InitializeCharacters()
   {
      if (characterPrefabs.Count == 0)
      {
         Debug.LogError("未分配角色预制体！");
         return;
      }

      foreach (var prefab in characterPrefabs)
      {
         var character = Instantiate(prefab, transform.position, Quaternion.identity);
         character.SetActive(false);
         _instantiatedCharacters.Add(character);
      }
   }

   /// <summary>
   ///    切换角色核心逻辑
   /// </summary>
   public void SwitchCharacter(int newIndex)
   {
      // 边界检查
      if (newIndex < 0 || newIndex >= _instantiatedCharacters.Count)
      {
         Debug.LogWarning($"无效的角色索引: {newIndex}");
         return;
      }

      // 相同角色不重复切换
      if (newIndex == CurrentCharacterIndex && CurrentCharacter != null)
         return;

      // 禁用当前角色
      if (CurrentCharacter != null)
      {
         _currentCharacterScript?.OnCharacterDisabled();
         CurrentCharacter.SetActive(false);
      }

      // 更新索引并激活新角色
      CurrentCharacterIndex = newIndex;
      CurrentCharacter = _instantiatedCharacters[CurrentCharacterIndex];
      CurrentCharacter.SetActive(true);

      // 获取角色控制脚本
      _currentCharacterScript = CurrentCharacter.GetComponent<BaseCharacter>();
      _currentCharacterScript?.OnCharacterEnabled();

      // 更新系统绑定
      UpdateCameraFollow();
      UpdateInputBindings();
      OnCharacterSwitched?.Invoke(CurrentCharacter);
   }

   /// <summary>
   ///    通过输入事件切换角色
   /// </summary>
   private void OnSwitchCharacterInput(InputAction.CallbackContext context)
   {
      // 数字键1-3对应角色0-2
      var keyNumber = (int)context.ReadValue<float>();
      var targetIndex = keyNumber - 1;

      if (targetIndex >= 0 && targetIndex < _instantiatedCharacters.Count) SwitchCharacter(targetIndex);
   }

   #endregion

   #region 系统同步

   /// <summary>
   ///    更新相机跟随目标
   /// </summary>
   private void UpdateCameraFollow()
   {
      if (mainCamera != null && CurrentCharacter != null)
      {
         mainCamera.Follow = CurrentCharacter.transform;
         mainCamera.LookAt = CurrentCharacter.transform;

         // 应用角色专属相机设置
         if (_currentCharacterScript != null) mainCamera.m_Lens.FieldOfView = _currentCharacterScript.cameraFOV;
      }
   }

   /// <summary>
   ///    更新输入绑定
   /// </summary>
   private void UpdateInputBindings()
   {
      if (playerInput != null && _currentCharacterScript != null)
      {
         // 清空原有绑定
         playerInput.actions.Disable();
         // 应用角色专属输入配置
         foreach (var binding in _currentCharacterScript.inputOverrides.controlBindings)
            playerInput.actions.FindAction(binding.actionName).ApplyBindingOverride(binding.bindingIndex, binding.path);

         playerInput.actions.Enable();
      }
   }

   #endregion

   #region 辅助功能

   /// <summary>
   ///    获取所有角色类型（用于UI显示）
   /// </summary>
   public List<CharacterType> GetAvailableCharacterTypes()
   {
      List<CharacterType> types = new();
      foreach (var character in _instantiatedCharacters)
         if (character.TryGetComponent(out BaseCharacter bc))
            types.Add(bc.characterType);

      return types;
   }

   /// <summary>
   ///    通过类型切换角色
   /// </summary>
   public void SwitchCharacterByType(CharacterType type)
   {
      for (var i = 0; i < _instantiatedCharacters.Count; i++)
         if (_instantiatedCharacters[i].GetComponent<BaseCharacter>().characterType == type)
         {
            SwitchCharacter(i);
            return;
         }
   }

   #endregion


   // Update is called once per frame
   //void Update()
}