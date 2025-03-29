using UnityEngine;
using UnityEngine.InputSystem;

public class Anamation : MonoBehaviour
{
   // Start is called once before the first execution of Update after the MonoBehaviour is created
   private void Start()
   {
   }

   // Update is called once per frame
   private void Update()
   {
   }

   public void OnFire(InputAction.CallbackContext context)
   {
      if (context.performed) Debug.Log("Fire!");
   }
}