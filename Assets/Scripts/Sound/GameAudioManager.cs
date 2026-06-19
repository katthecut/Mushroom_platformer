using UnityEngine;
using System.Collections;

namespace Game.Audio
{
    public class GameAudioManager : MonoBehaviour
    {
        public static GameAudioManager Instance { get; private set; }

        //Startup Music
        [SerializeField] private AudioClip startMusic;

        //Audio Sources
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        //volumes
        [Range(0f, 1f)]
        public float musicVolume = 0.6f;

        [Range(0f, 1f)]
        public float sfxVolume = 1f;


        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            SetupSources();
            ApplyVolumes();

            if (startMusic != null)
                PlayMusic(startMusic);
        }


        private void SetupSources()
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.spatialBlend = 0f;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f;
            }
        }

        public void ApplyVolumes()
        {
            if (musicSource)
                musicSource.volume = musicVolume;

            if (sfxSource)
                sfxSource.volume = sfxVolume;
        }


        public void PlayMusic(AudioClip clip)
        {
            if (!clip || !musicSource)
                return;

            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.Play();
        }

        public void StopMusic()
        {
            musicSource?.Stop();
        }

        public void FadeOutMusic(float duration)
        {
            if (musicSource)
                StartCoroutine(FadeMusic(duration));
        }


        private IEnumerator FadeMusic(float duration)
        {
            float start = musicSource.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(
                    start,
                    0f,
                    time / duration
                );

                yield return null;
            }

            musicSource.volume = 0f;
            musicSource.Stop();
        }


        public void PlaySFX(AudioClip clip, float multiplier = 1f)
        {
            if (!clip || !sfxSource)
                return;

            sfxSource.PlayOneShot(
                clip,
                Mathf.Clamp01(sfxVolume * multiplier)
            );
        }


        public void PlaySFXAt(AudioClip clip, Vector3 position, float multiplier = 1f)
        {
            if (!clip)
                return;

            AudioSource temp = new GameObject("TempAudio")
                .AddComponent<AudioSource>();

            temp.transform.position = position;
            temp.clip = clip;
            temp.volume = Mathf.Clamp01(sfxVolume * multiplier);
            temp.spatialBlend = 1f;
            temp.minDistance = 1f;
            temp.maxDistance = 15f;

            temp.Play();

            Destroy(temp.gameObject, clip.length);
        }
    }
}