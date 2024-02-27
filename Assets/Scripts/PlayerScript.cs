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
    //タイマーカウント
    private float _timer = default;
    //ジャンプフラグ
    private bool isJump = false;
    //射撃フラグ
    private bool isShoot = false;
    //自分の色の上フラグ
    private bool isMyColor = false;
    //プレイヤーのステート
    private PlayerStatus _playerStatus = PlayerStatus.Idle;
    //プレイヤーの移動方向
    private Vector3 _playerMoveDirection = default;

    //入力の名前
    private const string _horizontal = "Horizontal";
    private const string _vertical = "Vertical";
    private const string _jump = "Jump2";
    private const string _shot = "RTrigger";
    private const string _crouch = "LTrigger";
    #endregion
    private enum PlayerStatus
    {
        Idle,
        Crouch,
        Diver,
        Small
    }
    #region プロパティ
    public bool GetShoot { get => isShoot; }
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
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
        Debug.DrawRay(transform.position, transform.forward * _rayDistance, Color.blue);
        //スティックのX,Y軸がどれほど移動したか
        float X_Move = Input.GetAxisRaw(_horizontal);
        float Z_Move = Input.GetAxisRaw(_vertical);
        //コントローラーのR.Lトリガー
        float R_Trigger = Input.GetAxisRaw(_shot);
        float L_Trigger = Input.GetAxisRaw(_crouch);
        //移動
        if (X_Move != 0 || Z_Move != 0)
        {
            Debug.Log(_playerStatus);
            switch (_playerStatus)
            {
                //潜り状態の移動
                case PlayerStatus.Crouch:
                    PlayerCrouchMove(X_Move, Z_Move);
                    break;
                //歩き状態の移動
                case PlayerStatus.Idle:
                    PlayerWalkMove(X_Move, Z_Move);
                    break;
                case PlayerStatus.Small:
                    PlayerWalkMove(X_Move, Z_Move);
                    break;
                //壁の潜り状態の移動
                case PlayerStatus.Diver:
                    PlayerDiverMove(X_Move, Z_Move);
                    break;
            }
        }
        else
        {
            //移動の慣性が残っているか
            if (_playerMoveDirection != Vector3.zero)
            {
                //徐々に減速させる
                _playerMoveDirection -= _playerMoveDirection * Time.deltaTime * _speed;
                //慣性の移動分
                PlayerCrouchMove(X_Move, Z_Move);
            }
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
            //自分の色の上にいるか
            ColorCheck(hit);
            //ジャンプリセット
            isJump = false;
            _timer = 0;
        }
        else if (isJump == false)
        {
            //下向きに移動
            transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
            _mainCamera.transform.position += Vector3.down * _fallSpeed * Time.deltaTime;
        }
        //潜り状態
        if (L_Trigger != 0 || Input.GetKey(KeyCode.Q))
        {
            //自分の色のに触れているか
            if (isMyColor == true)
            {
                if (_playerStatus != PlayerStatus.Diver)
                {
                    _playerStatus = PlayerStatus.Crouch;
                    this.transform.localScale = Vector3.zero;
                }
            }
            else
            {
                _playerStatus = PlayerStatus.Small;
                this.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            }
        }
        else
        {
            //大きさを戻す
            this.transform.localScale = Vector3.one;
            //潜り移動の慣性をゼロに
            _playerMoveDirection = Vector3.zero;
            //ステータス変更
            _playerStatus = PlayerStatus.Idle;
        }
        //射撃
        if (R_Trigger != 0 || Input.GetKey(KeyCode.E))
        {
            //加速度を初期化
            _playerMoveDirection = Vector3.zero;
            //通常状態にする
            this.transform.localScale = Vector3.one;
            //カメラの向き調整
            CameraRevolution();
            //射撃
            _gunScript.Ballistic();
            //射撃フラグ変更
            isShoot = true;
            _playerStatus = PlayerStatus.Idle;
        }
        else
        {
            //射撃終了
            isShoot = false;
        }

    }
    /// <summary>
    /// プレイヤーの歩き移動
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
        if (moveDirection != Vector3.zero && isShoot == false)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = newRotation;
        }
        //ぶつかっているか
        if (!Physics.Raycast(transform.position, moveDirection, _rayDistance))
        {
            //移動させる
            transform.position += moveDirection * _speed * Time.deltaTime;
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
        _playerMoveDirection += moveDirection * Time.deltaTime;
        //移動速度の大きさが上限値を超えないように制限
        if (_playerMoveDirection.magnitude > _maxSpeed)
        {
            _playerMoveDirection = _playerMoveDirection.normalized * _maxSpeed;
        }
        //プレイヤーを移動方向に向かせる
        if (moveDirection != Vector3.zero && isShoot == false)
        {
            Quaternion newRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = newRotation;
        }
        //壁に当たっているか
        if (!Physics.Raycast(transform.position, _playerMoveDirection, _rayDistance))
        {
            //移動させる
            transform.position += _playerMoveDirection * _crouchSpeed * Time.deltaTime;
            _mainCamera.transform.position += _playerMoveDirection * _crouchSpeed * Time.deltaTime;
        }
        else
        {
            //壁移動状態に変更
            _playerStatus = PlayerStatus.Diver;
            //慣性をゼロにする
            _playerMoveDirection = Vector3.zero;
        }
    }
    /// <summary>
    /// 壁移動
    /// </summary>
    /// <param name="MoveX">Xの移動量</param>
    /// <param name="MoveZ">Zの移動量</param>
    private void PlayerDiverMove(float MoveX, float MoveZ)
    {
        this.transform.localScale = Vector3.zero;
        RaycastHit hit;
        Vector3 moveDirection = default;
        if (Physics.Raycast(transform.position, transform.forward, out hit, _rayDistance))
        {
            isJump = false;
            _timer = 0;
            ColorCheck(hit);
            //衝突したオブジェクトの法線ベクトルを取得
            Vector3 surfaceNormal = hit.normal;
            //法線ベクトルに対して垂直な方向を取得
            if (isMyColor == true)
            {
                Vector3 horizontalDirection = -Vector3.Cross(Vector3.up, surfaceNormal).normalized;
                moveDirection = Vector3.up * MoveZ + MoveX * horizontalDirection;
            }
            else
            {
                Debug.Log("in");
                moveDirection = Vector3.up * 20;
            }

        }
        if (Physics.Raycast(transform.position, Vector3.down, _rayDistance) && MoveZ <= 0)
        {
            _playerStatus = PlayerStatus.Crouch;
        }
        //プレイヤーとカメラの位置を移動
        transform.position += moveDirection * _speed * Time.deltaTime;
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
            isJump = true;
            //タイマー加算
            _timer += Time.deltaTime;
            //上に移動
            transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
            _mainCamera.transform.position += Vector3.up * _jumpSpeed * Time.deltaTime;
        }
        else
        {
            isJump = false;
        }
    }
    /// <summary>
    /// 射撃時のプレイヤーの向き調整
    /// </summary>
    private void CameraRevolution()
    {
        //1回目の射撃か
        if (isShoot == false)
        {
            //カメラの向きに合わせて回転
            Vector3 playerRotation = new Vector3(transform.rotation.x, _mainCamera.transform.eulerAngles.y, transform.rotation.z);
            transform.rotation = Quaternion.Euler(playerRotation);
        }
    }
    private void ColorCheck(RaycastHit hit)
    {
        Color color = default;
        //当たったオブジェクトがMeshColliderを持っているか確認する
        MeshCollider meshCollider = hit.collider.GetComponent<MeshCollider>();
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
        //仮置き\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        if (color.r >= 0.4 && color.g <= 0.4 && color.b <= 0.4)
        {
            isMyColor = true;
        }
        else
        {
            isMyColor = false;
        }
    }
}