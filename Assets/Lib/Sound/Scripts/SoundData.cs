using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using Lib.Util;
using System.ComponentModel;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Lib.Sound
{
    /// <summary>
    /// AudioClipの纏まり
    /// </summary>
    [CreateAssetMenu(menuName = "Lib/Sound/SoundData")]
    public class SoundData : ScriptableObject
    {
        private const string ENUM_FOLDER = "Lib/Sound/Prefabs";

        /// <summary>
        /// AudioClip情報
        /// </summary>
        [Serializable]
        public class AudioClipInfo
        {
            public string       key;
            public AudioClip    clip;
        }

        [SerializeField, HideInInspector] private string              _sourceFolder;
        [SerializeField, HideInInspector] private List<AudioClipInfo> _audioClipInfoList = new List<AudioClipInfo>();

        /// <summary>
        /// AudioClipを取得
        /// </summary>
        public AudioClip GetAudioClip(string key)
        {
            // とりあえず総当り
            foreach(var info in _audioClipInfoList)
            {
                if(info.key == key) { return info.clip; }
            }

            return null;
        }

        #if UNITY_EDITOR
        /// <summary>
        /// AudioClipの格納先フォルダ以下を検索して、AudioClip情報を自動入力
        /// </summary>
        [CustomEditor(typeof(SoundData))]
        public class SoundDataEditor : Editor
        {
            public override void OnInspectorGUI()
            {
                SoundData data = target as SoundData;
                List<AudioClipInfo> list = data._audioClipInfoList;

                DrawDefaultInspector();

                EditorGUILayout.BeginVertical();

                // 元フォルダ
                EditorGUILayout.BeginHorizontal();
                data._sourceFolder = EditorGUILayout.TextField("Source Folder", data._sourceFolder);
                if(GUILayout.Button("Folder"))
                {
                    var folderPath = EditorUtility.OpenFolderPanel("Source Folder", "Assets", "");
                    if(!string.IsNullOrEmpty(folderPath))
                    {
                        data._sourceFolder = folderPath.Remove(0, folderPath.LastIndexOf("Assets"));
                        EditorUtility.SetDirty(target);
                        AssetDatabase.SaveAssets();
                    }
                }
                EditorGUILayout.EndHorizontal();

                // リスト自動入力
                GUILayout.Label("キー名を変更したときもこのボタンを押してね");
                if(GUILayout.Button("Update"))
                {
                    data.UpdateSoundData();
                    data.UpdateSoundDataEnum(this.GetInstanceID());
                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                }

                // AudioClipのリスト
                GUI.enabled = false;
                int count = EditorGUILayout.DelayedIntField("Size", list.Count);
                GUI.enabled = true;
                foreach(var info in list)
                {
                    EditorGUILayout.BeginHorizontal();
                    info.key = EditorGUILayout.TextField(info.key);
                    info.clip = EditorGUILayout.ObjectField(info.clip, typeof(AudioClip), true) as AudioClip;
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.EndVertical();
            }
        }

        /// <summary>
        /// SoundDataを更新
        /// </summary>
        private void UpdateSoundData()
        {
            string sourceFolder = _sourceFolder.Replace("Assets/", "");
            string[] pathList = Directory.GetFiles(Path.Combine(Application.dataPath, sourceFolder));

            List<AudioClipInfo> clipInfoList = new List<AudioClipInfo>();
            foreach(var path in pathList)
            {
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(path.Remove(0, path.LastIndexOf("Assets")));
                if(clip == null) { continue; }

                var clipInfo = new AudioClipInfo();
                clipInfo.clip = clip;
                clipInfo.key = _audioClipInfoList.Where(x => x.clip == clip).FirstOrDefault()?.key;
                if(string.IsNullOrEmpty(clipInfo.key))
                {
                    clipInfo.key = clip.name.ToUpperSnake();    // キー名が空なら、デフォルトでファイル名を入れておく
                }
                clipInfoList.Add(clipInfo);
            }

            _audioClipInfoList = clipInfoList;
        }

        /// <summary>
        /// SoundDataのキーを列挙
        /// </summary>
        private void UpdateSoundDataEnum(int instanceID)
        {
            string path = ENUM_FOLDER;
            path = Path.Combine(Application.dataPath, path);
            path = Path.Combine(path, $"{name}Path.cs");

            using(var sw = File.CreateText(path))
            {
                sw.WriteLine($"public static class {name}Path");
                sw.WriteLine("{");
                foreach(var info in _audioClipInfoList)
                {
                    sw.WriteIndentLine($"public const string _{info.key.Remove(0, info.key.IndexOf("/") + 1)} = \"{info.key}\";", 1);
                }
                sw.WriteLine("}");
            }

            AssetDatabase.Refresh();
        }
        #endif
    }
}
