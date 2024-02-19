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

public class Brush  : MonoBehaviour
{
    public int brushWidth = 150;
    public int brushHeight = 150;
    public Color color = Color.blue;

    public Color[] colors { get; set; }

    public void UpdateBrushColor()
    {
        colors = new Color[brushWidth * brushHeight];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
    }
}

