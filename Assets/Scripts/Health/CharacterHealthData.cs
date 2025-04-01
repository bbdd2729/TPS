using UnityEngine;

[CreateAssetMenu(fileName = "CharacterHealthData", menuName = "Create/Asset/CharacterHealthData")]
public class CharacterHealthData : ScriptableObject
{
   [field: SerializeField] public HealthData healthData { get; private set; }
   //field������ { get;  set; }�������������²��ܶ��������л�
}