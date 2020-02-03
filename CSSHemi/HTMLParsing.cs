using System;
using System.Collections.Generic;
using System.Text;

namespace HTMLScrape
{
    public class HTMLParsing
    {
        //todo - more selector support
        //todo - fix attribute parsing
        public static IEnumerable<string> QueryHtml(string txt, CSSQuery query,bool firstlevel = false)
        {
            var builder = new StringBuilder();

            var index = 0;

            while (txt != null && index < txt.Length)
            {
                var elementStart = 0;
                var e = NextElement(builder, ref index, txt, out elementStart);

                if (index >= txt.Length)
                    break;
                if (e.Equals(query.Element,StringComparison.InvariantCultureIgnoreCase) || string.IsNullOrEmpty(query.Element))
                {
                    if (Validate(builder,ref index,txt,query))
                    {
                        index = elementStart;
                        if (query.Function != null)
                        {
                            var item = query.Function.Post(builder, txt, ref index, e);

                            if (!string.IsNullOrEmpty(item))
                                yield return item;
                          
                        }
                        else
                        {

                            yield return ParseElement(builder, txt, ref index, e,firstlevel);
                        }
                       
                        //if (firstlevel)
                        //{
                        //  index =  FindEndofElement(builder, txt, index, query.Element);
                        //}
                        index++;
                    }
                }
            }
        }

        public static bool Validate(StringBuilder builder,ref int index,string data, CSSQuery query)
        {
            var attributes = GetAttributes(builder, ref index, data);

            var allFound = true;
            foreach (var qv in query.Values)
            {
                allFound = false;
                foreach (var ea in attributes)
                {
                    if (qv.Name.Equals(ea.Name, StringComparison.InvariantCultureIgnoreCase))
                    {
                        allFound = true;
                        if (!string.IsNullOrWhiteSpace(qv.Value))
                        {
                            allFound = false;

                            if (!string.IsNullOrWhiteSpace(ea.Value))
                            {
                                var id1 = ea.Value.IndexOf(qv.Value, StringComparison.InvariantCulture);
                                if (id1 >= 0)
                                {
                                    var ch3 = ea.Value[id1 - 1];
                                    if (ch3 == ' ' || ch3 == '\'' || ch3 == '"')
                                    {
                                        var lastPlace = id1 + qv.Value.Length;
                                        if (lastPlace >= ea.Value.Length)
                                        {



                                            allFound = true;
                                        }
                                        else
                                        {
                                            var c2 = ea.Value[lastPlace];
                                            if (c2 == ' ' || c2 == '\'' || c2 == '"')
                                                allFound = true;
                                        }
                                    }



                                }
                            }
                        }
                    }
                }
                if (!allFound)
                    break;
            }
            return allFound;
        }
        public static string NextElement(StringBuilder builder, ref int index, string data, out int elementStart)
        {
            var started = false;
            builder.Clear();
            elementStart = index;


            while (index < data.Length)
            {
                var chr = data[index];
                //skip quotes
                if (chr == '<')
                {
                    builder.Clear();
                    elementStart = index;
                    index++;
                    while (index < data.Length)
                    {
                        chr = data[index];
                        if (chr == ' ' || chr == '>')
                        {
                            break;
                        }
                        builder.Append(chr);
                        index++;
                    }
                    var word = builder.ToString();
                    if (word != "!--")
                    {
                        return word;
                    }
                    index = data.IndexOf("-->", index, StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    index++;
                }
            }
            return "";
        }

 

        public static List<ElementAttribute> GetAttributes(StringBuilder builder, ref int index, string data)
        {
            var attributes = new List<ElementAttribute>();

            ElementAttribute current = null;
            var started = false;
            var attribute = true;
            var ignoreBreaks = false;
            var quote = ' ';
            builder.Clear();
            while (index < data.Length)
            {
                var chr = data[index];
                if (ignoreBreaks)
                {
                    if (chr == quote)
                    {
                        ignoreBreaks = false;
                    }
                }
                else
                {
                    if (chr == '\'' || chr == '"')
                    {
                        ignoreBreaks = true;
                        quote = chr;
                    }
                }
                if (!ignoreBreaks && (char.IsWhiteSpace(chr) || chr == '=' || chr == '>'))
                {
                    if (started)
                    {
                      
                        if (attribute && GetNextLetter(index, data) == '"')
                        {
                            current = new ElementAttribute();
                            current.Name = builder.ToString();
                            attribute = false;
                        }
                        else
                        {
                            if (current == null)
                                current = new ElementAttribute();
                            current.Value = builder.ToString();
                            attribute = true;
                            attributes.Add(current);

                            if (chr == '>')
                                return attributes;
                            current = null;
                        }
                        builder.Clear();
                    }
                }
                else
                {
                    if (!started)
                    {
                        started = true;
                    }

                    builder.Append(chr);
      
                }
                index++;
                if (chr == '>')
                {
                   
                    return attributes;
                }
            }
         
            return attributes;
        }

        private static char GetNextLetter(int index, string data)
        {
            index++;
            while (index < data.Length)
            {
                if (!char.IsWhiteSpace(data[index]))
                {
                    return data[index];
                }
                index++;
            }

            return ' ';
        }

        public static int FindEndofElement(StringBuilder builder, string txt, int index, string element)
        {
            var escape = $"/{element}";
            var counter = 0;
            var start = index;
            var eOut = 0;
            index += 1;
            if (element.Equals("img", StringComparison.InvariantCultureIgnoreCase))
            {
                index = txt.IndexOf(">", start, StringComparison.InvariantCultureIgnoreCase);

                return index + 1 - start;
            }
            while (index < txt.Length)
            {
                var tag = NextElement(builder, ref index, txt, out eOut);


                if (tag.Equals(escape, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (counter <= 0)
                    {
                        return index + 1 - start;
                    }
                    counter--;
                }
                else if (tag.Equals(element, StringComparison.InvariantCultureIgnoreCase))
                    counter++;
            }
            return -1;
        }
        public static string ParseElement(StringBuilder builder, string txt, ref int index, string element,bool startAtEnd = false)
        {
            var start = index;
   
            var endTag = FindEndofElement(builder, txt, index, element);
            if (endTag >= 0)
            {
                if (startAtEnd)
                {
                    index += endTag +1;
                }
                else
                {
                    index += 1;
                }

                return txt.Substring(start, endTag);
            }

            return "";
        }


        public static string ParseAttributeValue(string name, string txt, int startIndex)
        {
            try
            {
                var builder = new StringBuilder();
                var recording = false;
                var doubles = false;

                var i = txt.IndexOf(name, startIndex, StringComparison.InvariantCultureIgnoreCase);
                while (i >= 0 && i < txt.Length)
                {
                    var character = txt[i];

                    if (character == '"')
                    {
                        if (recording && doubles)
                            break;
                        doubles = true;
                        recording = true;
                    }
                    else if (character == '\'')
                    {
                        if (recording && !doubles)
                            break;
                        doubles = false;
                        recording = true;
                    }
                    else if (recording)
                    {
                        builder.Append(character);
                    }


                    i++;
                }


                return builder.ToString();
            }
            catch
            {
                return "";
            }
        }

        public static int FindIgnoreCase( string data, string what,int start,int end)
        {
            end -= (what.Length);
            while (start <= end)
            {

                bool found = true;
                for (int i = 0; i < what.Length; i++)
                {

                    if (char.ToUpperInvariant(data[start + i]) != char.ToUpperInvariant(what[i]))
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                {
                    return start;
                }
                start++;
            }

            return -1;
        }
    }
}