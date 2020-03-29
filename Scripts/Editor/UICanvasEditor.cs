using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Coffee.UIExtensions
{
    [CustomEditor(typeof(UICanvas))]
    public class UICanvasEditor : Editor
    {
        private Transform m_RootUI;
        private int m_iSortingOrder;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            UICanvas uicanvas = target as UICanvas;

            if (!_checkSortingOrder())
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("排序ui和特效：按层级分组对象到对应的canvas，然后设置正确的sorting order。", MessageType.Warning);
                GUILayout.BeginVertical();
                if (GUILayout.Button("确定"))
                {
                    m_RootUI = null;
                    m_iSortingOrder = 0;
                    _makeCanvasGroup(uicanvas.transform);
                    if (m_RootUI)
                    {
                        GameObject.Instantiate(m_RootUI, m_RootUI).name = m_RootUI.name;
                        foreach (Component com in m_RootUI.GetComponents<Component>())
                        {
                            if (com.GetType() == typeof(RectTransform) ||
                                com.GetType() == typeof(CanvasRenderer))
                            {
                                continue;
                            }
                            GameObject.DestroyImmediate(com);
                        }
                        GameObject.DestroyImmediate(m_RootUI.GetComponent<CanvasRenderer>());

                        m_RootUI.gameObject.name = "Canvas" + m_iSortingOrder;
                        m_RootUI.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                        m_RootUI.GetComponent<RectTransform>().localScale = Vector3.one;
                        m_RootUI.GetComponent<RectTransform>().rotation = Quaternion.identity;

                        Canvas canvas = m_RootUI.gameObject.AddComponent<Canvas>();
                        canvas.overrideSorting = true;
                        canvas.sortingOrder = m_iSortingOrder;
                        m_RootUI = null;
                    }
                }
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }
        }

        bool _checkSortingOrder()
        {
            return false;
        }

        void _makeCanvasGroup(Transform current)
        {
            /*if (!current.gameObject.activeSelf)
            {
                return;
            }*/

            if (m_RootUI)
            {
                ParticleSystem ps = current.GetComponent<ParticleSystem>();
                TrailRenderer tr = current.GetComponent<TrailRenderer>();
                if (ps || tr)
                {
                    GameObject.Instantiate(m_RootUI, m_RootUI).name = m_RootUI.name;
                    foreach (Component com in m_RootUI.GetComponents<Component>())
                    {
                        if (com.GetType() == typeof(RectTransform) ||
                            com.GetType() == typeof(CanvasRenderer))
                        {
                            continue;
                        }
                        GameObject.DestroyImmediate(com);
                    }
                    GameObject.DestroyImmediate(m_RootUI.GetComponent<CanvasRenderer>());

                    m_RootUI.name = "Canvas" + m_iSortingOrder;
                    m_RootUI.GetComponent<RectTransform>().anchoredPosition3D = Vector3.zero;
                    m_RootUI.GetComponent<RectTransform>().localScale = Vector3.one;
                    m_RootUI.GetComponent<RectTransform>().rotation = Quaternion.identity;

                    Canvas canvas = m_RootUI.gameObject.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = m_iSortingOrder;
                    m_RootUI = null;

                    ++m_iSortingOrder;
                    if (ps)
                    {
                        ps.GetComponent<ParticleSystemRenderer>().sortingOrder = m_iSortingOrder;
                    }
                    if (tr)
                    {
                        tr.sortingOrder = m_iSortingOrder;
                    }
                    ++m_iSortingOrder;
                }
            }
            else
            {
                if (current.GetComponent<CanvasRenderer>())
                {
                    m_RootUI = current;
                }
            }

            for (int i = 0; i < current.childCount; ++i)
            {
                _makeCanvasGroup(current.GetChild(i));
            }
        }
    }
}