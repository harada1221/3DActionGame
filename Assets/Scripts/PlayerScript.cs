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
    [SerializeField, Header("rayの長さ")]
    private float _rayDistance = 1f;
    [SerializeField, Header("落下時の速度制限")]
    private float _fallSpeed = 10f;
    [SerializeField, Header("落下の初速")]
    private float _fallfirstSpeed = 2;
    [SerializeField, Header("メインカメラ")]
    private Camera _mainCamera = default;
    [SerializeField, Header("ジャンプの時間")]
    private float _jumpTime = 10f;
    [SerializeField, Header("潜り状態の移動スピード")]
    private float _crouchSpeed = 5f;
    [SerializeField, Header("潜り状態の最高スピード")]
    private float _maxSpeed = 1.0f;

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
    //
    private bool isMyColor = false;
    //プレイヤーのステート
    private PlayerStatus _playerStatus = PlayerStatus.Idle;
    //プレイヤーの移動方向
    private Vector3 _playerMoveDirection = default;

    //入力の名前
    private string _horizontal = "Horizontal";
    private string _vertical = "Vertical";
    private string _jump = "Jump2";
    private string _shot = "RTrigger";
    private string _crouch = "LTrigger";
    #endregion
    private enum PlayerStatus
    {
        Idle,
        Walk,
        Shot,
        Crouch
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
        //コントローラーのR.Lトリガー
        float R_Trigger = Input.GetAxisRaw(_shot);
        float L_Trigger = Input.GetAxisRaw(_crouch);

        //移動
        if (X_Move != 0 || Z_Move != 0)
        {
            if (_playerStatus == PlayerStatus.Crouch && isMyColor == true)
            {
                PlayerCrouchMove(X_Move, Z_Move);
            }
            else
            {
                PlayerWalkMove(X_Move, Z_Move);
            }
        }
        else
        {
            //徐々に減速させる
            _playerMoveDirection -= _playerMoveDirection * Time.deltaTime * _speed;
            //慣性の移動分
            PlayerCrouchMove(X_Move, Z_Move);
            _animator.SetBool("Walking", false);
        }
        //ジャンプ
        if (Input.GetButton(_jump) || Input.GetKey(KeyCode.Space))
        {
            Jump();
        }
        if (Input.GetButtonUp(_jump) || Input.GetKeyUp(KeyCode.Space))
        {
            isJump = false;
        }
        RaycastHit hit;
        //着地
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _rayDistance))
        {
            Debug.DrawRay(transform.position, Vector3.down * _rayDistance, Color.blue);
            Color color = default;
            //当たったオブジェクトがMeshColliderを持っているか確認する
            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (meshCollider != null && meshCollider.sharedMesh != null)
            {
                //ヒットしたポイントのUV座標を取得する
                Vector2 uv = hit.textureCoord;
                //ヒットしたオブジェクトのマテリアルを取得する
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null && renderer.material != null)
                {
                    //UV座標から色をサンプリングする
                    Texture2D texture = renderer.material.mainTexture as Texture2D;
                    if (texture != null)
                    {
                        color = texture.GetPixelBilinear(uv.x, uv.y);
                    }
                }
            }
            if (color.r >= 0.5 && color.g <= 0.5 && color.b <= 0.5)
            {
                isMyColor = true;
            }
            else
            {
                isMyColor = false;
            }
            isJump = false;
            //タイマーリセット
            _timer = 0;
        }
        else if (isJump == false)
        {
            //下向きに移動
            _transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
            _mainCamera.transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
        }
        //潜り状態
        if (L_Trigger != 0 || Input.GetKey(KeyCode.Q))
        {
            //
            if (isMyColor == true)
            {
                this.transform.localScale = Vector3.zero;
            }
            else
            {
                this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
            _playerStatus = PlayerStatus.Crouch;
        }
        else
        {
            this.transform.localScale = Vector3.one;
            _playerMoveDirection = Vector3.zero;
            _playerStatus = PlayerStatus.Idle;
        }
        //射撃
        if (R_Trigger != 0 || Input.GetKey(KeyCode.E))
        {
            //加速度を初期化
            _playerMoveDirection = Vector3.zero;
            //人状態にする
            this.transform.localScale = Vector3.one;
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
        }

    }
    /// <summary>
    /// プレイヤーを移動させる
    /// </summary>
    /// <param name="MoveX">Xの移動量</param>
    /// <param name="MoveZ">Zの移動量</param>
    private void PlayerWalkMove(float MoveX, float MoveZ)
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
        //壁にぶつかっているか
        if (!Physics.Raycast(transform.position, transform.forward, _rayDistance))
        {
            //移動させる
            _transform.position += moveDirection * _speed * Time.deltaTime;
            _mainCamera.transform.position += moveDirection * _speed * Time.deltaTime;
        }
    }
    /// <summary>
    /// 潜り状態の移動
    /// </summary>
    /// <param name="MoveX">Xの移動量</param>
    /// <param name="MoveZ">Zの移動量</param>
    private void PlayerCrouchMove(float MoveX, float MoveZ)
    {
        //カメラの前方向を取得
        Vector3 comForward = new Vector3(_mainCamera.transform.forward.x, 0, _mainCamera.transform.forward.z).normalized;
        //移動量を計算
        //カメラの方向
        Vector3 cameraRight = _mainCamera.transform.right;
        //y座標を消す
        cameraRight.y -= _mainCamera.transform.right.y;
        Vector3 moveDirection = comForward * MoveZ + cameraRight * MoveX;
        moveDirection = moveDirection.normalized;
        //移動速度を加速させる
        _playerMoveDirection += moveDirection * Time.deltaTime * 2;
        //移動速度の大きさが上限値を超えないように制限
        if (_playerMoveDirection.magnitude > _maxSpeed)
        {
            _playerMoveDirection = _playerMoveDirection.normalized * _maxSpeed;
        }
        //移動させる
        _transform.position += _playerMoveDirection * _crouchSpeed * Time.deltaTime;
        _mainCamera.transform.position += _playerMoveDirection * _crouchSpeed * Time.deltaTime;
    }
    /// <summary>
    /// ジャンプさせる
    /// </summary>
    private void Jump()
    {
        //ジャンプ可能か
        if (_timer < _jumpTime)
        {
            isJump = true;
            _timer += Time.deltaTime;
            _transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
            _mainCamera.transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
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