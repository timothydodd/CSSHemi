using System;
using System.Collections.Generic;
using System.Text;

namespace HTMLScrape
{
    public static class QueryCoreExt
    {


        public static IEnumerable<string> Query(this string data, string query)
        {


            var items = query.Split(' ');


            foreach (var item in HTMLParsing.QueryHtml(data, new CSSQuery(items[0])))
            {
                if (items.Length > 1)
                {
                    var c = ConcatString(items, 1);
                    if (string.IsNullOrWhiteSpace(c))
                    {
                        yield return item;
                    }
                    else
                    {
                        foreach (var item2 in item.Query(c))
                        {
                            yield return item2;
                        }
                    }

                }
                else
                {
                    yield return item;
                }
            }





        }



        public static IEnumerable<string> Query(this IEnumerable<string> data, string query)
        {
            var qitems = query.Split(' ');
            foreach (var d in data)
            {
              foreach (var item in HTMLParsing.QueryHtml(d, new CSSQuery(qitems[0])))
              {
                if (qitems.Length > 1)
                {
                  var c = ConcatString(qitems, 1);
                  if (string.IsNullOrWhiteSpace(c))
                  {
                    yield return item;
                  }
                  else
                  {
                    foreach (var item2 in item.Query(c))
                    {
                      yield return item2;
                    }
                  }

                }
                else
                {
                  yield return item;
                }
              }
            }



        }

        public static IEnumerable<string> RemoveTags(this IEnumerable<string> source)
        {
            foreach (var item in source)
            {
                yield return item.RemoveTags();
            }
        }

        public static string RemoveTags(this string source)
        {
            if (string.IsNullOrEmpty(source))
                return "";
            int tmp = 0;
            while (tmp >= 0)
            {
                tmp = source.IndexOf("<", tmp, StringComparison.Ordinal);
                if (tmp >= 0)
                {


                    int count = source.IndexOf(">", tmp, StringComparison.Ordinal) - tmp;


                    source = source.Remove(tmp, count + 1);
                }
                else
                {

                    break;
                }
            }
            return source;

        }
        public static string RemoveTags(this string txt, string query)
        {
            var builder = new StringBuilder();
            var hq = new CSSQuery(query);
            var index = 0;

            while (txt != null && index < txt.Length)
            {
                var elementStart = 0;
                var e = HTMLParsing.NextElement(builder, ref index, txt, out elementStart);

                if (index >= txt.Length)
                    break;
                if (e == hq.Element || string.IsNullOrEmpty(hq.Element))
                {
                    index = elementStart;
                    if (HTMLParsing.Validate(builder, ref index, txt, hq))
                    {

                        if (hq.Function != null)
                        {
                            var item = hq.Function.Post(builder, txt, ref index, e);

                            if (!string.IsNullOrEmpty(item))
                            {
                                txt = txt.Remove(elementStart, index - elementStart);
                                index = elementStart;
                            }
                            index++;
                        }
                        else
                        {
                            var subIndex = HTMLParsing.FindEndofElement(builder, txt, elementStart, e);
                            if (subIndex >= 0)
                            {

                                txt = txt.Remove(elementStart, subIndex);
                                index = elementStart;
                            }

                        }
                    }
                }
                index++;
            }
            return txt;

        }

        public static string HTML(this string txt)
        {

            var start = txt.IndexOf(">") + 1;
            var end = txt.LastIndexOf("<");

            return txt.Substring(start, end - start);


        }
        public static string RemoveWhiteSpace(this string st)
        {
            if (string.IsNullOrWhiteSpace(st))
                return "";
            return st.Trim().Replace("\t", "").Replace("\r", "").Replace("\n", "").Replace("'", "");
        }

        public static string GetAttribute(this string data, string name)
        {
            if (string.IsNullOrWhiteSpace(data))
                return "";
            return HTMLParsing.ParseAttributeValue(name, data, 0);

        }
        public static IEnumerable<string> GetAttribute(this IEnumerable<string> data, string name)
        {
            foreach (var item in data)
            {
                yield return HTMLParsing.ParseAttributeValue(name, item, 0);
            }


        }
        private static string ConcatString(string[] arry, int start)
        {
            string r = "";
            for (int i = start; i < arry.Length; i++)
            {
                r += $"{arry[i]} ";
            }
            return r;
        }
    }
}



