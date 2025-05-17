using HSR.NPRShader;
using UnityEngine;

public class SharedAbilityManager : MonoBehaviour
{
   void Start()
   {
   }

   // Update is called once per frame
   void Update()
   {
      SetDitherAlpha();
   }

   #region Singleton

   public static SharedAbilityManager Instance { get; private set; }

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

   #region 子系统配置

   [Header("虚化系统")] public StarRailCharacterRenderingController renderingController;

   private void SetDitherAlpha()
   {
      if (renderingController != null)
         renderingController.DitherAlpha = CharacterManager.Instance.GetCameraToPointDistance(new Vector3(0, 1, 0));
   }

   #endregion

   #region 运行时数据

   #endregion

   #region 初始化

   #endregion
}