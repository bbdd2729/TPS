using System;
using System.Collections.Generic;
using HuHu;
using UnityEngine;
using ZZZ;

public class VFX_PoolManager : Singleton<VFX_PoolManager>
{
   [SerializeField] private List<effectData> effectDates = new List<effectData>();

   private Dictionary<CharacterNameList, Dictionary<string, Queue<GameObject>>> effectPool =
      new Dictionary<CharacterNameList, Dictionary<string, Queue<GameObject>>>();


   protected override void Awake()
   {
      base.Awake();
      InitEffectPools();
   }

   private void InitEffectPools()
   {
      if (effectDates.Count == 0)
      {
         return;
      }

      for (int i = 0; i < effectDates.Count; i++) //ѭ����Ч��������
      {
         //�ȴ���һ�������͵��ֵ�
         if (!effectPool.ContainsKey(effectDates[i].style))
         {
            effectPool.Add(effectDates[i].style, new Dictionary<string, Queue<GameObject>>());
         }

         for (int j = 0; j < effectDates[i].effectItemData.effectItems.Count; j++) //ѭ��ÿ����Ч�����еĶ����Ŀ
         {
            effectDates[i].effectItemData.effectItems[j].effectRotation =
               Quaternion.Euler(effectDates[i].effectItemData.effectItems[j].effectEulerAngle);

            for (int k = 0; k < effectDates[i].effectItemData.effectItems[j].count; k++)
            {
               //����ʵ��
               GameObject go = Instantiate(effectDates[i].effectItemData.effectItems[j].VFXPrefab);
               if (effectDates[i].effectItemData.effectItems[j].applyParentPos)
               {
                  //���ø�����
                  go.transform.parent = effectDates[i].effectItemData.effectItems[j].parentPos;
               }
               else
               {
                  go.transform.parent = this.transform;
               }

               //λ��
               go.transform.localPosition = Vector3.zero;
               //��ת
               go.transform.localRotation = effectDates[i].effectItemData.effectItems[j].effectRotation;
               //����
               go.SetActive(false);
               //�����ֵ�
               if (!effectPool[effectDates[i].style].ContainsKey(effectDates[i].effectItemData.effectItems[j].VFXName))
               {
                  effectPool[effectDates[i].style].Add(effectDates[i].effectItemData.effectItems[j].VFXName, new Queue<GameObject>());
               }

               effectPool[effectDates[i].style][effectDates[i].effectItemData.effectItems[j].VFXName].Enqueue(go);
            }
         }
      }
   }

   /// <summary>
   /// ֻ�������и�������Ч
   /// </summary>
   /// <param name="characterName"></param>
   /// <param name="effectName"></param>
   public void TryGetVFX(CharacterNameList characterName, string effectName)
   {
      if (effectPool.ContainsKey(characterName) && effectPool[characterName].ContainsKey(effectName) &&
          effectPool[characterName][effectName].Count > 0)
      {
         GameObject go = effectPool[characterName][effectName].Dequeue();
         go.SetActive(true);
         effectPool[characterName][effectName].Enqueue(go);
      }
      else
      {
         Debug.LogWarning(characterName + "����" + effectName + "���ֵ�" + "����ز�����");
      }
   }

   /// <summary>
   /// ��������û�и�������Ч
   /// </summary>
   /// <param name="characterName"></param>
   /// <param name="effectName"></param>
   /// <param name="worldPos"></param>
   /// <param name="quaternion"></param>
   public void GetVFX(CharacterNameList characterName, string effectName, Vector3 worldPos = default(Vector3),
      Quaternion quaternion = default(Quaternion))
   {
      if (effectPool.ContainsKey(characterName) && effectPool[characterName].ContainsKey(effectName) &&
          effectPool[characterName][effectName].Count > 0)
      {
         GameObject go = effectPool[characterName][effectName].Dequeue();
         go.transform.position = worldPos;
         go.transform.rotation = quaternion;
         go.SetActive(true);
         effectPool[characterName][effectName].Enqueue(go);
      }
      else
      {
         Debug.LogWarning(characterName + "����" + effectName + "���ֵ�" + "����ز�����");
      }
   }

   [Serializable]
   public class effectData
   {
      public CharacterNameList style;
      public VFXItemData effectItemData;
   }
}