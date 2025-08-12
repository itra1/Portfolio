using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Challenges.KingGameOfUr {

    public class GenerateGrid : MonoBehaviour {

        /// <summary>
        /// Префаб для генерации поля
        /// </summary>
        [SerializeField]
        private GameObject m_GeneratePrefab;

        /// <summary>
        /// Родитель поля
        /// </summary>
        [SerializeField]
        private Transform m_ParentTransform;

        /// <summary>
        /// Размер одного блока
        /// </summary>
        [SerializeField]
        private float m_BoxSize = 1.05f;

        /// <summary>
        /// Размер поля
        /// </summary>
        [SerializeField]
        private Vector2Int m_Size;

        /// <summary>
        /// Генерация
        /// </summary>
        [ContextMenu ("Generate Grid")]
        private void Generate () {

            if (m_GeneratePrefab == null) {
                Debug.LogError ("Укажите префаб");
                return;
            }
            if (m_ParentTransform == null) {
                Debug.LogError ("Укажите укажите родителя");
                return;
            }

            ClearFields ();

            float sizeX = m_Size.x * m_BoxSize;
            float sizeY = m_Size.y * m_BoxSize;

            float startXSpawn = -sizeX / 2 + m_BoxSize / 2;
            float startYSpawn = sizeY / 2 - m_BoxSize / 2;

            int index = 0;
            for (int y = 0; y < m_Size.y; y++) {
                for (int x = 0; x < m_Size.x; x++) {
                    GameObject inst = SpawnInst (startXSpawn + x * m_BoxSize, startYSpawn - y * m_BoxSize);
                    Section sc = inst.GetComponent<Section> ();
                    index++;
                }
            }

        }
        /// <summary>
        /// Генерация одного инстанта
        /// </summary>
        /// <param name="localX">Локальная координата по X</param>
        /// <param name="localZ">Локальная координата по Y</param>
        private GameObject SpawnInst (float localX, float localZ) {

            GameObject inst = Instantiate (m_GeneratePrefab, m_ParentTransform);
            inst.SetActive (true);
            inst.transform.localPosition = new Vector3 (localX, -m_BoxSize * 0.45f, localZ);
            inst.transform.rotation = Quaternion.Euler (90 * Random.Range (0, 4), 90 * Random.Range (0, 4), 90 * Random.Range (0, 4)) * Quaternion.identity;
            return inst;
        }
        /// <summary>
        /// Очистка поля
        /// </summary>
        [ContextMenu ("Clear")]
        private void ClearFields () {
            var components = m_ParentTransform.GetComponentsInChildren<Section> ();

            foreach (var elem in components) {
                DestroyImmediate (elem.gameObject);
            }
        }

    }
}