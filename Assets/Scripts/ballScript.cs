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
    private Vector3 _shootVelocity = default;
    private GameObject _player = default;
    private GunScript _gunScript = default;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //スクリプト取得
        _player = GameObject.FindWithTag("Player");
        _gunScript = _player.GetComponent<GunScript>();
        //非表示
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //毎フレーム弾を移動させる
        transform.position += _shootVelocity * _speed * Time.deltaTime;
        if (_gunScript.GetPower <= Vector3.Distance(transform.position, _player.transform.position))
        {
            //弾回収
            HideFromStage();
        }
    }
    /// <summary>
    /// 弾の方向を設定
    /// </summary>
    /// <param name="vel">飛ばす方向</param>
    public void SetVelocity(Vector3 vel)
    {
        _shootVelocity = vel;
    }
    /// <summary>
    /// 弾を回収する
    /// </summary>
    public void HideFromStage()
    {
        //オブジェクトプールのCollect関数を呼び出し自身を回収
        _gunScript.BallCollect(this);
        Debug.Log("回収");
    }
}

