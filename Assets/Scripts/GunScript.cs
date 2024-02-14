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

public class GunScript : MonoBehaviour
{
    #region 変数宣言
    [SerializeField, Header("発射位置")]
    private Transform _shootPosition = default;
    [SerializeField, Header("メインカメラ")]
    private Camera _mainCamera = default;
    [SerializeField, Header("威力")]
    private float _power = default;
    [SerializeField, Header("玉")]
    private GameObject _ball = default;
    [SerializeField, Header("生成する数")]
    private int _maxCount = 100;
    [SerializeField]
    private BallScript _ballScript = default;
    private Vector3 _finalDestination = default;
    private Queue<BallScript> _ballQueue = default;
    #endregion
    public float GetPower { get => _power; }
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        //プール生成
        _ballQueue = new Queue<BallScript>();
        //最大数作る
        for (int i = 0; i < _maxCount; i++)
        {
            //生成
            BallScript ball = Instantiate(_ballScript, _shootPosition.transform.position, Quaternion.identity, transform);
            ball.transform.parent = null;
            //Queueに追加
            _ballQueue.Enqueue(ball);
        }
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //Debug.Log(_shootPosition.position);
    }
    /// <summary>
    ///　弾を表示
    /// </summary>
    public void Ballistic()
    {
        //Queueの中になかったら生成
        if (_ballQueue.Count <= 0)
        {
            //生成
            BallScript ball = Instantiate(_ballScript, _shootPosition.transform.position, Quaternion.identity, transform);
            ball.transform.parent = null;
            //Queueに追加
            _ballQueue.Enqueue(ball);
        }
        //弾の方向を設定
        _finalDestination = transform.forward;
        _finalDestination.y = _mainCamera.transform.forward.y;
        //弾を取り出す
        BallScript ballScript = _ballQueue.Dequeue();
        //弾を表示
        ballScript.gameObject.SetActive(true);
        //発射位置に移動
        ballScript.transform.position = _shootPosition.transform.position;
        //方向を決定
        ballScript.SetVelocity(_finalDestination);
    }
    /// <summary>
    /// 弾を格納する
    /// </summary>
    /// <param name="ballScript">格納する弾</param>
    public void BallCollect(BallScript ballScript)
    {
        //弾のゲームオブジェクトを非表示
        ballScript.gameObject.SetActive(false);
        //Queueに格納
        _ballQueue.Enqueue(ballScript);
    }
}

