using System.Collections.Generic;

namespace SetlistLoader
{
    public static class Extensions
    {
        public static string SetSafeQuotes(this string str)
        {
            return str.Replace("'", "''");
        }

        public static List<List<Concert>> GetListOfLists(this List<Concert> all, int batchsize)
        {
            List<List<Concert>> lol = new List<List<Concert>>();
            var total = all.Count;
            while (total > 0)
            {
                var slicesize = total > batchsize ? batchsize : total;
                var listslice = all.GetRange(0, slicesize);
                all.RemoveRange(0, slicesize);
                lol.Add(listslice);
                total = all.Count;
            }
            return lol;
        }
    }
}
