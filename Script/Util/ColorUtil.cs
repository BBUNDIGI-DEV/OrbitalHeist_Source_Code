using System;
using UnityEngine;

    public static class ColorUtil
    {
        public static readonly Color[] COLOR_LIST = new Color[] { Color.red, Color.blue, Color.green, Color.cyan, Color.yellow };

        public static void DrawRandomColorLine(Vector3 start, Vector3 end, int colorIndex, float duration)
        {
            Debug.DrawLine(start, end, COLOR_LIST[colorIndex % COLOR_LIST.Length], duration);
        }

        public static readonly Color WHITE_FADE_OUT = new Color(1.0f, 1.0f, 1.0f, 0.0f);
    }
