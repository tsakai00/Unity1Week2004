using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Lib.Sound
{
    /// <summary>
    /// サウンド再生
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        public const float FADE_SEC = 0.5f;

        /// <summary>
        /// AudioMixerGroupの設定
        /// </summary>
        [Serializable]
        public class Setting
        {
            public AudioMixerGroup  mixerGroup;
            public int              sourceNum;   // 同時再生数
            public SoundData        soundData;
        }

        [SerializeField] private Setting    _setting;
        private List<AudioSource>           _sourceList;

        /// <summary>
        /// 初期化
        /// </summary>
        public void Init()
        {
            _sourceList = new List<AudioSource>(_setting.sourceNum);

            gameObject.name = _setting.mixerGroup.name;
            for(int i = 0; i < _setting.sourceNum; i++)
            {
                var src = gameObject.AddComponent<AudioSource>();
                src.outputAudioMixerGroup = _setting.mixerGroup;
                src.playOnAwake = false;
                _sourceList.Add(src);
            }
        }

        /// <summary>
        /// 再生
        /// </summary>
        public void Play(string key, float fadeSec = 0.0f, bool isLoop = false)
        {
            var clip = GetAudioClip(key);
            var src  = GetAudioSource();
            src?.Play(clip, 1.0f, fadeSec, isLoop, this);
        }

        /// <summary>
        /// クロスフェード再生
        /// </summary>
        public void PlayCrossFade(string key, float fadeSec = FADE_SEC, bool isLoop = true)
        {
            if(IsPlaying(key)) { return; }   // すでに再生中

            if(IsPlaying())
            {
                // 他を再生中ならクロスフェード
                Stop(fadeSec);
                Play(key, fadeSec, isLoop);
            }
            else
            {
                // 通常再生
                Play(key, 0.0f, isLoop);
            }
        }

        /// <summary>
        /// 停止
        /// </summary>
        public void Stop(float fadeSec = FADE_SEC)
        {
            foreach(var src in _sourceList)
            {
                src.Stop(fadeSec, this);
            }
        }

        /// <summary>
        /// ボリュームを設定
        /// </summary>
        public void SetVolume(float volume)
        {
            _setting.mixerGroup.audioMixer.SetVolume(_setting.mixerGroup.name, volume);
        }

        /// <summary>
        /// ボリュームを取得
        /// </summary>
        public float GetVolume()
        {
            return _setting.mixerGroup.audioMixer.GetVolume(_setting.mixerGroup.name);
        }

        /// <summary>
        /// 再生中か
        /// </summary>
        public bool IsPlaying()
        {
            foreach(var src in _sourceList)
            {
                if(src.isPlaying) { return true; }
            }

            return false;
        }

        /// <summary>
        /// 再生中か
        /// </summary>
        public bool IsPlaying(string key)
        {
            var clip = GetAudioClip(key);

            foreach(var src in _sourceList)
            {
                if(src.isPlaying == false)  { continue; }
                if(src.clip != clip)        { continue; }
                return true;
            }

            return false;
        }

        /// <summary>
        /// 再生に使用できるAudioSourceを返す
        /// </summary>
        private AudioSource GetAudioSource()
        {
            foreach(var src in _sourceList)
            {
                if(src.isPlaying == false) { return src; }
            }

            return null;
        }

        /// <summary>
        /// AudioClipを返す
        /// </summary>
        private AudioClip GetAudioClip(string key)
        {
            return _setting.soundData.GetAudioClip(key);
        }
    }
}
