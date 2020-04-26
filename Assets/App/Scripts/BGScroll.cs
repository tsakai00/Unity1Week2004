using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScroll : MonoBehaviour
{
    private const float MAP_SIZE = 50.0f;

    [SerializeField] private Camera _cam;
    [SerializeField] private float  _scrollScale = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = transform.position;
        pos.x = _cam.transform.position.x * _scrollScale;
        pos.x += Mathf.FloorToInt(Mathf.Max(_cam.transform.position.x - pos.x, 0.0f) / MAP_SIZE) * MAP_SIZE;
        transform.position = pos;
    }
}
