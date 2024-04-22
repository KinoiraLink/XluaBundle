using UnityEngine;
namespace XLuaFramework.Managers
{
   public class SoundManager : MonoBehaviour
   {
      private AudioSource _musicAudio;
      private AudioSource _soundAudio;

      private float SoundVolume
      {
         get => PlayerPrefs.GetFloat("SoundVolume", 1.0f);
         set
         {
            _soundAudio.volume = value;
            PlayerPrefs.SetFloat("SoundVolume", value);
         }
      }

      private float MusicVolume
      {
         get => PlayerPrefs.GetFloat("MusicVolume", 1.0f);
         set
         {
            _musicAudio.volume = value;
            PlayerPrefs.SetFloat("MusicVolume",value);
         }
      }

      private void Awake()
      {
         _musicAudio = this.gameObject.AddComponent<AudioSource>();
         _musicAudio.playOnAwake = false;
         _musicAudio.loop = true;

         _soundAudio = this.gameObject.AddComponent<AudioSource>();
         _soundAudio.loop = false;
      }

      public void PlayMusic(string musicName)
      {
         if (this.MusicVolume < 0.1f)
         {
            return;
         }

         string oldName = "";
         if (_musicAudio.clip != null)
            oldName = _musicAudio.clip.name;
         if (oldName == musicName)
         {
            _musicAudio.Play();
            return;
         }
         
      
         Manager.Resources.LoadMusic(musicName, obj =>
         {
            _musicAudio.clip = obj as AudioClip;
            _musicAudio.Play();
         });
      }

      public void PauseMusic()
      {
         _musicAudio.Pause();
      }

      public void OnUnPauseMusic()
      {
         _musicAudio.UnPause();
      }

      public void StopMusic()
      {
         _musicAudio.Stop();
      }

      public void PlaySound(string soundName)
      {
         if (this.SoundVolume < 0.1)
            return;
      
         Manager.Resources.LoadSound(soundName, obj =>
         {
         
            _soundAudio.PlayOneShot(obj as AudioClip);
         });
      }

      public void SetMusicVolume(float value)
      {
         this.MusicVolume = value;
      }

      public void SetSoundVolume(float value)
      {
         this.SoundVolume = value;
      }
   }
}
