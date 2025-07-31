using System;
using System.Collections.Generic;

namespace Hayson.EditorKit
{
    public interface ISearcher
    {
        void Search(string query, Action<IEnumerable<string>> onCompleted);
    }
}