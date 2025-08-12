using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Tools
{
    /// <summary>
    /// Копирование материалов от объекта источника к объекту с потерянными материалами
    /// </summary>
    public class MaterialCopier : MonoBehaviour
    {
        [SerializeField]
        private Transform m_Source = null;

        [SerializeField]
        private Dictionary<string, Material[]> m_Materials = new Dictionary<string, Material[]>();


        [ContextMenu("Copy")]
        public void Copy()
        {
            m_Materials.Clear();
            RecursiveRead(m_Source, "");
            Debug.Log(m_Materials.Count);
            RecursiveWrite(transform,"");
        }

        private void RecursiveRead(Transform parent, string path)
        {
            Transform[] trArr = parent.GetComponentsInChildren<Transform>();

            for(int i = 0; i < trArr.Length; i++)
            {
                if (trArr[i].Equals(parent))
                    continue;
                if (!trArr[i].parent.Equals(parent))
                    continue;

                string pathNew = path + "/" + trArr[i].gameObject.name;

                MeshRenderer mr = trArr[i].GetComponent<MeshRenderer>();

                if(mr != null)
                {
                    m_Materials.Add(pathNew, mr.sharedMaterials);
                }

                if(trArr[i].childCount > 0)
                {
                    RecursiveRead(trArr[i], pathNew);
                }
            }

        }

        private void RecursiveWrite(Transform parent, string path)
        {
            Transform[] trArr = parent.GetComponentsInChildren<Transform>();

            for (int i = 0; i < trArr.Length; i++)
            {
                if (trArr[i].Equals(parent))
                    continue;
                if (!trArr[i].parent.Equals(parent))
                    continue;

                string pathNew = path + "/" + trArr[i].gameObject.name;

                MeshRenderer mr = trArr[i].GetComponent<MeshRenderer>();

                if (mr != null)
                {
                    if (m_Materials.ContainsKey(pathNew))
                    {
                        mr.sharedMaterials = m_Materials[pathNew];
                    }
                }

                if (trArr[i].childCount > 0)
                {
                    RecursiveWrite(trArr[i], pathNew);
                }
            }

        }

    }
}