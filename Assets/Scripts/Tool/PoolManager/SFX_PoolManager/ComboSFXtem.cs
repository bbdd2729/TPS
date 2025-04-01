using UnityEngine;

public class ComboSFXtem : PoolItemBase
{
   [SerializeField] private ComboData comboData;
   [SerializeField] private SoundStyle soundStyle;
   private AudioSource audioSource;

   private void Awake()
   {
      audioSource = GetComponent<AudioSource>();
   }

   private void Update()
   {
      if (!audioSource.isPlaying)
      {
         StopAudioPlay();
      }
   }

   public void SetSoundStyle(SoundStyle soundStyle)
   {
      this.soundStyle = soundStyle;
   }

   public void GetComboData(ComboData comboData)
   {
      this.comboData = comboData;
   }

   protected override void Spawn()
   {
      base.Spawn();

      if (soundStyle == SoundStyle.ComboVoice)
      {
         audioSource.clip = comboData.characterVoice[Random.Range(0, comboData.characterVoice.Length)];
      }
      else if (soundStyle == SoundStyle.WeaponSound)
      {
         audioSource.clip = comboData.weaponSound[Random.Range(0, comboData.weaponSound.Length)];
      }

      if (audioSource.clip == null)
      {
         return;
      }

      audioSource.Play();
   }

   private void StopAudioPlay()
   {
      this.gameObject.SetActive(false);
   }
}