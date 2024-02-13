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

public class CameraScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("プレイヤー")]
    private GameObject _player = default;
    [SerializeField, Header("移動スピード")]
    private float _speed = default;
    private Vector3 _offset;
    private string _horizontal = "Horizontal2";
    private string _vertical = "Vertical2";
    #endregion
    private void Start()
    {
        //MainCameraとplayerとの相対距離を求める
        _offset = transform.position - _player.transform.position;

    }
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Rotation = Input.GetAxisRaw(_horizontal);
        float Y_Rotation = Input.GetAxisRaw(_vertical);
        //X方向に一定量移動していれば横回転
        if (Mathf.Abs(X_Rotation) > 0.001f)
        {
            //回転軸はワールド座標のY軸
            transform.RotateAround(_player.transform.position, Vector3.up, X_Rotation * Time.deltaTime * _speed);
        }
        if (Mathf.Abs(Y_Rotation) > 0.001f)
        {
            //回転軸はカメラ自身のX軸
            transform.RotateAround(_player.transform.position, transform.right, Y_Rotation * Time.deltaTime * _speed);
        }
    }
   
}


