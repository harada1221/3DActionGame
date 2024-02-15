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

public class BallScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("スピード")]
    private float _speed = default;
    [SerializeField, Header("落下スピード")]
    private float _foolSpeed = default;
    private Vector3 _shootVelocity = default;
    private GameObject _player = default;
    private Vector3 _nowShotPosition = default;
    private GunScript _gunScript = default;
    private bool isAngle = default;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //プレイヤー取得
        _player = GameObject.FindWithTag("Player");
        //銃のスクリプト
        _gunScript = _player.GetComponent<GunScript>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        if (_gunScript.GetPower >= Vector3.Distance(transform.position, _nowShotPosition) && isAngle == false)
        {
            //毎フレーム弾を移動させる
            transform.position += _shootVelocity * _speed * Time.deltaTime;
        }
        //射程距離計算
        else
        {
            //弾を落下
            FoolMove();
        }
        //着地判定
        if (transform.position.y <= 0)
        {
            //弾回収
            HideFromStage();
        }
    }
    /// <summary>
    /// 弾の方向を設定
    /// </summary>
    /// <param name="shotDirections">飛ばす方向</param>
    /// <param name="shotPosition">発射位置</param>
    public void SetVelocity(Vector3 shotDirections, Vector3 shotPosition)
    {
        isAngle = false;
        //向き設定
        _shootVelocity = shotDirections.normalized;
        //発射位置
        _nowShotPosition = shotPosition;
    }
    /// <summary>
    /// 弾を回収する
    /// </summary>
    public void HideFromStage()
    {
        //オブジェクトプールのCollect関数を呼び出し自身を回収
        _gunScript.BallCollect(this);
    }
    /// <summary>
    /// 落下させる
    /// </summary>
    private void FoolMove()
    {
        isAngle = true;
        //下に落とす
        _shootVelocity += Vector3.down * Time.deltaTime * _foolSpeed;
        Debug.Log(_shootVelocity);
        transform.position += _shootVelocity * Time.deltaTime;
    }
}

