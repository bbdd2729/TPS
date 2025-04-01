using UnityEngine;
using ZZZ;

public enum AttackStyle
{
   Attack,
   Skill,
   FinishSkill,
   SwitchSkill,
}

[CreateAssetMenu(fileName = "ComboData", menuName = "Create/Asset/ComboData")]
public class ComboData : ScriptableObject
{
   [SerializeField, Header("�����������ļ���ʹ��")]
   public CharacterNameList characterName;

   [SerializeField, Header("��������")] private AttackStyle _attackStyle;
   [SerializeField] private string _comboName;
   [SerializeField] private float _comboColdTime;
   [SerializeField] private float _comboDamage;
   [SerializeField] private float _attackDistance;
   [SerializeField] private float _comboOffset;
   [SerializeField] private string[] _hitName;
   [SerializeField] private string[] _parryName;

   [SerializeField, Header("��Ч����")] bool appAudioPrefab = false;
   [SerializeField] private AudioClip[] _weaponSound;
   [SerializeField] private AudioClip[] _characterVoice;

   [SerializeField, Header("������ÿһ��������������ͬ�������")]
   private SoundStyle _universalSound;


   [SerializeField, Header("�����")] private int _ATKCount;

   [SerializeField] private float _pauseFrameTime;
   [SerializeField] private float[] _shakeForceList;
   [SerializeField] private float[] _pauseFrameTimeList;


   #region ���Է�װ

   public AttackStyle attackStyle => _attackStyle;
   public string comboName => _comboName;
   public float comboDamage => _comboDamage;
   public float comboColdTime => _comboColdTime;
   public float attackDistance => _attackDistance;
   public float comboOffset => _comboOffset;

   public AudioClip[] weaponSound => _weaponSound;
   public AudioClip[] characterVoice => _characterVoice;
   public string hitName => _hitName[Random.Range(0, _hitName.Length)];

   public string parryName => _parryName[Random.Range(0, _parryName.Length)];
   public float[] shakeForce => _shakeForceList;
   public SoundStyle universalSound => _universalSound;

   public float pauseFrameTime => _pauseFrameTime;

   public float[] pauseFrameTimeList => _pauseFrameTimeList;

   public int ATKCount => _ATKCount;

   public bool AppAudioPrefab => appAudioPrefab;

   #endregion
}