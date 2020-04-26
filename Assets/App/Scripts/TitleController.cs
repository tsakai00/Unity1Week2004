using System.Collections;
using System.Collections.Generic;
using Lib.Sound;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.StopAll(2.0f);
    }

    // Update is called once per frame
    public void OnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
