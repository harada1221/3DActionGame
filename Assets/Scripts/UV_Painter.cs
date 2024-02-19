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

public class UV_Painter  : MonoBehaviour
{
    [SerializeField]
    private Brush _brush;

    [SerializeField]
    private GameObject _paintObj;

    private Texture2D _tex;


    void Start()
    {
        _tex = new Texture2D(_paintObj.GetComponent<Renderer>().material.mainTexture.width, _paintObj.GetComponent<Renderer>().material.mainTexture.height);
        Graphics.CopyTexture(_paintObj.GetComponent<Renderer>().material.mainTexture, 0, 0, _tex, 0, 0);
        _brush.UpdateBrushColor();
    }

    void Update()
    {
        if (!Input.GetMouseButton(0)) return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Renderer renderer = hit.collider.gameObject.GetComponent<Renderer>();
            MeshCollider meshCollider = hit.collider as MeshCollider;

            if (renderer == null || renderer.sharedMaterial == null || renderer.sharedMaterial.mainTexture == null || meshCollider == null)
            {
                Debug.Log("NULL");
                return;
            }

            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= _tex.width;
            pixelUV.y *= _tex.height;
            // Debug.Log("pixelUV:::" + (int)pixelUV.x + " , " + (int)pixelUV.y);

            _tex.SetPixels((int)pixelUV.x - _brush.brushWidth / 2, (int)pixelUV.y - _brush.brushHeight / 2, _brush.brushWidth, _brush.brushHeight, _brush.colors);
            _tex.Apply();
            renderer.material.mainTexture = _tex;
        }
    }
}

