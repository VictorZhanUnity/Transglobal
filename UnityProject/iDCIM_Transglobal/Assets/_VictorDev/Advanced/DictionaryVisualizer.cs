using System;
using System.Collections.Generic;

namespace VictorDev.Advanced
{
    /// <summary>
    /// 字典視覺化，T為內容值，可以為string，也可以為字典
    /// <para>+ 利用泛型，達到巢狀資料結的存儲</para>
    /// <para>+ 單一Dictionary資料</para>
    /// </summary>
    [Serializable]
    public class DictionaryVisualizer<K, V>
    {
        public K key;
        public V value;

        public DictionaryVisualizer(K key, V value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// 解析JSON字典資料
        /// </summary>
        public static List<DictionaryVisualizer<K, V>> Parse(Dictionary<K, V> jsonDict)
        {
            List<DictionaryVisualizer<K, V>> result = new List<DictionaryVisualizer<K, V>>();

            foreach (var data in jsonDict)
            {
                result.Add(new DictionaryVisualizer<K, V>(data.Key, data.Value));
            }
            return result;
        }
    }

    /// /// <summary>
    /// 字典視覺化，T為內容值，可以為string，也可以為字典
    /// <para>+ 利用泛型，達到巢狀資料結的存儲</para>
    /// <para>+ 多數Dictionary資料(List)</para>
    /// </summary>
    [Serializable]
    public class DictionaryVisualizerListItem<K, V>
    {
        public List<DictionaryVisualizer<K, V>> dictionaryVisualizerList;

        /// <summary>
        /// 解析JSON字典資料
        /// </summary>
        public static List<DictionaryVisualizerListItem<K, V>> Parse(List<Dictionary<K, V>> jsonDictList)
        {
            List<DictionaryVisualizerListItem<K, V>> result = new List<DictionaryVisualizerListItem<K, V>>();

            jsonDictList.ForEach(item =>
            {
                DictionaryVisualizerListItem<K, V> resultItem = new DictionaryVisualizerListItem<K, V>();

                List<DictionaryVisualizer<K, V>> list = new List<DictionaryVisualizer<K, V>>();
                foreach (var data in item)
                {
                    list.Add(new DictionaryVisualizer<K, V>(data.Key, data.Value));
                }
                resultItem.dictionaryVisualizerList = list;
                result.Add(resultItem);
            });
            return result;
        }
    }
}
