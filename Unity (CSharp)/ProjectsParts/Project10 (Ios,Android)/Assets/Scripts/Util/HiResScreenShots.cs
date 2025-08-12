using UnityEngine;
using System.Collections;

public class HiResScreenShots : MonoBehaviour {

    public int resWidth = 3300;
    public int resHeight = 2550;

    private bool takeHiResShot = false;

    public static string ScreenShotName(int width, int height) { return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                                                                                    Application.dataPath,
                                                                                    width, height,
                                                                                    System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                                                               }

    public void TakeHiResShot() {
        takeHiResShot = true;
    }

    void LateUpdate() {
        takeHiResShot |= Input.GetKeyDown("k");
        if (takeHiResShot) {
            RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            Camera.main.targetTexture = rt;
            Texture2D screenShot = new Texture2D(resWidth, resHeight, TextureFormat.RGB24, false);
            Camera.main.Render();
            RenderTexture.active = rt;
            screenShot.ReadPixels(new Rect(0, 0, resWidth, resHeight), 0, 0);
            Camera.main.targetTexture = null;
            RenderTexture.active = null; // JC: added to avoid errors
            Destroy(rt);
            byte[] bytes = screenShot.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
        }
    }

    public static byte[] GetScreenShoot() {
        RenderTexture rt = new RenderTexture((int)(Camera.main.pixelWidth/3f) , (int)(Camera.main.pixelHeight/ 3f) , 24);
        Camera.main.targetTexture = rt;
        Texture2D screenShot = new Texture2D(Camera.main.pixelWidth , Camera.main.pixelHeight , TextureFormat.RGB24 , false);
        Camera.main.Render();
        RenderTexture.active = rt;
        screenShot.ReadPixels(new Rect(0 , 0 , Camera.main.pixelWidth , Camera.main.pixelHeight) , 0 , 0);
        Camera.main.targetTexture = null;
        RenderTexture.active = null; // JC: added to avoid errors
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        string filename = ScreenShotName((int)(Camera.main.pixelWidth/ 3f) , (int)(Camera.main.pixelHeight/ 3f));
        System.IO.File.WriteAllBytes(filename , bytes);
        Debug.Log(string.Format("Took screenshot to: {0}" , filename));

        return bytes;
    }

}
