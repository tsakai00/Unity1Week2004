using Lib.Util;
using UnityEngine;
using UnityEngine.Audio;

namespace Lib.Sound
{
    /// <summary>
    /// サウンド再生管理
    /// </summary>
    public partial class SoundManager : SingletonBehaviour<SoundManager>
    {
        [SerializeField] private AudioMixerGroup    _mixerGroupMaster;
        [SerializeField] private SoundPlayer        _soundPlayerSE;
        [SerializeField] private SoundPlayer        _soundPlayerBGM;
        [SerializeField] private SoundPlayer        _soundPlayerJingle;

        /// <summary>
        /// 初期化
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            _soundPlayerSE.Init();
            _soundPlayerBGM.Init();
            _soundPlayerJingle.Init();
        }

        /// <summary>
        /// SEを再生
        /// </summary>
        /// <param name="key"></param>
        public void PlaySE(string key)
        {
            _soundPlayerSE.Play(key);
        }

        /// <summary>
        /// BGMを再生
        /// </summary>
        /// <param name="key"></param>
        public void PlayBGM(string key)
        {
            _soundPlayerBGM.PlayCrossFade(key);
        }

        public void PlayJingle(string key)
        {
            _soundPlayerJingle.Play(key);
        }

        public void StopBGM(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerBGM.Stop(fadeSec);
        }

        /// <summary>
        /// すべて停止
        /// </summary>
        public void StopAll(float fadeSec = SoundPlayer.FADE_SEC)
        {
            _soundPlayerSE.Stop(fadeSec);
            _soundPlayerBGM.Stop(fadeSec);
            _soundPlayerJingle.Stop(fadeSec);
        }

        /// <summary>
        /// マスターのボリュームを設定
        /// </summary>
        public void SetVolumeMaster(float volume)
        {
            _mixerGroupMaster.audioMixer.SetVolume(_mixerGroupMaster.name, volume);
        }

        /// <summary>
        /// マスターのボリュームを取得
        /// </summary>
        public float GetVolumeMaster()
        {
            return _mixerGroupMaster.audioMixer.GetVolume(_mixerGroupMaster.name);
        }
    }
}
