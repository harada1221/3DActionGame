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
using UnityEngine.UI;

public class TankScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("タンクの最大容量")]
    private float _maxCapacity = 100;
    [SerializeField, Header("通常時のインク回復速度")]
    private float _healSpeed = 10;
    [SerializeField, Header("潜り時のインク回復速度")]
    private float _healCrouchSpeed = 30;
    [SerializeField, Header("インクの残量表示")]
    private Slider _inkTank = default;
    [SerializeField, Header("")]
    private GameObject _tankObj = default;
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
        //見た目初期化
        _inkTank.value = _maxCapacity;
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
            _inkTank.value = _nowCapacity;
            return;
        }
        //タンク減少
        _nowCapacity -= reduction;
        _inkTank.value = _nowCapacity;
    }
    /// <summary>
    /// タンクの容量を回復
    /// </summary>
    /// <param name="increment">回復量</param>
    public void InkRecovery(PlayerScript.PlayerStatus playerStatus)
    {
        //上限だと処理しない
        if (_nowCapacity >= _maxCapacity)
        {
            _nowCapacity = _maxCapacity;
            _inkTank.value = _nowCapacity;
            return;
        }
        switch (playerStatus)
        {
            //潜り状態
            case PlayerScript.PlayerStatus.Crouch:
                //タンク回復
                _nowCapacity += _healCrouchSpeed * Time.deltaTime;
                _inkTank.gameObject.SetActive(true);
                break;
            //歩き状態の移動
            case PlayerScript.PlayerStatus.Idle:
                //タンク回復
                _nowCapacity += _healSpeed * Time.deltaTime;
                _inkTank.gameObject.SetActive(false);
                break;
            case PlayerScript.PlayerStatus.Small:
                //タンク回復
                _nowCapacity += _healSpeed * Time.deltaTime;
                _inkTank.gameObject.SetActive(false);
                break;
            //壁の潜り状態の移動
            case PlayerScript.PlayerStatus.Diver:
                //タンク回復
                _nowCapacity += _healCrouchSpeed * Time.deltaTime;
                _inkTank.gameObject.SetActive(true);
                break;
        }
        _inkTank.value = _nowCapacity;
    }
}

