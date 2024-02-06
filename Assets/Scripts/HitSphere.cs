using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitSphere : MonoBehaviour
{
    private float _radius = default;
    private float _targetradius = default;
    [SerializeField]
    private GameObject _targetObj = default;
    private void Start()
    {
        _radius = this.gameObject.transform.localScale.x / 2;
        _targetradius = _targetObj.transform.localScale.x / 2;
    }
    private void Update()
    {
        if (Vector3.Distance(_targetObj.transform.position, this.gameObject.transform.position) < _radius + _targetradius)
        {
            Debug.Log("hit");
        }
        else
        {
            Debug.Log("hothit");
        }
    }
}
