using System;
using System.Collections.Generic;
using HuHu;
using UnityEngine;

public class SFX_PoolManager : Singleton<SFX_PoolManager>
{
   [SerializeField] private List<SoundItem> soundPools = new List<SoundItem>();

   private Dictionary<string, Dictionary<SoundStyle, Queue<GameObject>>> bigSoundCenter =
      new Dictionary<string, Dictionary<SoundStyle, Queue<GameObject>>>();

   private Dictionary<SoundStyle, Queue<GameObject>> soundCenter = new Dictionary<SoundStyle, Queue<GameObject>>();

   protected override void Awake()
   {
      base.Awake();
      InitSoundPool();
   }

   private void InitSoundPool()
   {
      if (soundPools.Count == 0)
      {
         return;
      }

      for (int i = 0; i < soundPools.Count; i++)
      {
         if (soundPools[i].ApplyBigCenter)
         {
            for (int j = 0; j < soundPools[i].soundCount; j++)
            {
               //ʵ����
               var go = Instantiate(soundPools[i].soundPrefab);
               //���ø�����
               go.transform.parent = this.transform;
               //�ڲ�
               go.SetActive(false);
               if (!bigSoundCenter.ContainsKey(soundPools[i].soundName))
               {
                  Debug.Log(soundPools[i].soundName + "��������");
                  bigSoundCenter.Add(soundPools[i].soundName, new Dictionary<SoundStyle, Queue<GameObject>>());
               }

               if (!bigSoundCenter[soundPools[i].soundName].ContainsKey(soundPools[i].soundStyle))
               {
                  bigSoundCenter[soundPools[i].soundName].Add(soundPools[i].soundStyle, new Queue<GameObject>());
               }

               bigSoundCenter[soundPools[i].soundName][soundPools[i].soundStyle].Enqueue(go);
            }
         }
         else
         {
            for (int j = 0; j < soundPools[i].soundCount; j++)
            {
               //ʵ����
               var go = Instantiate(soundPools[i].soundPrefab);
               //���ø�����
               go.transform.parent = this.transform;
               //�ڲ�
               go.SetActive(false);
               //�����ֵ�
               if (!soundCenter.ContainsKey(soundPools[i].soundStyle))
               {
                  //����Kay
                  soundCenter.Add(soundPools[i].soundStyle, new Queue<GameObject>());
                  //����ʵ������Value��������Ԥ����
                  soundCenter[soundPools[i].soundStyle].Enqueue(go);
               }
               else
               {
                  //ֻ�ü�Value
                  soundCenter[soundPools[i].soundStyle].Enqueue(go);
               }
            }
         }
      }
   }

   public void TryGetSoundPool(SoundStyle soundStyle, string soundName, Vector3 position)
   {
      if (bigSoundCenter.ContainsKey(soundName))
      {
         if (bigSoundCenter[soundName].TryGetValue(soundStyle, out var Q))
         {
            GameObject go = Q.Dequeue();
            go.transform.position = position;
            go.gameObject.SetActive(true);
            Q.Enqueue(go);
            // Debug.Log("��������"+ soundName+"������"+soundStyle);
         }
         else
         {
            // Debug.LogWarning(soundStyle + "�Ҳ���");
         }
      }
      else
      {
         // Debug.LogWarning(soundName + "�Ҳ���");
      }
   }

   public void TryGetSoundPool(SoundStyle soundStye, Vector3 position, Quaternion quaternion)
   {
      if (soundCenter.TryGetValue(soundStye, out var sound))
      {
         // Debug.Log(soundStye + "����");
         GameObject go = sound.Dequeue();
         go.transform.position = position;
         go.gameObject.SetActive(true);
         soundCenter[soundStye].Enqueue(go);
      }
      else
      {
         // Debug.Log(soundStye + "������");
      }
   }

   //��SoundName�Ĵ����ֵĸ�ϸ����Ƶ
   [Serializable]
   public class SoundItem
   {
      public SoundStyle soundStyle;
      public string soundName;
      public GameObject soundPrefab;
      public int soundCount;
      public bool ApplyBigCenter;
   }
}