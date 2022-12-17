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
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }

    void Recursive(GameObject parentGameObject, Texture2D tex = null)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            SkinnedMeshRenderer smr = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            if (smr && smr.sharedMaterial.shader.name != "Sprites/Default")
            {
                if(tex)
                {
                    smr.sharedMaterial.SetTexture("_MainTex", tex);
                }
                // Debug.Log(smr.sharedMaterial.GetTexture("_MainTex"));
                smr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (tex)
                {
                    sr.sharedMaterial.SetTexture("_MainTex", tex);
                }
                // Texture test = sr.sprite.texture;
                // test = tex;
                if (sr.sprite && tex)
                {
                    sr.sprite = Sprite.Create(tex, sr.sprite.rect, new Vector2(0.5f, 0.5f));
                    // sr.sprite = Sprite.Create(tex, sr.sprite.rect, sr.sprite.pivot);
                }
                sr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            Recursive(child.gameObject, tex);
        }
    }

    void RecursiveV2(GameObject parentGameObject, Dictionary<string, Texture2D> map)
    {
        foreach (Transform child in parentGameObject.transform)
        {
            SkinnedMeshRenderer smr = child.gameObject.GetComponent<SkinnedMeshRenderer>();
            Texture2D tmp;
            if (smr && smr.sharedMaterial.shader.name != "Sprites/Default")
            {
                if (smr.sharedMaterial.GetTexture("_MainTex") && map.TryGetValue(smr.sharedMaterial.GetTexture("_MainTex").name, out tmp))
                {
                    smr.sharedMaterial.SetTexture("_MainTex", tmp);
                }
                Debug.Log(smr.sharedMaterial.GetTexture("_MainTex"));
                smr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            SpriteRenderer sr = child.gameObject.GetComponent<SpriteRenderer>();
            if (sr)
            {
                if (sr.sharedMaterial.GetTexture("_MainTex") && map.TryGetValue(sr.sharedMaterial.GetTexture("_MainTex").name, out tmp))
                {
                    sr.sharedMaterial.SetTexture("_MainTex", tmp);
                }
                Debug.Log(sr.sharedMaterial.GetTexture("_MainTex"));
                // Texture test = sr.sprite.texture;
                // test = tex;
                if (sr.sprite && map.TryGetValue(sr.sprite.texture.name, out tmp))
                {
                    sr.sprite = Sprite.Create(tmp, sr.sprite.rect, new Vector2(0.5f, 0.5f));
                    // sr.sprite = Sprite.Create(tex, sr.sprite.rect, sr.sprite.pivot);
                }
                sr.sharedMaterial.shader = Shader.Find("Sprites/Default");
            }
            RecursiveV2(child.gameObject, map);
        }
    }

    List<string> GetAllFileNames(string path, string pattern = "*")
    {
        List<FileInfo> folder = new DirectoryInfo(path).GetFiles(pattern).ToList();

        return folder.Select(x => x.FullName).ToList();
    }


    public InputField FolderPathInput;
    public Transform pos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PreView()
    {
        List<string> AssetPaths = new List<string>();
        Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();
        foreach (string file in GetAllFileNames(FolderPathInput.text))
        {
            if (Path.GetExtension(file) == ".png")
            {
                TextureMap.Add(Path.GetFileNameWithoutExtension(file), LoadPNG(file));
            }
            else
            {
                AssetPaths.Add(file);
            }
        }
        foreach (string AssetPath in AssetPaths)
        {
            AssetBundle ab = AssetBundle.LoadFromFile(AssetPath);
            Debug.Log(ab);
            if (!ab)
            {
                MessageBox(IntPtr.Zero, "AssetBundle文件加载失败，可能是平台不对，请转换成windows平台再进行尝试！", "错误", 0x00000010);
                return;
            }
            string[] ab_path = ab.GetAllAssetNames();
            if (ab_path.Length > 1)
            {
                MessageBox(IntPtr.Zero, "该包可能包含多个模型，此情况暂未处理！", "错误", 0x00000010);
                return;
            }
            GameObject prefab = ab.LoadAsset<GameObject>(ab_path[0]);
            RecursiveV2(prefab, TextureMap);
            GameObject gob = Instantiate(prefab, pos.position, pos.rotation);
        }
    }

    public void AndroidToWin64()
    {
        List<string> AssetPaths = new List<string>();
        Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();
        foreach (string file in GetAllFileNames(FolderPathInput.text))
        {
            if (Path.GetExtension(file) == ".png")
            {
                TextureMap.Add(Path.GetFileNameWithoutExtension(file), LoadPNG(file));
            }
            else
            {
                AssetPaths.Add(file);
            }
        }
        foreach (string AssetPath in AssetPaths)
        {
            byte[] buf;
            int pos = 0;
            Header m_Header = new Header();
            using (var reader = new BinaryReader(new FileStream(AssetPath, FileMode.Open)))
            {
                string signature = reader.ReadStringToNull();
                if (signature != "UnityFS")
                {
                    MessageBox(IntPtr.Zero, AssetPath + "文件不为AssetBundle！", "错误", 0x00000010);
                    continue;
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
            else
            {
                MessageBox(IntPtr.Zero, AssetPath + "不为Android平台！", "错误", 0x00000010);
                continue;
            }
            using (var fs = new FileStream(AssetPath, FileMode.Create))
            {
                fs.Write(buf, 0, buf.Length);
            }
            MessageBox(IntPtr.Zero, AssetPath + "成功转换为Win64平台！", "信息", 0);
        }
    }

    public void Win64ToAndroid()
    {
        List<string> AssetPaths = new List<string>();
        Dictionary<string, Texture2D> TextureMap = new Dictionary<string, Texture2D>();
        foreach (string file in GetAllFileNames(FolderPathInput.text))
        {
            if (Path.GetExtension(file) == ".png")
            {
                TextureMap.Add(Path.GetFileNameWithoutExtension(file), LoadPNG(file));
            }
            else
            {
                AssetPaths.Add(file);
            }
        }
        foreach (string AssetPath in AssetPaths)
        {
            byte[] buf;
            int pos = 0;
            Header m_Header = new Header();
            using (var reader = new BinaryReader(new FileStream(AssetPath, FileMode.Open)))
            {
                string signature = reader.ReadStringToNull();
                if (signature != "UnityFS")
                {
                    MessageBox(IntPtr.Zero, AssetPath + "文件不为AssetBundle！", "错误", 0x00000010);
                    continue;
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
            else
            {
                MessageBox(IntPtr.Zero, AssetPath + "不为Win64平台！", "错误", 0x00000010);
                continue;
            }
            using (var fs = new FileStream(AssetPath, FileMode.Create))
            {
                fs.Write(buf, 0, buf.Length);
            }
            MessageBox(IntPtr.Zero, AssetPath + "成功转换为Android平台！", "信息", 0);
        }
    }
}
