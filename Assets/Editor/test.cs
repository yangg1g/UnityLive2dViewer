using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class test : MonoBehaviour
{
    /// <summary>
    /// ����������е�AssetBundles������
    /// </summary>
    [MenuItem("AssetBundleTools/BuildAllAssetBundles")]
    public static void BuildAllAB()
    {
        // ���AB���·��
        string strABOutPAthDir = string.Empty;

        // ��ȡ��StreamingAssets���ļ���·������һ������ļ��У����Զ��壩
        strABOutPAthDir = Application.streamingAssetsPath;

        // �ж��ļ����Ƿ���ڣ����������½�
        if (Directory.Exists(strABOutPAthDir) == false)
        {
            Directory.CreateDirectory(strABOutPAthDir);
        }

        // �������AB�� (Ŀ��ƽ̨������Ҫ���ü���)
        BuildPipeline.BuildAssetBundles(strABOutPAthDir, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);

    }

    // Start is called before the first frame update

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
