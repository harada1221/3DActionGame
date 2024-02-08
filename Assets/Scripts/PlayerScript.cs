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

public class PlayerScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("移動スピード")]
    private float _speed = 3;
    [SerializeField, Header("ジャンプスピード")]
    private float _jumpSpeed = 7;
    [SerializeField, Header("重力加速度")]
    private float _gravity = -9.81f;
    [SerializeField, Header("落下時の速度制限")]
    private float _fallSpeed = 10f;
    [SerializeField, Header("落下の初速")]
    private float _fallfirstSpeed = 2;
    private Transform _transform = default;
    private float _verticalVelocity = default;
    private float _turnVelocity = default;
    private bool isGround = false;

    private string _horizontal = "Horizontal";
    private string _vertical = "Vertical";

    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        _transform = transform;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Move = Input.GetAxis(_horizontal);
        float Y_Move = Input.GetAxis(_vertical);

        if (isGround == false && (X_Move != 0 || Y_Move != 0))
        {
            PlayerMove(X_Move, Y_Move);
        }
    }

    private void PlayerMove(float MoveX,float MoveY)
    {
       
    }

    private void Jump()
    {
        // 鉛直上向きに速度を与える
        _verticalVelocity = _jumpSpeed;
    }
}

