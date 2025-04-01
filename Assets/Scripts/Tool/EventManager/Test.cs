using UnityEngine;

namespace ybh
{
   public class Test : MonoBehaviour
   {
      // Start is called before the first frame update
      void Start()
      {
      }

      // Update is called once per frame
      void Update()
      {
         if (CharacterInputSystem.MainInstance.Jump)
         {
            GameEventsManager.MainInstance.CallEvent("�¼�");
         }
      }

      private void OnEnable()
      {
         GameEventsManager.MainInstance.AddEventListening("�¼�", SendText);
      }

      private void OnDisable()
      {
         GameEventsManager.MainInstance.ReMoveEvent("�¼�", SendText);
      }

      private void SendText()
      {
         Debug.Log("�¼��ɹ�������");
      }
   }
}