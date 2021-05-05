using SerialPlotter.Core.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SerialPlotter.Core
{
    public static class Helpers
    {
        public static void SortCollection<TSource, TKey>(this ObservableCollection<TSource> observableCollection, Func<TSource, TKey> keySelector)
        {
            var a = observableCollection.OrderBy(keySelector).ToList();
            observableCollection.Clear();
            foreach (var b in a)
            {
                observableCollection.Add(b);
            }
        }


    }
}
