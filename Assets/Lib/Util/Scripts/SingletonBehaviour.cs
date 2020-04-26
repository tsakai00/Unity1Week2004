using UnityEngine;

namespace Lib.Util
{
    /// <summary>
    /// シングルトン
    /// </summary>
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        private static T instance;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = FindObjectOfType<T>();
                }
                return instance;
            }
        }

        protected virtual void Awake()
        {
            if(Instance == this)
            {
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
