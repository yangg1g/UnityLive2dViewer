using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Windows;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using B83.Win32;
using SpringGUI;
using Gif.Components;
public static class BinaryReaderExtensions
{
    public static string ReadStringToNull(this BinaryReader reader, int maxLength = 32767)
    {
        var bytes = new List<byte>();
        int count = 0;
        while (reader.BaseStream.Position != reader.BaseStream.Length && count < maxLength)
        {
            var b = reader.ReadByte();
            if (b == 0)
            {
                break;
            }
            bytes.Add(b);
            count++;
        }
        return Encoding.UTF8.GetString(bytes.ToArray());
    }

    public static byte[] ReadByteToNull(this BinaryReader reader, int maxLength = 32767)
    {
        var bytes = new List<byte>();
        int count = 0;
        while (reader.BaseStream.Position != reader.BaseStream.Length && count < maxLength)
        {
            var b = reader.ReadByte();
            if (b == 0)
            {
                break;
            }
            bytes.Add(b);
            count++;
        }
        return bytes.ToArray();
    }
}

public class Header
{
    public string signature;
    public uint version;
    public byte[] unityVersion;
    public byte[] unityRevision;
    public long size;
    public uint compressedBlocksInfoSize;
    public uint uncompressedBlocksInfoSize;
}

public class MyMain : MonoBehaviour
{
    [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
    public static extern int MessageBox(IntPtr handle, string message, string title, int type);

    int IndexOf(byte[] srcBytes, byte[] searchBytes, int offset = 0)
    {
        if (offset == -1) { return -1; }
        if (srcBytes == null) { return -1; }
        if (searchBytes == null) { return -1; }
        if (srcBytes.Length == 0) { return -1; }
        if (searchBytes.Length == 0) { return -1; }
        if (srcBytes.Length < searchBytes.Length) { return -1; }
        for (var i = offset; i < srcBytes.Length - searchBytes.Length; i++)
        {
            if (srcBytes[i] != searchBytes[0]) continue;
            if (searchBytes.Length == 1) { return i; }
            var flag = true;
            for (var j = 1; j < searchBytes.Length; j++)
            {
                if (srcBytes[i + j] != searchBytes[j])
                {
                    flag = false;
                    break;
                }
            }
            if (flag) { return i; }
        }
        return -1;
    }

    public static Texture2D LoadPNG(string filePath)
    {

        Texture2D tex = null;
        if (File.Exists(filePath))
        {
            return LoadPNG(File.ReadAllBytes(filePath));
        }
        return tex;
    }

    public static Texture2D LoadPNG(byte[] fileData)
    {

        Texture2D tex = new Texture2D(2, 2);
        tex.LoadImage(fileData);
        return tex;
    }

    private Animator animatior = null;
    public Material FindMatrial(string name)
    {
        Type type = Type.GetType("UnityEngine.Material,UnityEngine.dll");
        UnityEngine.Object[] objects = Resources.FindObjectsOfTypeAll(type);
        for (int i = 0; i < objects.Length; i++)
        {
            //Debug.Log(objects[i].name);
            if (objects[i].name == name)
            {
                return objects[i] as Material;
            }
        }
        return null;
    }


    void PrefabRecursive(GameObject parentGameObject, Dictionary<string, Texture2D> map, List<UITreeData> Tree)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            child.gameObject.SetActive(true);
            SkinnedMeshRenderer smr = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            Texture2D tmp;
            if (smr && smr.sharedMaterial.shader.name != "Sprites/Default")
            {
                if (smr.sharedMaterial.GetTexture("_MainTex"))
                {
                    string m_name = smr.sharedMaterial.GetTexture("_MainTex").name;
                    if (map.TryGetValue(m_name, out tmp))
                    {
                        smr.sharedMaterial.SetTexture("_MainTex", tmp);
                    }
                    else
                    {
                        MessageBox(IntPtr.Zero, m_name + "不存在，可能会无法正常显示！", "错误", 0x00000010);
                    }
                }
                //Debug.Log(smr.sharedMaterial.GetTexture("_MainTex"));
                smr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (sr.sharedMaterial.GetTexture("_MainTex"))
                {
                    string m_name = sr.sharedMaterial.GetTexture("_MainTex").name;
                    if (map.TryGetValue(m_name, out tmp))
                    {
                        sr.sharedMaterial.SetTexture("_MainTex", tmp);
                    }
                    else
                    {
                        MessageBox(IntPtr.Zero, m_name + "不存在，可能会无法正常显示！", "错误", 0x00000010);
                    }
                }
                //Debug.Log(sr.sharedMaterial.GetTexture("_MainTex"));
                if (sr.sprite && map.TryGetValue(sr.sprite.texture.name, out tmp))
                {
                    sr.sprite = Sprite.Create(tmp, sr.sprite.rect, new Vector2(0.5f, 0.5f));
                }
                sr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            if(ps)
            {
                Renderer rd = ps.GetComponent<Renderer>();
                rd.sharedMaterial = Default_Particle;
                //rd.sharedMaterial = new Material(Shader.Find("Particle/Default"));
            }
            if (smr || sr || ps)
            {
                Tree.Add(new UITreeData(child.name, child.gameObject));
            }
            PrefabRecursive(child.gameObject, map, Tree);
        }
    }

