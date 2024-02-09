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
    [SerializeField, Header("メインカメラ")]
    private Camera _mainCamera = default;
    [SerializeField]
    private float _jumpTime = 10f;
    private Animator _animator = default;
    private Transform _transform = default;
    private float _timer = default;
    private float _verticalVelocity = default;
    private float _turnVelocity = default;
    private bool isGround = false;
    private bool isJump = false;


    private string _horizontal = "Horizontal";
    private string _vertical = "Vertical";
    private string _jump = "Jump2";

    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        _transform = transform;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Move = Input.GetAxis(_horizontal);
        float Z_Move = Input.GetAxis(_vertical);

        Debug.Log(_horizontal);
        if (X_Move != 0 || Z_Move != 0)
        {
            PlayerMove(X_Move, Z_Move);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }
        if (Input.GetButton("Jump2") || Input.GetKey(KeyCode.Space))
        {
            isJump = true;
            Jump();
        }
        if (Input.GetButtonUp("Jump2")|| Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
        if (_transform.position.y <= 0)
        {
            Debug.Log("着地");
            _timer = 0;
            _transform.position = new Vector3(_transform.position.x, 0, _transform.position.z);
        }
        else if (isJump == false)
        {
            _transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
        }
    }
    /// <summary>
    /// プレイヤーを移動させる
    /// </summary>
    /// <param name="MoveX">Xの移動量</param>
    /// <param name="MoveZ">Zの移動量</param>
    private void PlayerMove(float MoveX, float MoveZ)
    {
        Debug.Log("移動");
        _animator.SetBool("Walking", true);
        //カメラの前方向を取得
        Vector3 comFoward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
        //移動量を計算
        Vector3 pos = comFoward * MoveZ + _mainCamera.transform.right * MoveX;
        //移動させる
        _transform.position += pos * _speed * Time.deltaTime;
    }
    private void Jump()
    {
        if (_timer < _jumpTime)
        {
            _timer += 1f * Time.deltaTime;
            _transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
        }
        else
        {
            isJump = false;
        }

    }
}

