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

public class ballisticScript : MonoBehaviour
{
    #region 変数宣言
    //[SerializeField, Header("発射位置")]
    //private Transform _firstPosition = default;
    //[SerializeField, Header("発射方向")]
    //private Vector3 _direcion = default;
    //[SerializeField, Header("発射距離")]
    //private float _distance = default;
    //[SerializeField, Header("打つ球")]
    //private GameObject _ball = default;
    //[SerializeField, Header("カーブの緩やかさ")]
    //private float _fallCurveVolume = 1f;
    //[SerializeField, Header("カーブの細かさ")]
    //private float _fallCurveDelta = 0.1f;
    //private Vector3 _targetPosition = default;
    ////カーブ終了後の落下距離
    //private float _fallDistance = default;
    ////下方向の向き
    //private Vector3 _fallVector = default;
    ////合計の長さ
    //private float _sumMagnitude = default;
    #endregion
    //public Vector3 GetTargetPosition { get => _targetPosition; }
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //_fallVector = Vector3.down;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Shot();
        }
    }
    /// <summary>
    /// 球を打つ
    /// </summary>
    public void Shot()
    {
        //GameObject ball = Instantiate(_ball, _firstPosition.transform.position, Quaternion.identity);
        //ball.transform.parent = this.transform;
        //ball.transform.forward = transform.forward;
        //ball.transform.eulerAngles = new Vector3(ball.transform.eulerAngles.x, ball.transform.eulerAngles.y, ball.transform.eulerAngles.z);
        //_direcion = transform.forward;
        ////到着位置
        //_targetPosition = _direcion * _distance;
    }
}

