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

public class Painteble  : MonoBehaviour
{
    #region 変数宣言
    private DecalPainter pp;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        pp = new DecalPainter(this.gameObject.GetComponent<MeshFilter>());
    }


    public void P(
    Vector3 paintPositionOnObjectSpace,
    Vector3 normal,
    Vector3 tangent,
    float decalSize,
    Color color)
    {
        pp.SetPointer(
             paintPositionOnObjectSpace,
             normal,
             tangent,
             decalSize,
             color,
             transform.lossyScale
            );
        pp.Paint();
    }
}