    void PrefabRecursive2(GameObject parentGameObject, Dictionary<string, Texture2D> map)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            child.gameObject.SetActive(true);
            Animator am = child.gameObject.GetComponent<Animator>();
            if (am)
            {
                foreach (AnimationClip ac in am.runtimeAnimatorController.animationClips)
                {
                    ac.wrapMode = WrapMode.ClampForever;
                    Anim.options.Add(new Dropdown.OptionData(ac.name));
                }
            }
            SkinnedMeshRenderer smr = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            Texture2D tmp;
            if (smr && smr.sharedMaterial.shader.name != "Sprites/Default")
            {
                if (smr.sharedMaterial.GetTexture("_MainTex"))
                {
                    string m_name = smr.sharedMaterial.GetTexture("_MainTex").name;
                    if (map.TryGetValue(m_name, out tmp))
                    {
                        smr.sharedMaterial.SetTexture("_MainTex", tmp);
                    }
                    else
                    {
                        MessageBox(IntPtr.Zero, m_name + "不存在，可能会无法正常显示！", "错误", 0x00000010);
                    }
                }
                //Debug.Log(smr.sharedMaterial.GetTexture("_MainTex"));
                smr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (sr.sharedMaterial.GetTexture("_MainTex"))
                {
                    string m_name = sr.sharedMaterial.GetTexture("_MainTex").name;
                    if (map.TryGetValue(m_name, out tmp))
                    {
                        sr.sharedMaterial.SetTexture("_MainTex", tmp);
                    }
                    else
                    {
                        MessageBox(IntPtr.Zero, m_name + "不存在，可能会无法正常显示！", "错误", 0x00000010);
                    }
                }
                //Debug.Log(sr.sharedMaterial.GetTexture("_MainTex"));
                if (sr.sprite && map.TryGetValue(sr.sprite.texture.name, out tmp))
                {
                    sr.sprite = Sprite.Create(tmp, sr.sprite.rect, new Vector2(0.5f, 0.5f));
                }
                sr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            if (ps)
            {
                Renderer rd = ps.GetComponent<Renderer>();
                rd.sharedMaterial = Default_Particle;
                //rd.sharedMaterial = new Material(Shader.Find("Particle/Default"));
            }
            PrefabRecursive2(child.gameObject, map);
        }
    }

    void GameObjRecursive(GameObject parentGameObject, List<UITreeData> Tree)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            Animator am = child.gameObject.GetComponent<Animator>();
            if (am)
            {
                animatior = am;
                List<AnimationClip> ani_list = new List<AnimationClip>();
                foreach (AnimationClip ac in animatior.runtimeAnimatorController.animationClips)
                {
                    ac.wrapMode = WrapMode.ClampForever;
                    ani_list.Add(ac);
                    //Debug.Log(ac);
                }
                AnimatorOverrideController aoc = new AnimatorOverrideController(animatior.runtimeAnimatorController);
                var anims = new List<KeyValuePair<AnimationClip, AnimationClip>>();
                foreach (var a in aoc.animationClips)
                    anims.Add(new KeyValuePair<AnimationClip, AnimationClip>(a, ani_list[Anim.value]));
                aoc.ApplyOverrides(anims);
                animatior.runtimeAnimatorController = aoc;
            }
            SkinnedMeshRenderer smr = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
            ParticleSystem ps = child.gameObject.GetComponent<ParticleSystem>();
            if (smr || sr || ps)
            {
                Tree.Add(new UITreeData(child.name, child.gameObject));
            }
            GameObjRecursive(child.gameObject, Tree);
        }
    }

    public Camera camera;
    public void PrintScreen(string filePath)
    {
        int accuracy = int.Parse(this.transform.Find("Accuracy").GetComponent<InputField>().text);
        int width = Screen.width * accuracy;
        int height = Screen.height * accuracy;
        RenderTexture rt = new RenderTexture(width, height, 32, RenderTextureFormat.ARGBFloat);
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.ARGB32, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();
        camera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(rt);
        byte[] bytes = screenShot.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
        //RefreshPrint(filePath, images[index]);
    }

    List<string> GetAllFileNames(string path, string pattern = "*")
    {
        List<FileInfo> folder = new DirectoryInfo(path).GetFiles(pattern).ToList();

        return folder.Select(x => x.FullName).ToList();
    }

    public InputField AssetPathInput;
    public Transform pos;
    public Toggle SaveSheck;
    private Material Default_Particle;
    public UITree UITree = null;
    public Dropdown Anim;

    private Stack<GameObject> Live2dObj = new Stack<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        AssetPathInput.text = @"D:\touchfish\the_unity\test.ab";
        Default_Particle = pos.gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial;
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
        List<string> imageFilePaths = new List<string>();
        for(int i=0;i<10;i++)
        {
            imageFilePaths.Add(@"D:\screem\" + i + ".png");
        }
        //string AssetPath = this.transform.Find("AssetPath").GetComponent<InputField>().text;
        //ConvertJpgToGif(imageFilePaths.ToArray(), @"D:\test.gif", 1);
        //UITree.SetData(data);
        // UITree.Inject(data);
    }

    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    public bool ConvertJpgToGif(string[] imageFilePaths, string gifPath, int time)
    {
        try
        {
            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.SetDelay(time);
            e.Start(gifPath);
            //0:循环播放    -1:不循环播放
            e.SetRepeat(0);
            for (int i = 0, count = imageFilePaths.Length; i < count; i++)
            {
                //e.AddFrame(Image.FromFile(Server.MapPath(imageFilePaths[i])));

                System.Drawing.Image img = System.Drawing.Image.FromFile(imageFilePaths[i]);
                //如果多张图片的高度和宽度都不一样，可以打开这个注释
                //img = ReSetPicSize(img, w, h);
                e.AddFrame(img);
            }
            e.Finish();

            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    private AnimatedGifEncoder GifEncoder = new AnimatedGifEncoder();
    int index = -1;
    // Update is called once per frame
    void Update()
    {
        if(animatior && index >= 0)
        {
            AnimatorStateInfo animationState = animatior.GetCurrentAnimatorStateInfo(0);
            AnimatorClipInfo[] myAnimatorClip = animatior.GetCurrentAnimatorClipInfo(0);
            float myTime = myAnimatorClip[0].clip.length * animationState.normalizedTime;
            int frame = int.Parse(this.transform.Find("Frame").GetComponent<InputField>().text);
            if (animationState.normalizedTime > index / frame)
            {
                if(index == 0 && this.transform.Find("GenGif").GetComponent<Toggle>().isOn)
                {
                    GifEncoder.Start(Path.GetDirectoryName(AssetPathInput.text) + "\\" + Anim.options[Anim.value].text + ".gif");
                    GifEncoder.SetRepeat(0);
                    GifEncoder.SetDelay(Convert.ToInt16(animationState.normalizedTime / frame * 1000));
                }
                string picPath = Path.GetDirectoryName(AssetPathInput.text) + "\\" + Anim.options[Anim.value].text;
                if (!Directory.Exists(picPath))
                {
                    Directory.CreateDirectory(picPath);
                }
                PrintScreen(picPath+ "\\" + index + ".png");
                if(this.transform.Find("GenGif").GetComponent<Toggle>().isOn)
                {
                    System.Drawing.Image img = System.Drawing.Image.FromFile(picPath + "\\" + index + ".png");
                    GifEncoder.AddFrame(img);
                }
                index++;
                if (index == frame)
                {
                    index = -1;
                    GifEncoder.Finish();
                    MessageBox(IntPtr.Zero, "已成功生成gif到" + Path.GetDirectoryName(AssetPathInput.text) + "！", "信息", 0x00000000);
                }
            }
            // Debug.Log(myTime);
        }
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        AssetPathInput.text = aFiles[0];
    }
    private GameObject NowPrefab = null;
    public void PreView()
    {
        ClearObj();
        Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();

        AssetBundle.UnloadAllAssetBundles(true);
        string AssetPath = AssetPathInput.text;
        string FolderPath = Path.GetDirectoryName(AssetPath);

        var MyAssetsManager = new AssetStudio.AssetsManager();
        MyAssetsManager.LoadFiles(AssetPath);
        foreach (var assetsFile in MyAssetsManager.assetsFileList)
        {
            foreach (var asset in assetsFile.Objects)
            {
                if (asset is AssetStudio.Texture2D)
                {
                    AssetStudio.Texture2D texture = (AssetStudio.Texture2D)asset;

                    var bitmap = new AssetStudio.Texture2DConverter(texture).ConvertToBitmap();
                    MemoryStream ms = new MemoryStream();
                    bitmap.Save(ms, ImageFormat.Png);
                    TextureMap.Add(texture.m_Name, LoadPNG(ms.ToArray()));
                    if(SaveSheck.isOn)
                    {
                        bitmap.Save(Path.Combine(FolderPath, texture.m_Name + ".png"), ImageFormat.Png);
                    }
                }
            }
        }

        AssetBundle ab = MyGetAssetBundle(AssetPath);
        if (!ab)
        {
            MessageBox(IntPtr.Zero, "AssetBundle文件加载失败！", "错误", 0x00000010);
            return;
        }
        string[] ab_path = ab.GetAllAssetNames();
        if (ab_path.Length > 1)
        {
            MessageBox(IntPtr.Zero, "该包可能包含多个模型，此情况暂未处理！", "错误", 0x00000010);
            return;
        }
        Anim.ClearOptions();
        NowPrefab = ab.LoadAsset<GameObject>(ab_path[0]);
        PrefabRecursive2(NowPrefab, TextureMap);
        GameObject gob = Instantiate(NowPrefab, pos.position, pos.rotation);
        List<UITreeData> MyTree = new List<UITreeData>();
        // PrefabRecursive(gob, TextureMap, MyTree);
        GameObjRecursive(gob, MyTree);
        UITree.Inject(MyTree);
        Live2dObj.Push(gob);
    }

    private AssetBundle MyGetAssetBundle(string AssetPath)
    {
        List<string> AssetPaths = new List<string>();
        byte[] buf;
        int pos = 0;
        Header m_Header = new Header();
        using (var reader = new BinaryReader(new FileStream(AssetPath, FileMode.Open)))
        {
            string signature = reader.ReadStringToNull();
            if (signature != "UnityFS")
            {
                MessageBox(IntPtr.Zero, AssetPath + "文件不为AssetBundle！", "错误", 0x00000010);
                return null;
            }
            m_Header.signature = signature;
            m_Header.version = reader.ReadUInt32();
            m_Header.unityVersion = reader.ReadByteToNull();
            m_Header.unityRevision = reader.ReadByteToNull();
            pos = (int)reader.BaseStream.Position;
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
            buf = new byte[reader.BaseStream.Length];
            var len = (int)reader.BaseStream.Length;
            reader.Read(buf, 0, len);
        }
        int idx = IndexOf(buf, m_Header.unityRevision, pos);
        if (buf[idx + m_Header.unityRevision.Length + 1] == 0xd)
        {
            buf[idx + m_Header.unityRevision.Length + 1] = 0x13;
        }
        if (buf[idx + m_Header.unityRevision.Length + 1] != 0x13)
        {
            MessageBox(IntPtr.Zero, AssetPath + "为未知平台！", "错误", 0x00000010);
            return null;
        }
        return AssetBundle.LoadFromMemory(buf);
    }
    public void ClearObj()
    {
        while(Live2dObj.Count > 0)
        {
            GameObject obj = Live2dObj.Pop();
            Destroy(obj);
        }
    }

    public void Test()
    {
        Debug.Log("test");
    }
    public void ResetObj()
    {
        ClearObj();
        GameObject gob = Instantiate(NowPrefab, pos.position, pos.rotation);
        List<UITreeData> MyTree = new List<UITreeData>();
        GameObjRecursive(gob, MyTree);
        UITree.Inject(MyTree);
        Live2dObj.Push(gob);
        if ((this.transform.Find("GenGif").GetComponent<Toggle>().isOn || this.transform.Find("GenPng").GetComponent<Toggle>().isOn) && index == -1)
        {
            index = 0;
        }
    }
}
