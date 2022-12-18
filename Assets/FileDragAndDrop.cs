using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using B83.Win32;
using UnityEngine.Events;

#if  UNITY_EDITOR_WIN
using UnityEditor;
#endif

public class FileDragAndDrop : MonoBehaviour
{
    public UnityEvent m_onDropDetected;
    public string[] m_filesDropped;


    private string[] m_currentPathsHold;

    public void Update()
    {

#if  UNITY_EDITOR_WIN
        string[] paths = DragAndDrop.paths;

        if (paths != null && m_currentPathsHold != null && paths.Length == 0 && m_currentPathsHold.Length > 0)
        {

            m_filesDropped = m_currentPathsHold;
            m_onDropDetected.Invoke();
        }
        m_currentPathsHold = paths;
#endif
    }

    void Awake()
    {
        UnityDragAndDropHook.InstallHook();
        UnityDragAndDropHook.OnDroppedFiles += OnFiles;
    }
    void OnDestroy()
    {
        UnityDragAndDropHook.UninstallHook();
    }

    void OnFiles(List<string> aFiles, POINT aPos)
    {
        m_filesDropped = aFiles.ToArray();
        m_onDropDetected.Invoke();
    }
}