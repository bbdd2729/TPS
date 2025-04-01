using UnityEngine;

public class PoolItemBase : MonoBehaviour
{
   private void OnEnable()
   {
      Spawn();
   }

   private void OnDisable()
   {
      ReSycle();
   }

   protected virtual void Spawn()
   {
   }

   protected virtual void ReSycle()
   {
   }
}