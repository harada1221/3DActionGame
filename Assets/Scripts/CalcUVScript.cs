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

public class CalcUVScript : MonoBehaviour
{
    public void HitObj(RaycastHit hitinfo)
    {
        MeshFilter meshRenderer = hitinfo.transform.GetComponent<MeshFilter>();
        Mesh mesh = meshRenderer.sharedMesh;
        for (int i = 0; i < mesh.triangles.Length; i += 3)
        {
            #region 1.ある点pが与えられた3点において平面上に存在するか

            int indexFirst = i + 0;
            int indexSecond = i + 1;
            int indexThird = i + 2;

            //三角形の座標を持ってくる
            Vector3 triangleVerticesFirst = mesh.vertices[mesh.triangles[indexFirst]];
            Vector3 triangleVerticesSecound = mesh.vertices[mesh.triangles[indexSecond]];
            Vector3 triangleVerticesThird = mesh.vertices[mesh.triangles[indexThird]];
            Vector3 localHitPoint = hitinfo.transform.InverseTransformPoint(hitinfo.point);
            //最初の座標から次の座標の位置
            Vector3 edgeVectorFirst = triangleVerticesSecound - triangleVerticesFirst;
            Vector3 edgeVectorSecond = triangleVerticesThird - triangleVerticesFirst;
            Vector3 edgeVectorThird = localHitPoint - triangleVerticesFirst;
            //2つのベクトルの外積
            Vector3 crossProduct = Vector3.Cross(edgeVectorFirst, edgeVectorSecond);
            float val = Vector3.Dot(crossProduct, edgeVectorThird);

            //適当に小さい少数値で誤差をカバー
            bool suc = -0.000001f < val && val < 0.000001f;

            #endregion 1.ある点pが与えられた3点において平面上に存在するか

            #region 2.同一平面上に存在する点pが三角形内部に存在するか

            if (!suc)
            {
                continue;
            }
            else
            {
                Vector3 a = Vector3.Cross(triangleVerticesFirst - triangleVerticesThird, localHitPoint - triangleVerticesFirst).normalized;
                Vector3 b = Vector3.Cross(triangleVerticesSecound - triangleVerticesFirst, localHitPoint - triangleVerticesSecound).normalized;
                Vector3 c = Vector3.Cross(triangleVerticesThird - triangleVerticesSecound, localHitPoint - triangleVerticesThird).normalized;

                float d_ab = Vector3.Dot(a, b);
                float d_bc = Vector3.Dot(b, c);

                suc = 0.999f < d_ab && 0.999f < d_bc;
            }

            #endregion 2.同一平面上に存在する点pが三角形内部に存在するか

            #region 3.点pのUV座標を求める

            if (!suc)
            {
                continue;
            }
            else
            {
                Vector2 uv1 = mesh.uv[mesh.triangles[indexFirst]];
                Vector2 uv2 = mesh.uv[mesh.triangles[indexSecond]];
                Vector2 uv3 = mesh.uv[mesh.triangles[indexThird]];

                //PerspectiveCollect
                Matrix4x4 mvp = Camera.main.projectionMatrix * Camera.main.worldToCameraMatrix * hitinfo.transform.localToWorldMatrix;
                //各点をProjectionSpaceへの変換
                Vector4 p1_p = mvp * new Vector4(triangleVerticesFirst.x, triangleVerticesFirst.y, triangleVerticesFirst.z, 1);
                Vector4 p2_p = mvp * new Vector4(triangleVerticesSecound.x, triangleVerticesSecound.y, triangleVerticesSecound.z, 1);
                Vector4 p3_p = mvp * new Vector4(triangleVerticesThird.x, triangleVerticesThird.y, triangleVerticesThird.z, 1);
                Vector4 p_p = mvp * new Vector4(localHitPoint.x, localHitPoint.y, localHitPoint.z, 1);
                //通常座標への変換(ProjectionSpace)
                Vector2 p1_n = new Vector2(p1_p.x, p1_p.y) / p1_p.w;
                Vector2 p2_n = new Vector2(p2_p.x, p2_p.y) / p2_p.w;
                Vector2 p3_n = new Vector2(p3_p.x, p3_p.y) / p3_p.w;
                Vector2 p_n = new Vector2(p_p.x, p_p.y) / p_p.w;
                //頂点のなす三角形を点pにより3分割し、必要になる面積を計算
                float s = 0.5f * ((p2_n.x - p1_n.x) * (p3_n.y - p1_n.y) - (p2_n.y - p1_n.y) * (p3_n.x - p1_n.x));
                float s1 = 0.5f * ((p3_n.x - p_n.x) * (p1_n.y - p_n.y) - (p3_n.y - p_n.y) * (p1_n.x - p_n.x));
                float s2 = 0.5f * ((p1_n.x - p_n.x) * (p2_n.y - p_n.y) - (p1_n.y - p_n.y) * (p2_n.x - p_n.x));
                //面積比からuvを補間
                float u = s1 / s;
                float v = s2 / s;
                float w = 1 / ((1 - u - v) * 1 / p1_p.w + u * 1 / p2_p.w + v * 1 / p3_p.w);
                Vector2 uv = w * ((1 - u - v) * uv1 / p1_p.w + u * uv2 / p2_p.w + v * uv3 / p3_p.w);

                //uvが求まったよ!!!!
                Debug.Log(uv + ":" + hitinfo.textureCoord);
                return;
            }

            #endregion 3.点pのUV座標を求める
        }
    }
}


