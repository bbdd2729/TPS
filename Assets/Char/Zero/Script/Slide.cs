using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class Slide : MonoBehaviour
{
   [Header("组件引用")] public SkinnedMeshRenderer[] characterRenderers; // 改为数组，支持多个渲染器

   [Header("残影设置")] public AfterimageSettings afterimageSettings = new();

   private Transform _afterimageContainer;

   private Animator _animator;
   private bool _isDodging;
   private readonly List<Queue<GameObject>> afterimagePools = new();

   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _afterimageContainer = new GameObject("Afterimages Container").transform;
      InitializePools();
   }

   private void OnDestroy()
   {
      if (_afterimageContainer != null) Destroy(_afterimageContainer.gameObject);
   }

   // 初始化多个对象池（每个SkinnedMeshRenderer一个池）
   private void InitializePools()
   {
      if (characterRenderers == null || characterRenderers.Length == 0)
      {
         Debug.LogError("没有指定任何SkinnedMeshRenderer！");
         return;
      }

      afterimagePools.Clear();

      for (var i = 0; i < characterRenderers.Length; i++)
      {
         var pool = new Queue<GameObject>();
         for (var j = 0; j < afterimageSettings.poolSize; j++)
         {
            var afterimage = CreateAfterimageObject(i);
            afterimage.SetActive(false);
            pool.Enqueue(afterimage);
         }

         afterimagePools.Add(pool);
      }
   }

   // 创建单个残影对象（指定渲染器索引）
   private GameObject CreateAfterimageObject(int rendererIndex)
   {
      var obj = new GameObject($"Afterimage_{rendererIndex}");
      obj.transform.SetParent(_afterimageContainer);

      // 添加Mesh组件
      var filter = obj.AddComponent<MeshFilter>();
      var renderer = obj.AddComponent<MeshRenderer>();

      // 设置材质
      renderer.material = afterimageSettings.afterimageMaterial != null
         ? new Material(afterimageSettings.afterimageMaterial)
         : new Material(characterRenderers[rendererIndex].material);

      // 初始透明
      var color = afterimageSettings.startColor;
      color.a = 0;
      renderer.material.color = color;

      return obj;
   }

   // 从对象池获取残影
   private GameObject GetAfterimageFromPool(int rendererIndex)
   {
      if (rendererIndex >= 0 && rendererIndex < afterimagePools.Count)
      {
         if (afterimagePools[rendererIndex].Count > 0) return afterimagePools[rendererIndex].Dequeue();

         Debug.LogWarning($"Afterimage pool {rendererIndex} empty! Consider increasing pool size.");
         return CreateAfterimageObject(rendererIndex);
      }

      return null;
   }

   // 返回残影到对象池
   private void ReturnAfterimageToPool(GameObject afterimage, int rendererIndex)
   {
      if (rendererIndex >= 0 && rendererIndex < afterimagePools.Count)
      {
         afterimage.SetActive(false);
         afterimagePools[rendererIndex].Enqueue(afterimage);

         // 清理可能存在的Mesh对象
         var filter = afterimage.GetComponent<MeshFilter>();
         if (filter.sharedMesh != null && !filter.sharedMesh.name.Contains("Clone")) Destroy(filter.sharedMesh);
      }
   }

   // 动画事件调用的方法
   public void OnDodgeStart()
   {
      if (!afterimageSettings.enabled || _isDodging) return;

      StartCoroutine(GenerateAfterimages());
   }

   // 生成残影的协程
   private IEnumerator GenerateAfterimages()
   {
      _isDodging = true;
      float timer = 0;

      while (timer < afterimageSettings.duration)
      {
         SpawnAfterimages();
         timer += afterimageSettings.spawnInterval;
         yield return new WaitForSeconds(afterimageSettings.spawnInterval);
      }

      _isDodging = false;
   }

   // 生成所有部分的残影
   private void SpawnAfterimages()
   {
      for (var i = 0; i < characterRenderers.Length; i++)
      {
         if (characterRenderers[i] == null) continue;

         var afterimage = GetAfterimageFromPool(i);
         if (afterimage == null) continue;

         afterimage.transform.SetParent(null);
         afterimage.transform.position = characterRenderers[i].transform.position;
         afterimage.transform.rotation = characterRenderers[i].transform.rotation;

         // 烘焙当前网格状态
         var bakedMesh = new Mesh();
         characterRenderers[i].BakeMesh(bakedMesh);

         // 应用烘焙后的网格
         var filter = afterimage.GetComponent<MeshFilter>();
         filter.sharedMesh = bakedMesh;

         // 设置材质颜色
         var renderer = afterimage.GetComponent<MeshRenderer>();
         renderer.material.color = afterimageSettings.startColor;

         afterimage.SetActive(true);

         // 开始淡出效果
         StartCoroutine(FadeOutAfterimage(afterimage, renderer, bakedMesh, i));
      }
   }

   // 残影淡出效果
   private IEnumerator FadeOutAfterimage(GameObject afterimage, MeshRenderer renderer, Mesh bakedMesh, int rendererIndex)
   {
      var alpha = afterimageSettings.startColor.a;
      var material = renderer.material;
      var color = material.color;

      while (alpha > 0)
      {
         alpha -= Time.deltaTime * afterimageSettings.fadeSpeed;
         color.a = alpha;
         material.color = color;
         yield return null;
      }

      // 销毁临时创建的Mesh
      Destroy(bakedMesh);

      // 将残影返回对象池
      ReturnAfterimageToPool(afterimage, rendererIndex);
   }

   [ContextMenu("Test Afterimage")]
   private void TestAfterimage()
   {
      if (!Application.isPlaying) return;
      OnDodgeStart();
   }

   [Serializable]
   public class AfterimageSettings
   {
      [Tooltip("是否启用残影效果")] public bool enabled = true;

      [Tooltip("残影持续时间(秒)")] public float duration = 0.5f;

      [Tooltip("生成残影的时间间隔(秒)")] public float spawnInterval = 0.05f;

      [Tooltip("残影初始颜色和透明度")] public Color startColor = new(1, 1, 1, 0.7f);

      [Tooltip("残影淡出速度")] public float fadeSpeed = 2f;

      [Tooltip("残影材质(如果为空则使用角色材质)")] public Material afterimageMaterial;

      [Tooltip("残影对象池大小")] public int poolSize = 15;
   }
}