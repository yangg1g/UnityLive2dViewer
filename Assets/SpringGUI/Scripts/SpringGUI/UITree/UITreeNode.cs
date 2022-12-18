
/*=========================================
* Author: springDong
* Description: SpringGUI.UITree/TreeView.UITreeNode
* UITreeNode is equivalent to the Controller in MVC, used to responsible for UITree and UITreeData interaction
==========================================*/

using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;

namespace SpringGUI
{
    [Serializable]
    public class MyEvent : UnityEvent<string, GameObject> { }
    public class UITreeNode : UIBehaviour
    {
        #region private && public  members

        public MyEvent OnEvent;

        private UITreeData TreeData = null;
        private UITree UITree = null;
        private Toggle toggle = null;
        private Text text = null;
        private Transform _myTransform = null;
        private Transform _container = null;

        private List<GameObject> _children = new List<GameObject>();

        #endregion

        #region get && reset ui component
        
        private void getComponent( )
        {
            _myTransform = this.transform;
            
            _container = _myTransform.Find("Container");
            toggle = _container.Find("Toggle").GetComponent<Toggle>();
            text = _container.Find("Label").GetComponent<Text>();
            UITree = _myTransform.parent.parent.parent.GetComponent<UITree>();
        }
        private void resetComponent( )
        {
            _container.localPosition = new Vector3(0 , _container.localPosition.y , 0);
        }

        #endregion

        #region external call interface

        public void Inject( UITreeData data )
        {
            if ( null == _myTransform )
                getComponent();
            resetComponent();
            TreeData = data;
            text.text = data.Name;
            // toggle.isOn = false;
            // toggle.onValueChanged.AddListener(openOrClose);
            toggle.isOn = TreeData.GetTargetActive();
            toggle.onValueChanged.AddListener( (bool isOn) => { TreeData.SwitchTarget(isOn); });
            // _container.localPosition += new Vector3(_container.GetComponent<RectTransform>().sizeDelta.y * TreeData.Layer , 0 , 0);
        }

        [Obsolete("This method is replaced by Inject")]
        public void SetData( UITreeData data )
        {
            if(null == _myTransform)
                getComponent();
            resetComponent();
            TreeData = data;
            text.text = data.Name;
            //toggle.isOn = false;
            //toggle.onValueChanged.AddListener(openOrClose);
            toggle.isOn = TreeData.GetTargetActive();
            toggle.onValueChanged.AddListener((bool isOn) => { TreeData.SwitchTarget(isOn); });
            //_container.localPosition += new Vector3(_container.GetComponent<RectTransform>().sizeDelta.y * TreeData.Layer , 0 , 0);
        }

        #endregion

        #region open && close

        private void openOrClose( bool isOn )
        {
            if ( isOn ) openChildren();
            else closeChildren();
        }
        private void openChildren()
        {
            _children = UITree.pop(TreeData.ChildNodes,transform.GetSiblingIndex());
        }

        protected void closeChildren( ) 
        {
            for (int i = 0; i < _children.Count; i++)
            {
                UITreeNode node = _children[i].GetComponent<UITreeNode>();
                node.RemoveListener();
                node.closeChildren();
            }
            UITree.push(_children);
            _children = new List<GameObject>();
        }
        private void RemoveListener()
        {
            toggle.onValueChanged.RemoveListener(openOrClose);
        }

        private void NodeClick()
        {
            Debug.Log(text.text);
        }

        #endregion
    }
}