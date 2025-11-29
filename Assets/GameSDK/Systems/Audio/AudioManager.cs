using UnityEngine;
using System;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

    public class AudioManager : Singleton<AudioManager>
    {
        public AudioMixer inGameMixer;
        public AudioMixer bgMixer;
        public AudioSource trainMoveAudioSource;

        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioSource audioSource;
            public void Play()
            {
                audioSource.Play();
            }

            public void Stop()
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            public void StopImmidiate()
            {
                if (audioSource != null)
                {
                    audioSource.Stop();
                }
            }
        }

        public Sound[] fxSounds;

        #region Unity Calls
        public void Initialize()
        {
           // bgMixer.SetFloat("Val", SaveDataHandler.Instance.BgSoundValue);
          //  inGameMixer.SetFloat("Val", SaveDataHandler.Instance.InGameSoundFXValue);

        }
        protected override void Awake()
        {
            base.Awake();
            for (int i = 0; i < initialSize; i++)
            {
                AddNewSource();
            }


        }
        private void Start()
        {
            Initialize();
        }
    #endregion

    private AudioSource GetAudioSource(string name)
    {
        Sound sound = Array.Find(fxSounds, s => s.name == name);
        if (sound == null) return null;

        if (sound.audioSource == null)
        {
            Transform _tempTransform = transform.Find(name);
            if (_tempTransform)
            {
                sound.audioSource = _tempTransform.GetComponent<AudioSource>();
            }
        }

        return sound.audioSource;

    }
        public void PlaySound(string name)
        {
            GetAudioSource(name).Play();
        }
    public void PlaySound(string name,float minPitch,float maxPitch)
    {
        AudioSource audioS = GetAudioSource(name);
        audioS.pitch = UnityEngine.Random.Range(minPitch, maxPitch);
        audioS.Play();
    }
    public void StopSound(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Sound sound = Array.Find(fxSounds, s => s.name == name);
            if (sound != null)
            {
                if (sound.audioSource == null)
                {
                    Transform _tempTransform = transform.Find(name);
                    if (_tempTransform)
                    {
                        sound.audioSource = _tempTransform.GetComponent<AudioSource>();
                    }
                }

                //sound.Stop();
                sound.StopImmidiate();
            }
            else
            {
                Debug.LogWarning("AudioManager -- Sound not found:" + name);
            }
        }

        public void StopAllSounds()
        {
            foreach (var item in fxSounds)
            {
                item.audioSource.Stop();
            }
        }

        public void PauseSound(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Sound sound = Array.Find(fxSounds, s => s.name == name);
            if (sound != null)
            {
                if (sound.audioSource == null)
                {
                    Transform _tempTransform = transform.Find(name);
                    if (_tempTransform)
                    {
                        sound.audioSource = _tempTransform.GetComponent<AudioSource>();
                    }
                }

                //sound.Stop();

                sound.audioSource.Pause();

            }
            else
            {
                Debug.LogWarning("AudioManager -- Sound not found:" + name);
            }
        }
        public void UnPauseSound(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }

            Sound sound = Array.Find(fxSounds, s => s.name == name);
            if (sound != null)
            {
                if (sound.audioSource == null)
                {
                    Transform _tempTransform = transform.Find(name);
                    if (_tempTransform)
                    {
                        sound.audioSource = _tempTransform.GetComponent<AudioSource>();
                    }
                }

                //sound.Stop();

                sound.audioSource.UnPause();
            }
            else
            {
                Debug.LogWarning("AudioManager -- Sound not found:" + name);
            }
        }
        Coroutine callerCoroutine;
        public void PlayCallerAudio(AudioClip characterClip,AudioClip numberClip)
        {
            if(callerCoroutine != null) StopCoroutine(callerCoroutine);
            callerCoroutine = StartCoroutine(PlaySequence(characterClip, numberClip));
        }

        private IEnumerator PlaySequence(AudioClip characterClip, AudioClip numberClip)
        {
            trainMoveAudioSource.clip = characterClip;
            trainMoveAudioSource.Play();
            yield return new WaitForSeconds(characterClip.length);
            trainMoveAudioSource.clip = numberClip;
            trainMoveAudioSource.Play();
            callerCoroutine = null;
        }
        
        public void PlayTrainSound()
        {
            
            trainMoveAudioSource.Play();
            UpdateTrainSoundVolume(0f);
        }
    public void UpdateTrainSoundVolume(float v)
        {
            trainMoveAudioSource.volume = v;
    }
    public void StopTrainSound()
        {
      
            trainMoveAudioSource.Stop();
        }
    public void PlayPatternSound(float pitch)
        {
            var source = GetAvailableSource();
            source.pitch = pitch;
            source.gameObject.SetActive(true);
            source.Play();

            StartCoroutine(DisableAfterPlaying(source));
        }

        private IEnumerator DisableAfterPlaying(AudioSource source)
        {
            yield return new WaitWhile(() => source.isPlaying);
            source.gameObject.SetActive(false);
        }

        #region AudioSourcePool
        [SerializeField] private AudioSource audioSourcePrefab;
        [SerializeField] private int initialSize = 5;

        private List<AudioSource> pool = new List<AudioSource>();

       
        private AudioSource AddNewSource()
        {
            if (audioSourcePrefab == null) return null;
     
        var source = Instantiate(audioSourcePrefab, transform);
            source.gameObject.SetActive(false);
            pool.Add(source);
            return source;
        }

        public AudioSource GetAvailableSource()
        {
            foreach (var source in pool)
            {
                if (!source.isPlaying)
                {
                    return source;
                }
            }


            return AddNewSource();
        }
        #endregion
    }
