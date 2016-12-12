using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HTMLScrape
{
    public class HtmlQuery
    {
        public string Element { get; set; }
        public List<ElementAttribute> Values { get; set; }
        public QueryFunction Function;

        public HtmlQuery(string query)
        {

            ParseQuery(query);
        }

        public void ParseQuery(string query)
        {
            query = query.Trim();
            Values = new List<ElementAttribute>();

            int i = 0;
            string value = "";
            string attribute = "";

            while (i < query.Length)
            {
                var chr = query[i];
                if (chr == '.' || chr == ' ' || chr == '#' || i == query.Length - 1 || chr == '[' || chr == ':')
                {
                    if (i == query.Length - 1)
                        value += chr;
                    if (!string.IsNullOrEmpty(value))
                    {
                        if (string.IsNullOrEmpty(attribute))
                        {
                            if (string.IsNullOrEmpty(Element))
                                Element = value;
                        }
                        else
                        {
                            Values.Add(new ElementAttribute() {Name = attribute, Value = value});

                        }

                    }
                    if (i == query.Length - 1)
                        return;
                    if (chr == '.')
                        attribute = "class";
                    else if (chr == '#')
                        attribute = "id";
                    else if (chr == ':')
                    {
                        int x = i + 1;
                        while (x < query.Length)
                        {
                            var chr2 = query[x];
                            if (chr2 == '(' || chr2 == ')')
                            {
                                x++;
                                value = "";
                                break;
                            }
                            attribute += chr2;
                            x++;
                        }
                        if (query[x - 1] == '(')
                        {

                            while (x < query.Length)
                            {
                                var chr2 = query[x];
                                if (chr2 == ')')
                                {

                                    x++;
                                    break;
                                }
                                if (chr2 != '\'' && chr2 != '"')
                                    value += chr2;
                                x++;
                            }

                        }

                        Function = QueryFunction.GetFunction(attribute, value);
                        attribute = "";
                        value = "";
                        i = x;
                        continue;
                    }
                    else if (chr == '[')
                    {
                        int x = i + 1;
                        while (x < query.Length)
                        {
                            var chr2 = query[x];
                            if (chr2 == '=' || chr2 == ']')
                            {
                                x++;
                                value = "";
                                break;
                            }
                            attribute += chr2;
                            x++;
                        }
                        if (query[x - 1] == '=')
                        {

                            while (x < query.Length)
                            {
                                var chr2 = query[x];
                                if (chr2 == ']')
                                {

                                    x++;
                                    break;
                                }
                                if (chr2 != '\'' && chr2 != '"')
                                    value += chr2;
                                x++;
                            }

                        }
                        Values.Add(new ElementAttribute() {Name = attribute, Value = value});
                        attribute = "";
                        value = "";
                        i = x;
                        continue;
                    }
                    else
                    {
                        throw new Exception("Invalid query");
                    }
                    if (chr == ' ')
                    {

                        return;
                    }
                    value = "";

                }

                else
                {
                    value += chr;
                }
                i++;
            }

        }
    }

    public class ElementAttribute
    {
        public static ElementAttribute Empty = new ElementAttribute();
        public string Name { get; set; }
        public string Value { get; set; }

        public override string ToString()
        {
            return $"{Name}:{Value}";
        }
    }

    public class QueryFunction
    {
        public string Type;
        public object Parameter;



        public static QueryFunction GetFunction(string s, string v)
        {
            switch (s)
            {
                case "first":
                    return new QueryFunction() {Type = s};
                case "contains":
                {

                    if (v != null)
                        return new QueryFunction() {Type = s, Parameter = v};
                }
                    break;
                case "eq":
                {
                    int x = 0;
                    if (int.TryParse(v, out x))
                        return new QueryFunction() {Type = s, Parameter = x};
                    ;

                }
                    break;
                case "not":
                {
                    if (v != null)
                    {
                        var q1 = new HtmlQuery(v);

                        return new QueryFunction() {Type = s, Parameter = q1};
                    }
                }
                    break;
            }
            return null;
        }

        public string Post(StringBuilder builder, string txt, ref int index, string element)
        {
            switch (Type)
            {
                case "eq":
                {
                    int i = (int) Parameter;
                    i--;
                    Parameter = i;
                    if (i <= 0)
                    {
                        var html = HTMLParsing.ParseElement(builder, txt, ref index, element);
                        index = txt.Length;
                        return html;
                    }
                    else
                    {
                        return null;
                    }
                }
                    break;
                case "first":
                {
                    var html = HTMLParsing.ParseElement(builder, txt, ref index, element);
                    index = txt.Length;
                    return html;
                }
                case "contains":
                {
                    var end = index + HTMLParsing.FindEndofElement(builder, txt, index, element);
                    var start = HTMLParsing.FindIgnoreCase(txt, ">", index, end);
                    if (HTMLParsing.FindIgnoreCase(txt, (string) Parameter, start, end) >= 0)
                    {


                        var html = HTMLParsing.ParseElement(builder, txt, ref index, element);
                        index = end;
                        return html;
                    }
                    else
                    {
                        index = index + end;
                    }
                    return null;
                }
                case "not":
                {
                    var q1 = Parameter as HtmlQuery;

                    var elementStart = index;
                    if (!HTMLParsing.Validate(builder, ref index, txt, q1))
                    {
                        index = elementStart;
                        return HTMLParsing.ParseElement(builder, txt, ref index, element);
                    }

                }
                    break;

            }


            return null;
        }
    }

}
