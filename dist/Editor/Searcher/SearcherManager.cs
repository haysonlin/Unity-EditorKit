using UnityEditor;
using UnityEngine;

namespace Hayson.EditorKit
{
    public sealed class SearcherManager
    {
        static ISearcher Searcher = null;

        SearcherManager() { }

        [InitializeOnLoadMethod]
        static void ValidateSearcher()
        {
            if (Searcher == null)
            {
                Debug.LogError("EditorKit Searcher is unavailable. Please implement ISearcher interface in your project.");
            }
        }

        public static void SetSearcher(ISearcher searcher)
        {
            if (Searcher != null)
            {
                Debug.LogError("EditorKit Searcher is already set.");
                return;
            }
            Searcher = searcher;
        }

        public static ISearcher GetSearcher()
        {
            return Searcher;
        }
    }
}