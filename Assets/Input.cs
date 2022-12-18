using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MyInput : MonoBehaviour
{
    public int index = 0;
    public Image[] images;
    public Camera camera;
    public Transform pos;
    public GameObject test;
    private Material mt;
    private Texture[] my_t;
    /// 运行模式下Texture转换成Texture2D
    private Texture2D TextureToTexture2D(Texture texture)
    {
        Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
        Graphics.Blit(texture, renderTexture);

        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        texture2D.Apply();

        RenderTexture.active = currentRT;
        RenderTexture.ReleaseTemporary(renderTexture);

        return texture2D;
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    
    private void RefreshPrint(string filePaht, Image image)
    {
        FileStream fileStream = new FileStream(filePaht, FileMode.Open, FileAccess.Read);
        fileStream.Seek(0, SeekOrigin.Begin);
        byte[] bytes = new byte[fileStream.Length];
        fileStream.Read(bytes, 0, (int)fileStream.Length);
        fileStream.Close();
        fileStream.Dispose();
        fileStream = null;
        Texture2D texture2D = new Texture2D(1920, 1080, TextureFormat.RGB24, false);
        texture2D.LoadImage(bytes);
        Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        image.sprite = sprite;
    }

    void Recursive2(GameObject parentGameObject)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            SkinnedMeshRenderer c1 = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (c1 && c1.sharedMaterial.shader.name != "Hidden/InternalErrorShader")
            {
                mt = c1.sharedMaterial;
            }
            Recursive2(child.gameObject);
        }
    }

    void Recursive(GameObject parentGameObject, Texture my_t = null)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            SkinnedMeshRenderer c1 = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (c1 && c1.sharedMaterial.shader.name == "Hidden/InternalErrorShader")
            {
                Debug.Log(c1.sharedMaterial.GetTexture("_MainTex"));
                Texture2D texture = LoadPNG("D:\\SavedScreen.png");
                c1.sharedMaterial.SetTexture("_MainTex", texture);
                //Texture2D tex = TextureToTexture2D(c1.sharedMaterial.GetTexture("_MainTex"));
                //byte[] bytes = tex.EncodeToPNG();
                //File.WriteAllBytes("D:\\SavedScreen.png", bytes);
                Debug.Log(c1.sharedMaterial.shader.name);
                c1.sharedMaterial.shader = Shader.Find("Sprites/Default");
                //c1.sharedMaterial.shader = mt.shader;
                //c1.sharedMaterial = mt;
                // Debug.Log(t);
            }
            Recursive(child.gameObject, my_t);
        }
    }

    private void MyEndEdit(string path)
    {
        Debug.Log(path);
        if (File.Exists(path))
        {
            // path = "E:\\Documents\\Tencent Files\\1094766238\\FileRecv\\__data";
            //"E:\Documents\Tencent Files\1094766238\FileRecv\__data"
            AssetBundle ab = AssetBundle.LoadFromFile(path);
            if(ab)
            {
                Debug.Log(ab);
                string[] patht = ab.GetAllAssetNames();
                
                foreach (string s in patht)
                {
                    Debug.Log(s);
                    GameObject prefab = ab.LoadAsset<GameObject>(s);
                    Recursive(prefab);
                    GameObject gob = Instantiate(prefab, pos.position, pos.rotation);
                    //UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(gob, "Assets\\test.prefab", UnityEditor.InteractionMode.UserAction);
                    //MonoBehaviour.DestroyImmediate(gob);
                    //gob.transform.position.Set(0,10,0);
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        /*
        InputField myInput = this.GetComponent<InputField>();
        //myInput.onEndEdit.AddListener(MyEndEdit);

        //Recursive2(test);

        string path = "E:\\Documents\\Tencent Files\\1094766238\\FileRecv\\__data";
        AssetBundle ab = AssetBundle.LoadFromFile(path);
        Texture[] my_t = ab.LoadAllAssets<Texture>();
        if (ab)
        {
            Debug.Log(ab);
            string[] patht = ab.GetAllAssetNames();
            foreach (string s in patht)
            {
                Debug.Log(s);
                GameObject prefab = ab.LoadAsset<GameObject>(s);
                Texture texture = Resources.Load<Texture>(s);
                Recursive(prefab);
                GameObject gob = Instantiate(prefab, pos.position, pos.rotation);
                //UnityEditor.PrefabUtility.SaveAsPrefabAssetAndConnect(gob, "Assets\\test.prefab", UnityEditor.InteractionMode.UserAction);
                //MonoBehaviour.DestroyImmediate(gob);
                //gob.transform.position.Set(0,10,0);
            }
        }
        */
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
