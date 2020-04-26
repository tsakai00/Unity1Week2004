using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// チェックポイントのポップアップ
/// </summary>
public class CheckPointText : MonoBehaviour
{
    [SerializeField] private float _showScale = 0.5f;
    [SerializeField] private float _showSec = 2.0f;
    [SerializeField] private float _dec = 0.9f;
    [SerializeField] private float _damp = 0.25f;
    [SerializeField] private List<string> _randomText = new List<string>();
    [SerializeField] private TextMeshPro _text;

    private float _sclVel = 0.0f;
    private float _scl = 1.0f;
    private float _sec = 2.0f;

    public void Show(Vector2 pos)
    {
        gameObject.SetActive(true);
        _scl = _showScale;
        _sec = _showSec;
        transform.position = new Vector3(pos.x, pos.y, transform.position.z);

        if(_randomText.Count >= 1)
        {
            for(int i = 0; i < 10; i++)
            {
                var str = _randomText[Random.Range(0, _randomText.Count)];
                if(str == _text.text) { continue; }
                _text.text = str;
                break;
            }
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            Show(Vector2.zero);
        }
    }

    void FixedUpdate()
    {
        _sec -= Time.fixedDeltaTime;
        if(_sec < 0.0f) { gameObject.SetActive(false); }

        _sclVel *= _dec;
        _sclVel += (1.0f - _scl) * _damp;
        _scl += _sclVel;

        transform.localScale = new Vector3(_scl, 1.0f / _scl, 1.0f);
    }
}
