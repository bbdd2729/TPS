using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AdvancedAfterimageEffect : MonoBehaviour
{
   [Header("渲染器设置")] public Renderer[] targetRenderers;

   [Header("残影设置")] public AfterimageSettings settings = new();

   private Transform _afterimageContainer;
   private readonly List<Queue<GameObject>> _afterimagePools = new();

   private Animator _animator;
   private bool _isGenerating;
   private readonly List<Material[]> _originalMaterials = new();

   private void Awake()
   {
      _animator = GetComponent<Animator>();
      InitializeEffect();
   }

   private void OnDestroy()
   {
      if (_afterimageContainer != null) Destroy(_afterimageContainer.gameObject);
   }

   private void InitializeEffect()
   {
      // 创建容器对象
      _afterimageContainer = new GameObject("Afterimages Container").transform;

      // 验证渲染器
      if (targetRenderers == null || targetRenderers.Length == 0)
      {
         targetRenderers = GetComponentsInChildren<Renderer>(true);
         Debug.LogWarning("自动查找并添加了 " + targetRenderers.Length + " 个渲染器");
      }

      // 存储原始材质
      foreach (var renderer in targetRenderers)
         if (renderer != null)
            _originalMaterials.Add(renderer.sharedMaterials);

      // 初始化对象池
      InitializePools();
   }

   private void InitializePools()
   {
      _afterimagePools.Clear();

      for (var i = 0; i < targetRenderers.Length; i++)
      {
         var pool = new Queue<GameObject>();
         for (var j = 0; j < settings.poolSize; j++)
         {
            pool.Enqueue(CreateAfterimageObject(i));
         }

         _afterimagePools.Add(pool);
      }
   }

   private GameObject CreateAfterimageObject(int rendererIndex)
   {
      var obj = new GameObject($"Afterimage_{rendererIndex}");
      obj.transform.SetParent(_afterimageContainer);
      obj.SetActive(false);

      // 添加必要的组件
      var filter = obj.AddComponent<MeshFilter>();
      var renderer = obj.AddComponent<MeshRenderer>();

      // 初始化材质
      if (settings.afterimageMaterial != null && settings.overrideAllMaterials)
      {
         var mats = new Material[targetRenderers[rendererIndex].sharedMaterials.Length];
         for (var i = 0; i < mats.Length; i++)
         {
            mats[i] = new Material(settings.afterimageMaterial);
            mats[i].color = settings.startColor;
         }

         renderer.sharedMaterials = mats;
      }

      return obj;
   }

   public void TriggerAfterimage()
   {
      if (!settings.enabled || _isGenerating) return;
      StartCoroutine(GenerateAfterimages());
   }

   private IEnumerator GenerateAfterimages()
   {
      _isGenerating = true;
      float timer = 0;

      while (timer < settings.duration)
      {
         SpawnAfterimages();
         timer += settings.spawnInterval;
         yield return new WaitForSeconds(settings.spawnInterval);
      }

      _isGenerating = false;
   }

   private void SpawnAfterimages()
   {
      for (var i = 0; i < targetRenderers.Length; i++)
      {
         if (targetRenderers[i] == null) continue;

         var afterimage = GetAfterimageFromPool(i);
         if (afterimage == null) continue;

         ConfigureAfterimage(afterimage, i);
         StartCoroutine(FadeOutAfterimage(afterimage, i));
      }
   }

   private GameObject GetAfterimageFromPool(int rendererIndex)
   {
      if (rendererIndex < 0 || rendererIndex >= _afterimagePools.Count)
         return null;

      if (_afterimagePools[rendererIndex].Count > 0)
         return _afterimagePools[rendererIndex].Dequeue();

      Debug.LogWarning($"Pool {rendererIndex} empty, creating new afterimage");
      return CreateAfterimageObject(rendererIndex);
   }

   private void ConfigureAfterimage(GameObject afterimage, int rendererIndex)
   {
      var targetRenderer = targetRenderers[rendererIndex];
      afterimage.transform.SetPositionAndRotation(
         targetRenderer.transform.position,
         targetRenderer.transform.rotation
      );

      var filter = afterimage.GetComponent<MeshFilter>();
      var meshRenderer = afterimage.GetComponent<MeshRenderer>();

      // 处理不同类型的渲染器
      if (targetRenderer is SkinnedMeshRenderer skinnedRenderer)
      {
         var mesh = new Mesh();
         skinnedRenderer.BakeMesh(mesh);
         filter.sharedMesh = mesh;
      }
      else if (targetRenderer is MeshRenderer)
      {
         filter.sharedMesh = targetRenderer.GetComponent<MeshFilter>().sharedMesh;
      }

      // 应用材质
      if (!settings.overrideAllMaterials)
      {
         var mats = new Material[targetRenderer.sharedMaterials.Length];
         for (var i = 0; i < mats.Length; i++)
         {
            mats[i] = new Material(targetRenderer.sharedMaterials[i]);
            mats[i].color = settings.startColor;
         }

         meshRenderer.sharedMaterials = mats;
      }

      afterimage.SetActive(true);
   }

   private IEnumerator FadeOutAfterimage(GameObject afterimage, int rendererIndex)
   {
      var renderer = afterimage.GetComponent<MeshRenderer>();
      var materials = renderer.materials;
      var startAlpha = settings.startColor.a;
      var alpha = startAlpha;

      while (alpha > 0)
      {
         alpha -= Time.deltaTime * settings.fadeSpeed;
         foreach (var mat in materials)
         {
            var color = mat.color;
            color.a = alpha;
            mat.color = color;
         }

         yield return null;
      }

      CleanupAfterimage(afterimage, rendererIndex);
   }

   private void CleanupAfterimage(GameObject afterimage, int rendererIndex)
   {
      // 清理网格
      var filter = afterimage.GetComponent<MeshFilter>();
      if (filter.sharedMesh != null && !filter.sharedMesh.name.Contains("Clone")) Destroy(filter.sharedMesh);

      // 重置材质
      var renderer = afterimage.GetComponent<MeshRenderer>();
      foreach (var mat in renderer.materials) Destroy(mat);

      afterimage.SetActive(false);
      _afterimagePools[rendererIndex].Enqueue(afterimage);
   }

   [Serializable]
   public class AfterimageSettings
   {
      public bool enabled = true;
      public float duration = 0.5f;
      public float spawnInterval = 0.05f;
      public Color startColor = new(1, 1, 1, 0.7f);
      public float fadeSpeed = 2f;
      public Material afterimageMaterial;
      public int poolSize = 15;

      [Tooltip("强制覆盖所有材质")] public bool overrideAllMaterials = true;
   }
}