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
    private CameraScript _cameraScript = default;
    private Animator _animator = default;
    private Transform _transform = default;
    private float _timer = default;
    private float _verticalVelocity = default;
    private float _turnVelocity = default;
    private bool isGround = false;
    private bool isJump = false;
    // 前フレームのワールド位置
    private Vector3 _prevPosition;


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
        _cameraScript = _mainCamera.GetComponent<CameraScript>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Move = Input.GetAxisRaw(_horizontal);
        float Z_Move = Input.GetAxisRaw(_vertical);

        if (X_Move != 0 || Z_Move != 0)
        {
            PlayerMove(X_Move, Z_Move);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }
        if (Input.GetButton(_jump) || Input.GetKey(KeyCode.Space))
        {
            isJump = true;
            Jump();
        }
        if (Input.GetButtonUp(_jump) || Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
        if (_transform.position.y <= 0)
        {
            //Debug.Log("着地");
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
        _animator.SetBool("Walking", true);
        //カメラの前方向を取得
        Vector3 comForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;

        //移動量を計算
        //カメラの方向
        Vector3 cameraRight = _mainCamera.transform.right;
        //y座標を消す
        cameraRight.y -= _mainCamera.transform.right.y; 
        Vector3 moveDirection = comForward * MoveZ + cameraRight * MoveX;
        moveDirection = moveDirection.normalized;
        //プレイヤーを移動方向に向かせる
        if (moveDirection != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            _transform.rotation = newRotation;
        }

        //移動させる
        _transform.position += moveDirection * _speed * Time.deltaTime;
        _mainCamera.transform.position += moveDirection * _speed * Time.deltaTime;
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

