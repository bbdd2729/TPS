using UnityEngine;

public class CameraHideCursor : MonoBehaviour
{
   private void Start()
   {
      UpdateCorcur();
   }

   private void UpdateCorcur()
   {
      Cursor.visible = false;
      Cursor.lockState = CursorLockMode.Locked;
   }
}