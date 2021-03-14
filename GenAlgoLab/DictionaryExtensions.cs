using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenAlgoLab
{
    public static class DictionaryExtensions
    {
        public static TVal GetTupleKey<TVal, TK1, TK2>(this Dictionary<Tuple<TK1, TK2>, TVal> dict, TK1 key1, TK2 key2)
        {
            var key = new Tuple<TK1, TK2>(key1, key2);
            return dict[key];
        }
        public static bool RemoveTupleKey<TVal, TK1, TK2>(this Dictionary<Tuple<TK1, TK2>, TVal> dict, TK1 key1, TK2 key2)
        {
            var key = new Tuple<TK1, TK2>(key1, key2);
            return dict.Remove(key);
        }
    }
}
