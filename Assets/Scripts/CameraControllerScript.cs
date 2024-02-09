/*
*　　説明　
*　　日付
*
*
*
*　　原田　智大
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControllerScript : MonoBehaviour
{
    [SerializeField, Header("キャラクター")]
    private Transform _character = default;
    [SerializeField, Header("キャラクターの中心")]
    private Transform _pivot = default;
    [SerializeField, Header("カメラの移動スピード")]
    private float _speed = 1f;
    //カメラ上下移動の最大、最小角度
    [Range(-0.999f, -0.5f)]
    public float maxYAngle = -0.5f;
    [Range(0.5f, 0.999f)]
    public float minYAngle = 0.5f;
    private string _horizontal = "Horizontal2";
    private string _vertical = "Vertical2";

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //null処理
        if (_character == null)
        {
            _character = transform.parent;
        }
        if (_pivot == null)
        {
            _pivot = transform;
        }
    }
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Rotation = Input.GetAxis(_horizontal);
        float Y_Rotation = Input.GetAxis(_vertical);
        //Y軸を更新、取得したX軸の変更をキャラクターのY軸に反映
        _character.transform.Rotate(0, X_Rotation * _speed * Time.deltaTime, 0);

        //X軸の設定
        float nowAngle = _pivot.transform.localRotation.x;
        //最大値、または最小値を超えた場合、カメラをそれ以上動かないように
        if (-Y_Rotation != 0)
        {
            if (0 < Y_Rotation)
            {
                if (minYAngle <= nowAngle)
                {
                    _pivot.transform.Rotate(-Y_Rotation * _speed * Time.deltaTime, 0, 0);
                }
            }
            else
            {
                if (nowAngle <= maxYAngle)
                {
                    _pivot.transform.Rotate(-Y_Rotation * _speed* Time.deltaTime, 0, 0);
                }
            }
        }
    }
}

