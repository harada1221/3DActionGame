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

public class ballScript : MonoBehaviour
{
    #region 変数宣言
    private ballisticScript _ballisticScript = default;
    //[SerializeField]
    //private float _speed = default;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Start()
    {
        _ballisticScript = GetComponentInParent<ballisticScript>();
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update()
    {
        //位置の更新
        //transform.Translate(_ballisticScript.GetTargetPosition * _speed * Time.deltaTime);
    }
}

