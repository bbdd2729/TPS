using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
   public float cameraFOV = 60f;
   public CharacterInputOverrides inputOverrides;

   public float health = 100f;
   public float maxHealth = 100f;
   public float maxStamina = 100f;
   public float stamina = 100f;
   public float maxSpeed = 100f;
   public float jumpForce = 100f;
   public float gravity = -9.8f;
   public CharacterType characterType;

   public abstract void OnCharacterEnabled();
   public abstract void OnCharacterDisabled();
}

[Serializable]
public struct CharacterInputOverrides
{
   public List<ControlBinding> controlBindings;
}

[Serializable]
public struct ControlBinding
{
   public string actionName;
   public int bindingIndex;
   public string path;
}