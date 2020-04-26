using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Lib.Sound
{
    /// <summary>
    /// オーディオミキサーの拡張メソッド
    /// </summary>
    public static class AudioMixerExtension
    {
        /// <summary>
        /// ボリュームを設定
        /// </summary>
        public static void SetVolume(this AudioMixer self, string name, float volume)
        {
            volume = Mathf.Clamp(volume, 0.0001f, 1.0f);
            float decibel =  Mathf.Log10(volume) * 20.0f;
            self.SetFloat(name, decibel);
        }

        /// <summary>
        /// ボリュームを取得
        /// </summary>
        public static float GetVolume(this AudioMixer self, string name)
        {
            float decibel;
            self.GetFloat(name, out decibel);
            float volume = Mathf.Pow(10.0f, decibel / 20.0f);
            return Mathf.Clamp(volume, 0.0001f, 1.0f);
        }
    }
}
