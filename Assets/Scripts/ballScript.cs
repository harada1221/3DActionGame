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
    [SerializeField, Header("当たり判定の半径")]
    private float _radius = 0.2f;
    [SerializeField, Header("インク")]
    private GameObject _ink = default;
    //射撃の向き
    private Vector3 _shootVelocity = default;
    //射撃するプレイヤー
    private GameObject _player = default;
    //射撃位置
    private Vector3 _nowShotPosition = default;
    //銃のスクリプト
    private GunScript _gunScript = default;
    //射程の最高地点に到達したか
    private bool isAngle = default;

    private CalcUVScript _calcUV = default;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //プレイヤー取得
        _player = GameObject.FindWithTag("Player");
        _calcUV = GameObject.FindWithTag("MainCamera").GetComponent<CalcUVScript>();
        //銃のスクリプト
        _gunScript = _player.GetComponent<GunScript>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //射程範囲内か
        if (_gunScript.GetPower >= Vector3.Distance(transform.position, _nowShotPosition) && isAngle == false)
        {
            //毎フレーム弾を移動させる
            transform.position += _shootVelocity * _speed * Time.deltaTime;
        }
        //射程距離範囲外
        else
        {
            //弾を落下
            FoolMove();
        }
        RaycastHit hit;
        if (Physics.Raycast(transform.position, _shootVelocity, out hit, _radius, LayerMask.GetMask("floor")) /*|| transform.position.y < 0*/)
        {
            Debug.Log(hit.transform.name);
            Paint(hit);
            //弾回収
            HideFromStage();
        }
        //0以下だと回収
        if (transform.position.y < 0)
        {
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
        transform.position += _shootVelocity * Time.deltaTime;
    }
   private void Paint(RaycastHit hit)
    {
        GameObject decal = Instantiate(_ink, hit.point, Quaternion.identity);
        decal.transform.forward = hit.normal; // デカールを法線方向に向ける
    }
}

