using System;
using UnityEngine;

public static class b64toTex
{
    public static Texture convert(string str)
    {
        byte[] imageBytes = Convert.FromBase64String(str);
        Texture2D tex = new Texture2D(256, 256);
        tex.LoadImage(imageBytes);

        return tex;
    }
}
