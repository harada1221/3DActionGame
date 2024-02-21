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
    [SerializeField]
    private Texture _texture;
    MeshRenderer _meshRenderer = default;
    MeshFilter _meshFilter = default;
    DecalPainter _decalPainter = default;
    Material _material= default;
    #endregion
    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        //マテリアルをインスタンス化
        _material = _meshRenderer.material;
        if (_material == null)
        {
            _material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }

        //このMesh用デカール累積テクスチャを生成・設定
        int textureSize = _material.mainTexture != null
            ? _material.mainTexture.width
            : 1024;
        _decalPainter = new DecalPainter(_meshFilter, textureSize);
        _decalPainter.BakeBaseTexture(_material.mainTexture);
        _material.mainTexture = _decalPainter.texture;

        //ペイントテクスチャを設定
        _decalPainter.SetDecalTexture(_texture);
    }


    public void Paint(
    Vector3 worldPosition,
    Vector3 normal,
    Vector3 tangent,
    float decalSize,
    Color color)
    {
        Vector3 positionOS = transform.InverseTransformPoint(worldPosition);
        Vector3 normalOS = transform.InverseTransformPoint(normal);
        Vector3 tangentOS = transform.InverseTransformDirection(tangent);
        _decalPainter.SetPointer(
             positionOS,
             normalOS,
             tangentOS,
             decalSize,
             color,
             transform.lossyScale
            );
        _decalPainter.Paint();
    }
}

