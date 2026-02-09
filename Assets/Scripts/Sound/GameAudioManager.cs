using UnityEngine;

namespace Game.Audio
{
    public class GameAudioManager : MonoBehaviour
    {
        public static GameAudioManager Instance { get; private set; }

        //audio Sources
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        //volumes
        [Range(0f, 1f)] public float musicVolume = 0.6f;
        [Range(0f, 1f)] public float sfxVolume = 1f;

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
        }

        private void SetupSources()
        {
            //music source
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
                musicSource.spatialBlend = 0f; //2D
            }

            // SFX source
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
                sfxSource.spatialBlend = 0f; //2D
            }
        }

        public void ApplyVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume;

            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
        }

        //music
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null || musicSource == null)
                return;

            if (musicSource.clip == clip && musicSource.isPlaying)
                return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
        }

        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        public void FadeOutMusic(float duration)
        {
            if (musicSource == null) return;
            StartCoroutine(FadeMusic(0f, duration));
        }

        private System.Collections.IEnumerator FadeMusic(float target, float duration)
        {
            float start = musicSource.volume;
            float time = 0f;

            while (time < duration)
            {
                time += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(start, target, time / duration);
                yield return null;
            }

            musicSource.volume = target;
        }

        //SFX (2D)

        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null || sfxSource == null)
                return;

            sfxSource.PlayOneShot(clip, Mathf.Clamp01(sfxVolume * volumeMultiplier));
        }

        //SFX (3D/world position)

        public void PlaySFXAt(AudioClip clip, Vector3 worldPosition, float volumeMultiplier = 1f)
        {
            if (clip == null)
                return;

            AudioSource tempSource = new GameObject("TempAudio").AddComponent<AudioSource>();
            tempSource.transform.position = worldPosition;

            tempSource.clip = clip;
            tempSource.volume = Mathf.Clamp01(sfxVolume * volumeMultiplier);
            tempSource.spatialBlend = 1f; //3D
            tempSource.minDistance = 1f;
            tempSource.maxDistance = 15f;
            tempSource.Play();

            Destroy(tempSource.gameObject, clip.length);
        }
    }
}