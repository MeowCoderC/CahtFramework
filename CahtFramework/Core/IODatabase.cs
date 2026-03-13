namespace CahtFramework
{
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;

    [CreateAssetMenu(menuName = "IODatabase")]
    public class IODatabase : ScriptableObject
    {
        [SerializeField] private List<IdentifiedObject> datas = new();

        public IReadOnlyList<IdentifiedObject> Datas => this.datas;
        public int                             Count => this.datas.Count;

        public IdentifiedObject this[int index] => this.datas[index];

        private void SetID(IdentifiedObject target, int id)
        {
            var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(target, id);
#if UNITY_EDITOR
            EditorUtility.SetDirty(target);
#endif
        }

        private void ReorderDatas()
        {
            var field = typeof(IdentifiedObject).GetField("id", BindingFlags.NonPublic | BindingFlags.Instance);
            for (var i = 0; i < this.datas.Count; i++)
            {
                field.SetValue(this.datas[i], i);
#if UNITY_EDITOR
                EditorUtility.SetDirty(this.datas[i]);
#endif
            }
        }

        public void Add(IdentifiedObject newData)
        {
            this.datas.Add(newData);
            this.SetID(newData, this.datas.Count - 1);
        }

        public void Remove(IdentifiedObject data)
        {
            this.datas.Remove(data);
            this.ReorderDatas();
        }

        public IdentifiedObject GetDataByID(int id) { return this.datas[id]; }

        public T GetDataByID<T>(int id) where T : IdentifiedObject { return this.GetDataByID(id) as T; }

        public IdentifiedObject GetDataCodeName(string codeName) { return this.datas.Find(item => item.CodeName == codeName); }

        public T GetDataCodeName<T>(string codeName) where T : IdentifiedObject { return this.GetDataCodeName(codeName) as T; }

        public bool Contains(IdentifiedObject item) { return this.datas.Contains(item); }

        public void SortByCodeName()
        {
            this.datas.Sort((x, y) => x.CodeName.CompareTo(y.CodeName));
            this.ReorderDatas();
        }
    }
}