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

public class TankScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("タンクの最大容量")]
    private float _maxCapacity = 100;
    [SerializeField, Header("インク回復速度")]
    private float _healSpeed = 10;
    private float _nowCapacity = default;

    #endregion
    public float GetNowCapacity { get => _nowCapacity; }
    /// <summary>
    /// 更新前処理
    /// </summary>
    private void Start()
    {
        //タンクの容量初期化
        _nowCapacity = _maxCapacity;
    }
    /// <summary>
    /// タンクの残量を減らす
    /// </summary>
    /// <param name="reduction">減少量</param>
    public void Inkdecrease(float reduction)
    {
        if (_nowCapacity <= 0)
        {
            _nowCapacity = 0;
            return;
        }
        //タンク減少
        _nowCapacity -= reduction;
    }
    /// <summary>
    /// タンクの容量を回復
    /// </summary>
    /// <param name="increment">回復量</param>
    public void InkRecovery(PlayerScript.PlayerStatus playerStatus)
    {
        if (_nowCapacity >= _maxCapacity)
        {
            return;
        }
        switch (playerStatus)
        {
            //インク回復
            case PlayerScript.PlayerStatus.Crouch:
                //タンク回復
                _nowCapacity += _nowCapacity * Time.deltaTime;
                break;
            //歩き状態の移動
            case PlayerScript.PlayerStatus.Idle:
                //タンク回復
                _nowCapacity += _nowCapacity * Time.deltaTime;
                break;
            case PlayerScript.PlayerStatus.Small:
                //タンク回復
                _nowCapacity += _nowCapacity * Time.deltaTime;
                break;
            //壁の潜り状態の移動
            case PlayerScript.PlayerStatus.Diver:
                //タンク回復
                _nowCapacity += _nowCapacity * Time.deltaTime;
                break;
        }
    }
}

