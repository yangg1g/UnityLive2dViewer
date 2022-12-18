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
            if(smr || sr || ps)
            {
                Tree.Add(new UITreeData(child.name, child.gameObject));
            }
            PrefabRecursive(child.gameObject, map, Tree);
        }
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

    private Stack<GameObject> Live2dObj = new Stack<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        AssetPathInput.text = @"E:\Documents\Tencent Files\1094766238\FileRecv\__data(2)";
        Default_Particle = pos.gameObject.GetComponent<ParticleSystem>().GetComponent<Renderer>().sharedMaterial;
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;

        var data = new UITreeData("SpringGUI", new List<UITreeData>()
        {
            new UITreeData("Button",new List<UITreeData>()
            {
                new UITreeData("DoubleClickButton"),
                new UITreeData("LongClickButton")
            }),
            new UITreeData("Pie"),
            new UITreeData("DatePicker"),
            new UITreeData("C#",new List<UITreeData>()
            {
                new UITreeData("high-level syntax",new List<UITreeData>()
                {
                    new UITreeData("Action",new List<UITreeData>()
                        {
                            new UITreeData("One parameter"),
                            new UITreeData("Two parameter"),
                            new UITreeData("Three parameter"),
                            new UITreeData("Four parameter"),
                            new UITreeData("Five parameter")
                        }),
                    new UITreeData("Func"),
                    new UITreeData("delegate")
                }),
                new UITreeData("Reflect")
            })
        });
        //UITree.SetData(data);
        // UITree.Inject(data);
    }

    void OnDisable()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        // do something with the dropped file names. aPos will contain the 
        // mouse position within the window where the files has been dropped.
        AssetPathInput.text = aFiles[0];
    }

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
        GameObject prefab = ab.LoadAsset<GameObject>(ab_path[0]);
        GameObject gob = Instantiate(prefab, pos.position, pos.rotation);
        List<UITreeData> MyTree = new List<UITreeData>();
        PrefabRecursive(gob, TextureMap, MyTree);
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
}
