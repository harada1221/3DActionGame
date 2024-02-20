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

public class yyyyy  : MonoBehaviour
{
    #region 変数宣言
    [SerializeField]
    private MeshFilter MeshFilter;
    #endregion



    private void OnCollisionEnter(Collision collision)
    {
        Painteble ppp = collision.gameObject.GetComponent<Painteble>();
        var contacter = collision.GetContact(0);
        var normal = contacter.normal;
        var hitposition = contacter.point;
        var tangent = Vector3.Cross(normal, Vector3.right).normalized ;

        if (tangent.sqrMagnitude < 0.01f)
        {
            tangent = Vector3.Cross(normal, Vector3.forward).normalized;
        }

        float size = 2048;
        Color c = Color.black;

        print("いヴぁーーー");
        ppp.P
            (
                hitposition,
                normal,
                tangent,
                size,
                c
            );
    }
}

