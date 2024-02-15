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
    [SerializeField,Header("ジャンプの時間")]
    private float _jumpTime = 10f;
   
    //銃スクリプト
    private GunScript _gunScript = default;
    //プレイヤーのアニメータ
    private Animator _animator = default;
    //自分のポジション
    private Transform _transform = default;
    //タイマーカウント
    private float _timer = default;
    //ジャンプフラグ
    private bool isJump = false;
    //射撃フラグ
    private bool isShoot = false;
    //プレイヤーのステート
    private PlayerStatus _playerStatus = PlayerStatus.Idle;

    //入力の名前
    private string _horizontal = "Horizontal";
    private string _vertical = "Vertical";
    private string _jump = "Jump2";
    private string _shot = "RTrigger";
    #endregion
    private enum PlayerStatus
    {
        Idle,
        Walk,
        Shot
    }
    #region プロパティ
    public bool GetShoot { get => isShoot; }
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //自分のポジション
        _transform = transform;
        //アニメーター取得
        _animator = GetComponent<Animator>();
        //銃のスクリプト取得
        _gunScript = GetComponent<GunScript>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //スティックのX,Y軸がどれほど移動したか
        float X_Move = Input.GetAxisRaw(_horizontal);
        float Z_Move = Input.GetAxisRaw(_vertical);
        //コントローラーのRトリガー
        float R_Trigger = Input.GetAxisRaw(_shot);

        //移動
        if (X_Move != 0 || Z_Move != 0)
        {
            PlayerMove(X_Move, Z_Move);
        }
        else
        {
            _animator.SetBool("Walking", false);
        }
        //ジャンプ
        if (Input.GetButton(_jump) || Input.GetKey(KeyCode.Space))
        {
            isJump = true;
            Jump();
        }
        if (Input.GetButtonUp(_jump) || Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
        //着地
        if (_transform.position.y <= 0)
        {
            //タイマーリセット
            _timer = 0;
            _transform.position = new Vector3(_transform.position.x, 0, _transform.position.z);
        }
        else if (isJump == false)
        {
            //下向きに移動
            _transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
        }
        //射撃
        if (R_Trigger != 0 || Input.GetKey(KeyCode.E))
        {
            //カメラの向き調整
            CameraRevolution();
            //射撃
            _gunScript.Ballistic();
            //ステート変更
            isShoot = true;
            _playerStatus = PlayerStatus.Shot;
        }
        else
        {
            //射撃終了
            isShoot = false;
            _playerStatus = PlayerStatus.Idle;
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
        if (moveDirection != Vector3.zero && _playerStatus != PlayerStatus.Shot)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            _transform.rotation = newRotation;
        }

        //移動させる
        _transform.position += moveDirection * _speed * Time.deltaTime;
        _mainCamera.transform.position += moveDirection * _speed * Time.deltaTime;
    }
    /// <summary>
    /// ジャンプさせる
    /// </summary>
    private void Jump()
    {
        //ジャンプ可能か
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
    /// <summary>
    /// プレイヤーの向き調整
    /// </summary>
    private void CameraRevolution()
    {
        //1回目の射撃か
        if (isShoot == false)
        {
            //カメラの向きに合わせて回転
            Vector3 playerRotation = new Vector3(_transform.rotation.x, _mainCamera.transform.eulerAngles.y, _transform.rotation.z);
            _transform.rotation = Quaternion.Euler(playerRotation);
        }
    }
}

