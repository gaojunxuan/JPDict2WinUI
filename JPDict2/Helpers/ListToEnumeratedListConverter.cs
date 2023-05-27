using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Data;

namespace JPDict2.Helpers
{
    class ListToEnumeratedListConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            List<Tuple<int, object>> newList = new List<Tuple<int, object>>();
            if (value is IList list)
            {
                foreach (var item in list)
                {
                    if (item != null)
                        newList.Add(new Tuple<int, object>(list.IndexOf(item) + 1, item));
                }
                return newList;
            }
            throw new NotImplementedException();
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            List<object> newList = new List<object>();
            if (value is IList list)
            {
                foreach (var item in list)
                {
                    if (item != null)
                        newList.Add(((Tuple<int, object>)item).Item2);
                }
                return newList;
            }
            throw new NotImplementedException();
        }
    }
}
