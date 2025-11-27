using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance;
    public static float timeTaken = 0f;
    public static int hitsTaken = 0;
    public static bool victory = true;
    public static List<Texture2D> photoList = new List<Texture2D>();

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
    public static void makeScreenshotsFolder()
    {
        var folder = Directory.CreateDirectory(AppDomain.CurrentDomain.BaseDirectory + "/Photos");
        Debug.Log("ScoreManager: Photo folder created at " + folder.FullName);
    }
    public static void writeScreenshotsToDisk()
    {
        for (int i = 0; i < photoList.Count; i++) {
            byte[] bytes =  photoList[i].EncodeToPNG();
            string filename = AppDomain.CurrentDomain.BaseDirectory + "/Photos/photo_saved_" + i + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png";
            File.WriteAllBytes(filename, bytes);
            Debug.Log("ScoreManager: Photo saved to " + filename);

        }
        for (int i = 0; i < photoList.Count; i++) {
            Destroy(photoList[i]);
        }
        photoList.Clear();
    }
}
