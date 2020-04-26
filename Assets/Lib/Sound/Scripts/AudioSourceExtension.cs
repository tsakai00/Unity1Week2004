using System;
using System.Collections;
using UnityEngine;

namespace Lib.Sound
{
    /// <summary>
    /// AudioClipの拡張メソッド
    /// </summary>
    public static class AudioSourceExtension
    {
        /// <summary>
        /// サウンド再生
        /// </summary>
        public static void Play(this AudioSource self, AudioClip clip, float volume, float fadeSec, bool isLoop, MonoBehaviour mono)
        {
            if(clip == null || volume <= 0) { return; }
            
            self.clip = clip;
            self.loop = isLoop;
            mono.StartCoroutine(Fade(self, 0.0f, volume, fadeSec));
            self.Play();
        }

        /// <summary>
        /// サウンド停止
        /// </summary>
        public static void Stop(this AudioSource self, float fadeSec, MonoBehaviour mono)
        {
            if(self.isPlaying == false) { return; }

            mono.StartCoroutine(Fade(self, self.volume, 0.0f, fadeSec));
        }

        /// <summary>
        /// サウンドをフェード
        /// </summary>
        private static IEnumerator Fade(AudioSource src, float a, float b, float fadeSec)
        {
            if(fadeSec <= 0.0f)
            {
                src.volume = b;
            }
            else
            {
                float inv = 1.0f / fadeSec;
                float sec = Time.time;
                while(true)
                {
                    float t = (Time.time - sec) * inv;
                    src.volume = Mathf.Lerp(a, b, Mathf.Clamp01(t));
                    
                    if(t >= 1.0f) { break; }
                    yield return null;
                }
            }

            if(b <= 0.0f)
            {
                // ボリュームをゼロにする場合はサウンド停止
                src.Stop();
            }
        }
    }
}
