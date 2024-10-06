using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lucky.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace Lucky.Framework.Audio
{
    public class AudioController : MonoBehaviour
    {
        public AudioSource BaseLoopSource => loopSources[0];

        public static AudioController Instance { get; private set; }

        public const float GBC_INTERIOR_BGM_LOWEREDVOLUME = 0.35f; // gbc我猜是global config
        public const float GBC_INTERIOR_BGM_FULLVOLUME = 0.55f;

        [SerializeField] private List<AudioSource> loopSources;

        private List<AudioClip> sfx = new List<AudioClip>();
        private List<AudioClip> loops = new List<AudioClip>();

        private List<AudioSource> ActiveSFXSources
        {
            get
            {
                activeSFX.RemoveAll(x => x == null);
                return activeSFX;
            }
        }

        private List<AudioSource> activeSFX = new List<AudioSource>();

        public bool IsFading { get; set; }

        /// 表示最后一次 音频id + repetitionId 的播放时间戳 
        private DefaultDict<string, float> limitedFrequencySounds = new DefaultDict<string, float>(() => float.NegativeInfinity);

        /// 表示最后一次播放的随机音频对应的#后边的id  
        private DefaultDict<string, string> lastPlayedSounds = new DefaultDict<string, string>(() => "");

        private List<AudioMixer> loadedMixers = new List<AudioMixer>();
        private AudioMixerGroup currentSFXMixer = default;

        private const string SOUNDID_REPEAT_DELIMITER = "#";
        private const float DEFAULT_SPATIAL_BLEND = 0.75f;

        private readonly int[] DEFAULT_LOOPSOURCE_INDICES = { 0 }; // 大部分情况只用到一个loop

        void Awake()
        {
            // 单例
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            // 最高层级
            transform.parent = null;
            DontDestroyOnLoad(gameObject);

            // 加载资源
            Resources.LoadAll<AudioClip>("Audio/SFX").ToList().ForEach(clip => sfx.Add(clip));
            Resources.LoadAll<AudioClip>("Audio/Loops").ToList().ForEach(clip => loops.Add(clip));
        }

        /// 获取第index个循环音源 
        public AudioSource GetLoopSource(int index)
        {
            return loopSources[index];
        }

        /// 把声音通过较为平滑的抛物线映射到[-80, 0]
        private float GetVolumeFromOptions(int volume, int maxVolume)
        {
            float normalizedValue = volume / (float)maxVolume;
            float adjustedValue = Mathf.Pow(normalizedValue, 0.2f);
            return (1f - adjustedValue) * -80f; // 相当于幂函数拉伸平移了一下
        }

        /// <summary>
        /// 播放2D音效
        /// </summary>
        /// <param name="soundId">音频名称</param>
        /// <param name="volume">音量</param>
        /// <param name="skipToTime">播放起点</param>
        /// <param name="pitch">音高</param>
        /// <param name="repetition">重复</param>
        /// <param name="randomization">随机</param>
        /// <param name="distortion">扰动</param>
        /// <param name="looping">循环</param>
        /// <returns></returns>
        public AudioSource PlaySound2D(
            string soundId,
            float volume = 1f,
            float skipToTime = 0f,
            AudioParams.Pitch pitch = null,
            AudioParams.Repetition repetition = null,
            AudioParams.Randomization randomization = null,
            AudioParams.Distortion distortion = null,
            bool looping = false
        )
        {
            AudioSource source = PlaySound3D(soundId, Vector3.zero, volume, skipToTime, pitch, repetition, randomization, distortion, looping);

            if (source != null)
            {
                source.spatialBlend = 0f; // 因为2D，所以声音不随距离衰减
                DontDestroyOnLoad(source.gameObject);
            }

            return source;
        }

        public AudioSource PlaySound3D(
            string soundId,
            Vector3 position,
            float volume = 1f,
            float skipToTime = 0f,
            AudioParams.Pitch pitch = null,
            AudioParams.Repetition repetition = null,
            AudioParams.Randomization randomization = null,
            AudioParams.Distortion distortion = null,
            bool looping = false
        )
        {
            if (string.IsNullOrEmpty(soundId))
            {
                Debug.LogWarning("You are trying to play sound with empty id");
                return null;
            }

            // 如果设置了频率并且播放频率超过了限制
            if (repetition != null && RepetitionIsTooFrequent(soundId, repetition.maxRepetitionFrequency, repetition.entryId))
                return null;

            string randomVariationId = soundId;
            if (randomization != null)
                randomVariationId = GetRandomVariationOfSound(soundId, randomization.isNoRepeating);

            var source = CreateAudioSourceForSound(randomVariationId, position, looping);
            if (source == null)
            {
                Debug.LogWarning("No corresponding audioClip is found");
                return null;
            }

            source.volume = volume;
            source.time = source.clip.length * skipToTime;

            if (pitch != null)
            {
                source.pitch = pitch.pitch;
            }

            if (distortion != null)
            {
                if (distortion.isMuffled)
                {
                    MuffleSource(source);
                }
            }

            activeSFX.Add(source);
            return source;
        }

        public void SetAllSoundsPaused(bool paused)
        {
            ActiveSFXSources.ForEach(
                x =>
                {
                    if (paused)
                        x.Pause();
                    else
                        x.UnPause();
                }
            );
        }

        public void FadeSourceVolume(AudioSource source, float volume, float duration, bool obeyTimescale = true)
        {
            // Tween.Volume(source, volume, duration, 0f, obeyTimescale: obeyTimescale);
        }

        public AudioClip GetLoopClip(string loopId)
        {
            return loops.Find(x => x.name.ToLowerInvariant() == loopId.ToLowerInvariant());
        }

        public AudioClip GetAudioClip(string soundId) => sfx.Find(x => x.name.ToLowerInvariant() == soundId.ToLowerInvariant());

        private AudioSource CreateAudioSourceForSound(string soundId, Vector3 position, bool looping)
        {
            AudioClip sound = GetAudioClip(soundId);

            if (sound != null)
                return InstantiateAudioObject(sound, position, looping);
            return null;
        }

        private AudioSource InstantiateAudioObject(AudioClip clip, Vector3 pos, bool looping)
        {
            GameObject tempGO = new GameObject("Audio_" + clip.name);
            tempGO.transform.position = pos;

            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.clip = clip;
            aSource.loop = looping;
            aSource.outputAudioMixerGroup = currentSFXMixer;
            aSource.spatialBlend = DEFAULT_SPATIAL_BLEND;

            aSource.Play();
            if (!looping)
                Destroy(tempGO, clip.length * 3f); // todo 可用对象池优化，但还要考虑到pause的对象，所以还是有点麻烦

            return aSource;
        }

        /// <summary>
        /// 音频的播放频率是否超过了所限制的频率
        /// </summary>
        /// <param name="soundId">音频id</param>
        /// <param name="frequencyMax">最大频率</param>
        /// <param name="entrySuffix">频率id</param>
        private bool RepetitionIsTooFrequent(string soundId, float frequencyMax, string entrySuffix = "")
        {
            float time = Time.unscaledTime; // 现实时间戳
            string soundKey = soundId + entrySuffix;

            if (time - 1 / frequencyMax > limitedFrequencySounds[soundKey])
            {
                limitedFrequencySounds[soundKey] = time;
                return false;
            }

            return true;
        }

        /// <summary>
        /// 获得对应音频id的随机版本，比如从attack中得到attack#1, attack#3, attack#6
        /// </summary>
        /// <param name="soundPrefix">音频id</param>
        /// <param name="isNoRepeating">是否保证前后随机到不同的音频</param>
        /// <returns></returns>
        private string GetRandomVariationOfSound(string soundPrefix, bool isNoRepeating)
        {
            // 比如我们想播放attack
            // 他就会找到形如attack#1, attack#2, attack#3的clip，#后面必须填从1开始的数字
            List<AudioClip> variations =
                sfx.FindAll(x => x != null && x.name.ToLowerInvariant().StartsWith(soundPrefix.ToLowerInvariant() + SOUNDID_REPEAT_DELIMITER));
            if (variations.Count == 0) // 没找到就结束
                return soundPrefix;
            if (variations.Count == 1 && isNoRepeating)
            {
                Debug.LogWarning("You're trying to play an audioClip without repetition, but there's only one clip found!");
                return soundPrefix;
            }

            // 抽一个
            int index = Random.Range(0, variations.Count);
            if (isNoRepeating)
            {
                while (lastPlayedSounds[soundPrefix] == variations[index].name)
                    index = Random.Range(0, variations.Count);
                lastPlayedSounds[soundPrefix] = variations[index].name;
            }

            return variations[index].name;
        }

        private void MuffleSource(AudioSource source, float cutoff = 300f)
        {
            var filter = source.gameObject.AddComponent<AudioLowPassFilter>();
            filter.cutoffFrequency = cutoff;
        }

        private void UnMuffleSource(AudioSource source)
        {
            var lowPassFilter = source.GetComponent<AudioLowPassFilter>();
            if (lowPassFilter != null)
            {
                Destroy(lowPassFilter);
            }
        }

        public void MuffleLoop(float cutoff, int loopIndex = 0)
        {
            MuffleSource(loopSources[loopIndex], cutoff);
        }

        public void UnMuffleLoop(int loopIndex = 0)
        {
            UnMuffleSource(loopSources[loopIndex]);
        }

        public void SetLoopTimeNormalized(float normalizedTime, int loopIndex = 0)
        {
            if (loopSources[loopIndex].clip != null)
            {
                loopSources[loopIndex].time = Mathf.Clamp(normalizedTime * loopSources[loopIndex].clip.length, 0f, loopSources[loopIndex].clip.length - 0.1f);
            }
        }

        public void SetLoopPaused(bool paused)
        {
            foreach (AudioSource loopSource in loopSources)
            {
                if (paused)
                    loopSource.Pause();
                else
                    loopSource.UnPause();
            }
        }

        public void ResumeLoop(float fadeInSpeed = float.MaxValue)
        {
            foreach (AudioSource loopSource in loopSources)
            {
                loopSource.UnPause();

                if (!loopSource.isPlaying)
                {
                    loopSource.Play();
                }
            }
        }

        public void RestartLoop(int sourceIndex = 0)
        {
            loopSources[sourceIndex].Stop();
            loopSources[sourceIndex].time = 0f;
            loopSources[sourceIndex].volume = 1f;
            loopSources[sourceIndex].pitch = 1f;
            loopSources[sourceIndex].Play();
        }

        public void StopAllLoops()
        {
            CancelFades();
            foreach (AudioSource loopSource in loopSources)
                loopSource.Stop();
        }

        public void StopLoop(int sourceIndex = 0)
        {
            loopSources[sourceIndex].Stop();
        }

        public void SetLoopAndPlay(string loopName, int sourceIndex = 0, bool looping = true, bool cancelFades = true)
        {
            if (cancelFades)
                CancelFades();

            TrySetLoop(loopName, sourceIndex);
            RestartLoop(sourceIndex);

            loopSources[sourceIndex].loop = looping;
        }

        public void CrossFadeLoop(string loopName, float duration, float volume = 1f, float newLoopStartTime = 0f, int sourceIndex = 0,
            float intersection = 0.5f)
        {
            if ((loopSources[sourceIndex].clip == null || loopSources[sourceIndex].clip.name != loopName) && loops.Exists(x => x.name == loopName))
            {
                CancelFades();
                StartCoroutine(CrossFade(loopName, volume, duration, newLoopStartTime, sourceIndex, intersection: intersection));
            }
        }

        public void FadeOutLoop(float fadeDuration, params int[] sourceIndices)
        {
            CancelFades();

            if (sourceIndices == null || sourceIndices.Length == 0)
            {
                sourceIndices = DEFAULT_LOOPSOURCE_INDICES;
            }

            for (int i = 0; i < sourceIndices.Length; i++)
            {
                StartCoroutine(DoFadeToVolume(fadeDuration, 0f, sourceIndices[i]));
            }
        }

        public void FadeInLoop(float fadeDuration, float toVolume, params int[] sourceIndices)
        {
            CancelFades();

            if (sourceIndices == null || sourceIndices.Length == 0)
            {
                sourceIndices = DEFAULT_LOOPSOURCE_INDICES;
            }

            for (int i = 0; i < sourceIndices.Length; i++)
            {
                StartCoroutine(DoFadeToVolume(fadeDuration, toVolume, sourceIndices[i]));
            }
        }

        public void SetLoopVolumeImmediate(float volume, int sourceIndex = 0)
        {
            CancelFades();
            loopSources[sourceIndex].volume = volume;
        }

        public void SetLoopVolume(float volume, float duration, int sourceIndex = 0, bool cancelOtherFades = true)
        {
            if (cancelOtherFades)
            {
                CancelFades();
            }

            StartCoroutine(DoFadeToVolume(duration, volume, sourceIndex));
        }

        private void CancelFades()
        {
            StopAllCoroutines();
            foreach (AudioSource loopSource in loopSources)
                // Tween.Cancel(loopSource.GetInstanceID());

                IsFading = false;
        }

        private void TrySetLoop(string loopName, int sourceIndex = 0)
        {
            AudioClip loop = GetLoop(loopName);

            if (loop != null)
            {
                loopSources[sourceIndex].clip = loop;
                loopSources[sourceIndex].pitch = 1f;
            }
        }

        private AudioClip GetLoop(string loopName)
        {
            return loops.Find(x => x.name == loopName);
        }

        private IEnumerator DoFadeToVolume(float duration, float volume, int sourceIndex = 0)
        {
            IsFading = true;

            // Tween.Volume(loopSources[sourceIndex], volume, duration, 0f, Tween.EaseInOut);
            yield return new WaitForSeconds(duration);

            IsFading = false;
        }

        /// <summary>
        /// 当然这么写的话同时fade多个loop会有问题，不过一般不会触发 
        /// </summary>
        /// <param name="newLoop">fadeIn的loopId</param>
        /// <param name="volume">音量</param>
        /// <param name="duration">fade时长</param>
        /// <param name="newLoopStartTimeNormalized">新loop开始播放的位置</param>
        /// <param name="sourceIndex">选择哪个audioSource播放</param>
        /// <param name="intersection">crossFade中重叠的部分</param>
        /// <returns></returns>
        private IEnumerator CrossFade(string newLoop, float volume, float duration, float newLoopStartTimeNormalized, int sourceIndex = 0,
            float intersection = 0.5f)
        {
            intersection = Mathf.Clamp(intersection, 0, 1);
            float wait = (1 - intersection) * duration / 2;
            if (loopSources[sourceIndex].clip != null && loopSources[sourceIndex].isPlaying)
                StartCoroutine(DoFadeToVolume(duration - wait, 0f, 1)); // HACK: also fade out 2nd loop source here

            yield return new WaitForSeconds(wait);
            int helperId = loopSources.Count - 1;
            TrySetLoop(newLoop, helperId);
            loopSources[helperId].time = 0f;
            loopSources[helperId].Play();
            SetLoopTimeNormalized(newLoopStartTimeNormalized, helperId);
            StartCoroutine(DoFadeToVolume(duration - wait, volume, helperId));
            yield return new WaitForSeconds(duration - wait);

            (loopSources[sourceIndex], loopSources[helperId]) = (loopSources[helperId], loopSources[sourceIndex]);
        }
    }
}