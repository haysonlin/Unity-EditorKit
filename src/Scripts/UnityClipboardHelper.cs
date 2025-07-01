using UnityEngine;
using System;
using System.Runtime.InteropServices; // 引入 P/Invoke 相關命名空間

public class UnityClipboardHelper 
{
    // DLL 檔案的名稱 (不包含 .dll 副檔名)
    private const string DLL_NAME = "ClipboardHelper";

    // 匯入 C++ DLL 中的 SetImageToClipboard 函數
    [DllImport(DLL_NAME)]
    private static extern bool SetImageToClipboard(IntPtr pixelData, int width, int height, int bytesPerPixel);

    /// <summary>
    /// 將 Texture2D 的像素資料複製到剪貼簿。
    /// 注意：此方法會將 RGBA 像素轉換為 BGRA，以符合 Windows DIB 的預期格式。
    /// </summary>
    /// <param name="texture">要複製的 Texture2D。</param>
    /// <returns>如果成功複製到剪貼簿則為 true，否則為 false。</returns>
    public static bool CopyTextureToClipboard(Texture2D texture)
    {
        if (texture == null)
        {
            Debug.LogError("提供的 Texture2D 為空。");
            return false;
        }
        
        var pixels = texture.GetPixels32();
        byte[] bgraPixels = new byte[pixels.Length * 4];

        for (int i = 0; i < pixels.Length; i++)
        {
            Color32 p = pixels[i];
            bgraPixels[i * 4 + 0] = p.b;
            bgraPixels[i * 4 + 1] = p.g;
            bgraPixels[i * 4 + 2] = p.r;
            bgraPixels[i * 4 + 3] = p.a;
        }
        
        GCHandle gcHandle = GCHandle.Alloc(bgraPixels, GCHandleType.Pinned);
        try
        {
            IntPtr pixelDataPtr = gcHandle.AddrOfPinnedObject();
            int width = texture.width;
            int height = texture.height;
            int bytesPerPixel = 4; // BGRA，每個分量 1 byte
            return SetImageToClipboard(pixelDataPtr, width, height, bytesPerPixel);
        }
        finally
        {
            if (gcHandle.IsAllocated)
            {
                gcHandle.Free();
            }
        }
    }
}