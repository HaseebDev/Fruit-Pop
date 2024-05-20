/* * * * *
 * A simple JSON Parser / builder
 * ------------------------------
 * 
 * It mainly has been written as a simple JSON parser. It can build a JSON string
 * from the node-tree, or generate a node tree from any valid JSON string.
 * 
 * Written by Bunny83 
 * 2012-06-09
 * 
 * Changelog now external. See Changelog.txt
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2019 Markus Göbel (Bunny83)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 * 
 * * * * */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace DTT.DailyRewards
{
    internal enum JsonNodeType
    {
        ARRAY = 1,
        OBJECT = 2,
        STRING = 3,
        NUMBER = 4,
        NULL_VALUE = 5,
        BOOLEAN = 6,
        NONE = 7,
        CUSTOM = 0xFF,
    }
    internal enum JsonTextMode
    {
        COMPACT,
        INDENT
    }

    internal abstract partial class JsonNode
    {
        #region Enumerators
        public struct Enumerator
        {
            private enum Type { NONE, ARRAY, OBJECT }
            private Type _type;
            private Dictionary<string, JsonNode>.Enumerator _mObject;
            private List<JsonNode>.Enumerator _mArray;
            public bool IsValid { get { return _type != Type.NONE; } }
            public Enumerator(List<JsonNode>.Enumerator aArrayEnum)
            {
                _type = Type.ARRAY;
                _mObject = default(Dictionary<string, JsonNode>.Enumerator);
                _mArray = aArrayEnum;
            }
            public Enumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum)
            {
                _type = Type.OBJECT;
                _mObject = aDictEnum;
                _mArray = default(List<JsonNode>.Enumerator);
            }
            public KeyValuePair<string, JsonNode> Current
            {
                get
                {
                    if (_type == Type.ARRAY)
                        return new KeyValuePair<string, JsonNode>(string.Empty, _mArray.Current);
                    else if (_type == Type.OBJECT)
                        return _mObject.Current;
                    return new KeyValuePair<string, JsonNode>(string.Empty, null);
                }
            }
            public bool MoveNext()
            {
                if (_type == Type.ARRAY)
                    return _mArray.MoveNext();
                else if (_type == Type.OBJECT)
                    return _mObject.MoveNext();
                return false;
            }
        }
        public struct ValueEnumerator
        {
            private Enumerator _mEnumerator;
            public ValueEnumerator(List<JsonNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public ValueEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public ValueEnumerator(Enumerator aEnumerator) { _mEnumerator = aEnumerator; }
            public JsonNode Current { get { return _mEnumerator.Current.Value; } }
            public bool MoveNext() { return _mEnumerator.MoveNext(); }
            public ValueEnumerator GetEnumerator() { return this; }
        }
        public struct KeyEnumerator
        {
            private Enumerator _mEnumerator;
            public KeyEnumerator(List<JsonNode>.Enumerator aArrayEnum) : this(new Enumerator(aArrayEnum)) { }
            public KeyEnumerator(Dictionary<string, JsonNode>.Enumerator aDictEnum) : this(new Enumerator(aDictEnum)) { }
            public KeyEnumerator(Enumerator aEnumerator) { _mEnumerator = aEnumerator; }
            public string Current { get { return _mEnumerator.Current.Key; } }
            public bool MoveNext() { return _mEnumerator.MoveNext(); }
            public KeyEnumerator GetEnumerator() { return this; }
        }

        public class LinqEnumerator : IEnumerator<KeyValuePair<string, JsonNode>>, IEnumerable<KeyValuePair<string, JsonNode>>
        {
            private JsonNode _mNode;
            private Enumerator _mEnumerator;
            internal LinqEnumerator(JsonNode aNode)
            {
                _mNode = aNode;
                if (_mNode != null)
                    _mEnumerator = _mNode.GetEnumerator();
            }
            public KeyValuePair<string, JsonNode> Current { get { return _mEnumerator.Current; } }
            object IEnumerator.Current { get { return _mEnumerator.Current; } }
            public bool MoveNext() { return _mEnumerator.MoveNext(); }

            public void Dispose()
            {
                _mNode = null;
                _mEnumerator = new Enumerator();
            }

            public IEnumerator<KeyValuePair<string, JsonNode>> GetEnumerator()
            {
                return new LinqEnumerator(_mNode);
            }

            public void Reset()
            {
                if (_mNode != null)
                    _mEnumerator = _mNode.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new LinqEnumerator(_mNode);
            }
        }

        #endregion Enumerators

        #region common interface

        public static bool ForceAscii = false; // Use Unicode by default
        public static bool LongAsString = false; // lazy creator creates a JSONString instead of JSONNumber
        public static bool AllowLineComments = true; // allow "//"-style comments at the end of a line

        public abstract JsonNodeType Tag { get; }

        public virtual JsonNode this[int aIndex] { get { return null; } set { } }

        public virtual JsonNode this[string aKey] { get { return null; } set { } }

        public virtual string Value { get { return ""; } set { } }

        public virtual int Count { get { return 0; } }

        public virtual bool IsNumber { get { return false; } }
        public virtual bool IsString { get { return false; } }
        public virtual bool IsBoolean { get { return false; } }
        public virtual bool IsNull { get { return false; } }
        public virtual bool IsArray { get { return false; } }
        public virtual bool IsObject { get { return false; } }

        public virtual bool Inline { get { return false; } set { } }

        public virtual void Add(string aKey, JsonNode aItem)
        {
        }
        public virtual void Add(JsonNode aItem)
        {
            Add("", aItem);
        }

        public virtual JsonNode Remove(string aKey)
        {
            return null;
        }

        public virtual JsonNode Remove(int aIndex)
        {
            return null;
        }

        public virtual JsonNode Remove(JsonNode aNode)
        {
            return aNode;
        }
        public virtual void Clear() { }

        public virtual JsonNode Clone()
        {
            return null;
        }

        public virtual IEnumerable<JsonNode> Children
        {
            get
            {
                yield break;
            }
        }

        public IEnumerable<JsonNode> DeepChildren
        {
            get
            {
                foreach (var c in Children)
                    foreach (var d in c.DeepChildren)
                        yield return d;
            }
        }

        public virtual bool HasKey(string aKey)
        {
            return false;
        }

        public virtual JsonNode GetValueOrDefault(string aKey, JsonNode aDefault)
        {
            return aDefault;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, 0, JsonTextMode.COMPACT);
            return sb.ToString();
        }

        public virtual string ToString(int aIndent)
        {
            StringBuilder sb = new StringBuilder();
            WriteToStringBuilder(sb, 0, aIndent, JsonTextMode.INDENT);
            return sb.ToString();
        }
        internal abstract void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode);

        public abstract Enumerator GetEnumerator();
        public IEnumerable<KeyValuePair<string, JsonNode>> Linq { get { return new LinqEnumerator(this); } }
        public KeyEnumerator Keys { get { return new KeyEnumerator(GetEnumerator()); } }
        public ValueEnumerator Values { get { return new ValueEnumerator(GetEnumerator()); } }

        #endregion common interface

        #region typecasting properties


        public virtual double AsDouble
        {
            get
            {
                double v = 0.0;
                if (double.TryParse(Value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                    return v;
                return 0.0;
            }
            set
            {
                Value = value.ToString(CultureInfo.InvariantCulture);
            }
        }

        public virtual int AsInt
        {
            get { return (int)AsDouble; }
            set { AsDouble = value; }
        }

        public virtual float AsFloat
        {
            get { return (float)AsDouble; }
            set { AsDouble = value; }
        }

        public virtual bool AsBool
        {
            get
            {
                bool v = false;
                if (bool.TryParse(Value, out v))
                    return v;
                return !string.IsNullOrEmpty(Value);
            }
            set
            {
                Value = (value) ? "true" : "false";
            }
        }

        public virtual long AsLong
        {
            get
            {
                long val = 0;
                if (long.TryParse(Value, out val))
                    return val;
                return 0L;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual ulong AsULong
        {
            get
            {
                ulong val = 0;
                if (ulong.TryParse(Value, out val))
                    return val;
                return 0;
            }
            set
            {
                Value = value.ToString();
            }
        }

        public virtual JsonArray AsArray
        {
            get
            {
                return this as JsonArray;
            }
        }

        public virtual JsonObject AsObject
        {
            get
            {
                return this as JsonObject;
            }
        }


        #endregion typecasting properties

        #region operators

        public static implicit operator JsonNode(string s)
        {
            return (s == null) ? (JsonNode) JsonNull.CreateOrGet() : new JsonString(s);
        }
        public static implicit operator string(JsonNode d)
        {
            return (d == null) ? null : d.Value;
        }

        public static implicit operator JsonNode(double n)
        {
            return new JsonNumber(n);
        }
        public static implicit operator double(JsonNode d)
        {
            return (d == null) ? 0 : d.AsDouble;
        }

        public static implicit operator JsonNode(float n)
        {
            return new JsonNumber(n);
        }
        public static implicit operator float(JsonNode d)
        {
            return (d == null) ? 0 : d.AsFloat;
        }

        public static implicit operator JsonNode(int n)
        {
            return new JsonNumber(n);
        }
        public static implicit operator int(JsonNode d)
        {
            return (d == null) ? 0 : d.AsInt;
        }

        public static implicit operator JsonNode(long n)
        {
            if (LongAsString)
                return new JsonString(n.ToString());
            return new JsonNumber(n);
        }
        public static implicit operator long(JsonNode d)
        {
            return (d == null) ? 0L : d.AsLong;
        }

        public static implicit operator JsonNode(ulong n)
        {
            if (LongAsString)
                return new JsonString(n.ToString());
            return new JsonNumber(n);
        }
        public static implicit operator ulong(JsonNode d)
        {
            return (d == null) ? 0 : d.AsULong;
        }

        public static implicit operator JsonNode(bool b)
        {
            return new JsonBool(b);
        }
        public static implicit operator bool(JsonNode d)
        {
            return (d == null) ? false : d.AsBool;
        }

        public static implicit operator JsonNode(KeyValuePair<string, JsonNode> aKeyValue)
        {
            return aKeyValue.Value;
        }

        public static bool operator ==(JsonNode a, object b)
        {
            if (ReferenceEquals(a, b))
                return true;
            bool aIsNull = a is JsonNull || ReferenceEquals(a, null) || a is JsonLazyCreator;
            bool bIsNull = b is JsonNull || ReferenceEquals(b, null) || b is JsonLazyCreator;
            if (aIsNull && bIsNull)
                return true;
            return !aIsNull && a.Equals(b);
        }

        public static bool operator !=(JsonNode a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #endregion operators

        [ThreadStatic]
        private static StringBuilder _mEscapeBuilder;
        internal static StringBuilder EscapeBuilder
        {
            get
            {
                if (_mEscapeBuilder == null)
                    _mEscapeBuilder = new StringBuilder();
                return _mEscapeBuilder;
            }
        }
        internal static string Escape(string aText)
        {
            var sb = EscapeBuilder;
            sb.Length = 0;
            if (sb.Capacity < aText.Length + aText.Length / 10)
                sb.Capacity = aText.Length + aText.Length / 10;
            foreach (char c in aText)
            {
                switch (c)
                {
                    case '\\':
                        sb.Append("\\\\");
                        break;
                    case '\"':
                        sb.Append("\\\"");
                        break;
                    case '\n':
                        sb.Append("\\n");
                        break;
                    case '\r':
                        sb.Append("\\r");
                        break;
                    case '\t':
                        sb.Append("\\t");
                        break;
                    case '\b':
                        sb.Append("\\b");
                        break;
                    case '\f':
                        sb.Append("\\f");
                        break;
                    default:
                        if (c < ' ' || (ForceAscii && c > 127))
                        {
                            ushort val = c;
                            sb.Append("\\u").Append(val.ToString("X4"));
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }
            string result = sb.ToString();
            sb.Length = 0;
            return result;
        }

        private static JsonNode ParseElement(string token, bool quoted)
        {
            if (quoted)
                return token;
            if (token.Length <= 5)
            {
                string tmp = token.ToLower();
                if (tmp == "false" || tmp == "true")
                    return tmp == "true";
                if (tmp == "null")
                    return JsonNull.CreateOrGet();
            }
            double val;
            if (double.TryParse(token, NumberStyles.Float, CultureInfo.InvariantCulture, out val))
                return val;
            else
                return token;
        }

        public static JsonNode Parse(string aJson)
        {
            Stack<JsonNode> stack = new Stack<JsonNode>();
            JsonNode ctx = null;
            int i = 0;
            StringBuilder token = new StringBuilder();
            string tokenName = "";
            bool quoteMode = false;
            bool tokenIsQuoted = false;
            bool hasNewlineChar = false;
            while (i < aJson.Length)
            {
                switch (aJson[i])
                {
                    case '{':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }
                        stack.Push(new JsonObject());
                        if (ctx != null)
                        {
                            ctx.Add(tokenName, stack.Peek());
                        }
                        tokenName = "";
                        token.Length = 0;
                        ctx = stack.Peek();
                        hasNewlineChar = false;
                        break;

                    case '[':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }

                        stack.Push(new JsonArray());
                        if (ctx != null)
                        {
                            ctx.Add(tokenName, stack.Peek());
                        }
                        tokenName = "";
                        token.Length = 0;
                        ctx = stack.Peek();
                        hasNewlineChar = false;
                        break;

                    case '}':
                    case ']':
                        if (quoteMode)
                        {

                            token.Append(aJson[i]);
                            break;
                        }
                        if (stack.Count == 0)
                            throw new Exception("JSON Parse: Too many closing brackets");

                        stack.Pop();
                        if (token.Length > 0 || tokenIsQuoted)
                            ctx.Add(tokenName, ParseElement(token.ToString(), tokenIsQuoted));
                        if (ctx != null)
                            ctx.Inline = !hasNewlineChar;
                        tokenIsQuoted = false;
                        tokenName = "";
                        token.Length = 0;
                        if (stack.Count > 0)
                            ctx = stack.Peek();
                        break;

                    case ':':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }
                        tokenName = token.ToString();
                        token.Length = 0;
                        tokenIsQuoted = false;
                        break;

                    case '"':
                        quoteMode ^= true;
                        tokenIsQuoted |= quoteMode;
                        break;

                    case ',':
                        if (quoteMode)
                        {
                            token.Append(aJson[i]);
                            break;
                        }
                        if (token.Length > 0 || tokenIsQuoted)
                            ctx.Add(tokenName, ParseElement(token.ToString(), tokenIsQuoted));
                        tokenIsQuoted = false;
                        tokenName = "";
                        token.Length = 0;
                        tokenIsQuoted = false;
                        break;

                    case '\r':
                    case '\n':
                        hasNewlineChar = true;
                        break;

                    case ' ':
                    case '\t':
                        if (quoteMode)
                            token.Append(aJson[i]);
                        break;

                    case '\\':
                        ++i;
                        if (quoteMode)
                        {
                            char c = aJson[i];
                            switch (c)
                            {
                                case 't':
                                    token.Append('\t');
                                    break;
                                case 'r':
                                    token.Append('\r');
                                    break;
                                case 'n':
                                    token.Append('\n');
                                    break;
                                case 'b':
                                    token.Append('\b');
                                    break;
                                case 'f':
                                    token.Append('\f');
                                    break;
                                case 'u':
                                    {
                                        string s = aJson.Substring(i + 1, 4);
                                        token.Append((char)int.Parse(
                                            s,
                                            System.Globalization.NumberStyles.AllowHexSpecifier));
                                        i += 4;
                                        break;
                                    }
                                default:
                                    token.Append(c);
                                    break;
                            }
                        }
                        break;
                    case '/':
                        if (AllowLineComments && !quoteMode && i + 1 < aJson.Length && aJson[i + 1] == '/')
                        {
                            while (++i < aJson.Length && aJson[i] != '\n' && aJson[i] != '\r') ;
                            break;
                        }
                        token.Append(aJson[i]);
                        break;
                    case '\uFEFF': // remove / ignore BOM (Byte Order Mark)
                        break;

                    default:
                        token.Append(aJson[i]);
                        break;
                }
                ++i;
            }
            if (quoteMode)
            {
                throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
            }
            if (ctx == null)
                return ParseElement(token.ToString(), tokenIsQuoted);
            return ctx;
        }

    }
    // End of JSONNode

    internal partial class JsonArray : JsonNode
    {
        private List<JsonNode> _mList = new List<JsonNode>();
        private bool _inline = false;
        public override bool Inline
        {
            get { return _inline; }
            set { _inline = value; }
        }

        public override JsonNodeType Tag { get { return JsonNodeType.ARRAY; } }
        public override bool IsArray { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(_mList.GetEnumerator()); }

        public override JsonNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= _mList.Count)
                    return new JsonLazyCreator(this);
                return _mList[aIndex];
            }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= _mList.Count)
                    _mList.Add(value);
                else
                    _mList[aIndex] = value;
            }
        }

        public override JsonNode this[string aKey]
        {
            get { return new JsonLazyCreator(this); }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                _mList.Add(value);
            }
        }

        public override int Count
        {
            get { return _mList.Count; }
        }

        public override void Add(string aKey, JsonNode aItem)
        {
            if (aItem == null)
                aItem = JsonNull.CreateOrGet();
            _mList.Add(aItem);
        }

        public override JsonNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= _mList.Count)
                return null;
            JsonNode tmp = _mList[aIndex];
            _mList.RemoveAt(aIndex);
            return tmp;
        }

        public override JsonNode Remove(JsonNode aNode)
        {
            _mList.Remove(aNode);
            return aNode;
        }

        public override void Clear()
        {
            _mList.Clear();
        }

        public override JsonNode Clone()
        {
            var node = new JsonArray();
            node._mList.Capacity = _mList.Capacity;
            foreach(var n in _mList)
            {
                if (n != null)
                    node.Add(n.Clone());
                else
                    node.Add(null);
            }
            return node;
        }

        public override IEnumerable<JsonNode> Children
        {
            get
            {
                foreach (JsonNode n in _mList)
                    yield return n;
            }
        }


        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('[');
            int count = _mList.Count;
            if (_inline)
                aMode = JsonTextMode.COMPACT;
            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                    aSb.Append(',');
                if (aMode == JsonTextMode.INDENT)
                    aSb.AppendLine();

                if (aMode == JsonTextMode.INDENT)
                    aSb.Append(' ', aIndent + aIndentInc);
                _mList[i].WriteToStringBuilder(aSb, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JsonTextMode.INDENT)
                aSb.AppendLine().Append(' ', aIndent);
            aSb.Append(']');
        }
    }
    // End of JSONArray

    internal partial class JsonObject : JsonNode
    {
        private Dictionary<string, JsonNode> _mDict = new Dictionary<string, JsonNode>();

        private bool _inline = false;
        public override bool Inline
        {
            get { return _inline; }
            set { _inline = value; }
        }

        public override JsonNodeType Tag { get { return JsonNodeType.OBJECT; } }
        public override bool IsObject { get { return true; } }

        public override Enumerator GetEnumerator() { return new Enumerator(_mDict.GetEnumerator()); }


        public override JsonNode this[string aKey]
        {
            get
            {
                if (_mDict.ContainsKey(aKey))
                    return _mDict[aKey];
                else
                    return new JsonLazyCreator(this, aKey);
            }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (_mDict.ContainsKey(aKey))
                    _mDict[aKey] = value;
                else
                    _mDict.Add(aKey, value);
            }
        }

        public override JsonNode this[int aIndex]
        {
            get
            {
                if (aIndex < 0 || aIndex >= _mDict.Count)
                    return null;
                return _mDict.ElementAt(aIndex).Value;
            }
            set
            {
                if (value == null)
                    value = JsonNull.CreateOrGet();
                if (aIndex < 0 || aIndex >= _mDict.Count)
                    return;
                string key = _mDict.ElementAt(aIndex).Key;
                _mDict[key] = value;
            }
        }

        public override int Count
        {
            get { return _mDict.Count; }
        }

        public override void Add(string aKey, JsonNode aItem)
        {
            if (aItem == null)
                aItem = JsonNull.CreateOrGet();

            if (aKey != null)
            {
                if (_mDict.ContainsKey(aKey))
                    _mDict[aKey] = aItem;
                else
                    _mDict.Add(aKey, aItem);
            }
            else
                _mDict.Add(Guid.NewGuid().ToString(), aItem);
        }

        public override JsonNode Remove(string aKey)
        {
            if (!_mDict.ContainsKey(aKey))
                return null;
            JsonNode tmp = _mDict[aKey];
            _mDict.Remove(aKey);
            return tmp;
        }

        public override JsonNode Remove(int aIndex)
        {
            if (aIndex < 0 || aIndex >= _mDict.Count)
                return null;
            var item = _mDict.ElementAt(aIndex);
            _mDict.Remove(item.Key);
            return item.Value;
        }

        public override JsonNode Remove(JsonNode aNode)
        {
            try
            {
                var item = _mDict.Where(k => k.Value == aNode).First();
                _mDict.Remove(item.Key);
                return aNode;
            }
            catch
            {
                return null;
            }
        }

        public override void Clear()
        {
            _mDict.Clear();
        }

        public override JsonNode Clone()
        {
            var node = new JsonObject();
            foreach (var n in _mDict)
            {
                node.Add(n.Key, n.Value.Clone());
            }
            return node;
        }

        public override bool HasKey(string aKey)
        {
            return _mDict.ContainsKey(aKey);
        }

        public override JsonNode GetValueOrDefault(string aKey, JsonNode aDefault)
        {
            JsonNode res;
            if (_mDict.TryGetValue(aKey, out res))
                return res;
            return aDefault;
        }

        public override IEnumerable<JsonNode> Children
        {
            get
            {
                foreach (KeyValuePair<string, JsonNode> n in _mDict)
                    yield return n.Value;
            }
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('{');
            bool first = true;
            if (_inline)
                aMode = JsonTextMode.COMPACT;
            foreach (var k in _mDict)
            {
                if (!first)
                    aSb.Append(',');
                first = false;
                if (aMode == JsonTextMode.INDENT)
                    aSb.AppendLine();
                if (aMode == JsonTextMode.INDENT)
                    aSb.Append(' ', aIndent + aIndentInc);
                aSb.Append('\"').Append(Escape(k.Key)).Append('\"');
                if (aMode == JsonTextMode.COMPACT)
                    aSb.Append(':');
                else
                    aSb.Append(" : ");
                k.Value.WriteToStringBuilder(aSb, aIndent + aIndentInc, aIndentInc, aMode);
            }
            if (aMode == JsonTextMode.INDENT)
                aSb.AppendLine().Append(' ', aIndent);
            aSb.Append('}');
        }

    }
    // End of JSONObject

    internal partial class JsonString : JsonNode
    {
        private string _mData;

        public override JsonNodeType Tag { get { return JsonNodeType.STRING; } }
        public override bool IsString { get { return true; } }

        public override Enumerator GetEnumerator() { return new Enumerator(); }


        public override string Value
        {
            get { return _mData; }
            set
            {
                _mData = value;
            }
        }

        public JsonString(string aData)
        {
            _mData = aData;
        }
        public override JsonNode Clone()
        {
            return new JsonString(_mData);
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append('\"').Append(Escape(_mData)).Append('\"');
        }
        public override bool Equals(object obj)
        {
            if (base.Equals(obj))
                return true;
            string s = obj as string;
            if (s != null)
                return _mData == s;
            JsonString s2 = obj as JsonString;
            if (s2 != null)
                return _mData == s2._mData;
            return false;
        }
        public override int GetHashCode()
        {
            return _mData.GetHashCode();
        }
        public override void Clear()
        {
            _mData = "";
        }
    }
    // End of JSONString

    internal partial class JsonNumber : JsonNode
    {
        private double _mData;

        public override JsonNodeType Tag { get { return JsonNodeType.NUMBER; } }
        public override bool IsNumber { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return _mData.ToString(CultureInfo.InvariantCulture); }
            set
            {
                double v;
                if (double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out v))
                    _mData = v;
            }
        }

        public override double AsDouble
        {
            get { return _mData; }
            set { _mData = value; }
        }
        public override long AsLong
        {
            get { return (long)_mData; }
            set { _mData = value; }
        }
        public override ulong AsULong
        {
            get { return (ulong)_mData; }
            set { _mData = value; }
        }

        public JsonNumber(double aData)
        {
            _mData = aData;
        }

        public JsonNumber(string aData)
        {
            Value = aData;
        }

        public override JsonNode Clone()
        {
            return new JsonNumber(_mData);
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append(Value);
        }
        private static bool IsNumeric(object value)
        {
            return value is int || value is uint
                || value is float || value is double
                || value is decimal
                || value is long || value is ulong
                || value is short || value is ushort
                || value is sbyte || value is byte;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (base.Equals(obj))
                return true;
            JsonNumber s2 = obj as JsonNumber;
            if (s2 != null)
                return _mData == s2._mData;
            if (IsNumeric(obj))
                return Convert.ToDouble(obj) == _mData;
            return false;
        }
        public override int GetHashCode()
        {
            return _mData.GetHashCode();
        }
        public override void Clear()
        {
            _mData = 0;
        }
    }
    // End of JSONNumber

    internal partial class JsonBool : JsonNode
    {
        private bool _mData;

        public override JsonNodeType Tag { get { return JsonNodeType.BOOLEAN; } }
        public override bool IsBoolean { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return _mData.ToString(); }
            set
            {
                bool v;
                if (bool.TryParse(value, out v))
                    _mData = v;
            }
        }
        public override bool AsBool
        {
            get { return _mData; }
            set { _mData = value; }
        }

        public JsonBool(bool aData)
        {
            _mData = aData;
        }

        public JsonBool(string aData)
        {
            Value = aData;
        }

        public override JsonNode Clone()
        {
            return new JsonBool(_mData);
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append((_mData) ? "true" : "false");
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (obj is bool)
                return _mData == (bool)obj;
            return false;
        }
        public override int GetHashCode()
        {
            return _mData.GetHashCode();
        }
        public override void Clear()
        {
            _mData = false;
        }
    }
    // End of JSONBool

    internal partial class JsonNull : JsonNode
    {
        static JsonNull _mStaticInstance = new JsonNull();
        public static bool ReuseSameInstance = true;
        public static JsonNull CreateOrGet()
        {
            if (ReuseSameInstance)
                return _mStaticInstance;
            return new JsonNull();
        }
        private JsonNull() { }

        public override JsonNodeType Tag { get { return JsonNodeType.NULL_VALUE; } }
        public override bool IsNull { get { return true; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public override string Value
        {
            get { return "null"; }
            set { }
        }
        public override bool AsBool
        {
            get { return false; }
            set { }
        }

        public override JsonNode Clone()
        {
            return CreateOrGet();
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(this, obj))
                return true;
            return (obj is JsonNull);
        }
        public override int GetHashCode()
        {
            return 0;
        }

        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append("null");
        }
    }
    // End of JSONNull

    internal partial class JsonLazyCreator : JsonNode
    {
        private JsonNode _mNode = null;
        private string _mKey = null;
        public override JsonNodeType Tag { get { return JsonNodeType.NONE; } }
        public override Enumerator GetEnumerator() { return new Enumerator(); }

        public JsonLazyCreator(JsonNode aNode)
        {
            _mNode = aNode;
            _mKey = null;
        }

        public JsonLazyCreator(JsonNode aNode, string aKey)
        {
            _mNode = aNode;
            _mKey = aKey;
        }

        private T Set<T>(T aVal) where T : JsonNode
        {
            if (_mKey == null)
                _mNode.Add(aVal);
            else
                _mNode.Add(_mKey, aVal);
            _mNode = null; // Be GC friendly.
            return aVal;
        }

        public override JsonNode this[int aIndex]
        {
            get { return new JsonLazyCreator(this); }
            set { Set(new JsonArray()).Add(value); }
        }

        public override JsonNode this[string aKey]
        {
            get { return new JsonLazyCreator(this, aKey); }
            set { Set(new JsonObject()).Add(aKey, value); }
        }

        public override void Add(JsonNode aItem)
        {
            Set(new JsonArray()).Add(aItem);
        }

        public override void Add(string aKey, JsonNode aItem)
        {
            Set(new JsonObject()).Add(aKey, aItem);
        }

        public static bool operator ==(JsonLazyCreator a, object b)
        {
            if (b == null)
                return true;
            return System.Object.ReferenceEquals(a, b);
        }

        public static bool operator !=(JsonLazyCreator a, object b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return true;
            return System.Object.ReferenceEquals(this, obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public override int AsInt
        {
            get { Set(new JsonNumber(0)); return 0; }
            set { Set(new JsonNumber(value)); }
        }

        public override float AsFloat
        {
            get { Set(new JsonNumber(0.0f)); return 0.0f; }
            set { Set(new JsonNumber(value)); }
        }

        public override double AsDouble
        {
            get { Set(new JsonNumber(0.0)); return 0.0; }
            set { Set(new JsonNumber(value)); }
        }

        public override long AsLong
        {
            get
            {
                if (LongAsString)
                    Set(new JsonString("0"));
                else
                    Set(new JsonNumber(0.0));
                return 0L;
            }
            set
            {
                if (LongAsString)
                    Set(new JsonString(value.ToString()));
                else
                    Set(new JsonNumber(value));
            }
        }

        public override ulong AsULong
        {
            get
            {
                if (LongAsString)
                    Set(new JsonString("0"));
                else
                    Set(new JsonNumber(0.0));
                return 0L;
            }
            set
            {
                if (LongAsString)
                    Set(new JsonString(value.ToString()));
                else
                    Set(new JsonNumber(value));
            }
        }

        public override bool AsBool
        {
            get { Set(new JsonBool(false)); return false; }
            set { Set(new JsonBool(value)); }
        }

        public override JsonArray AsArray
        {
            get { return Set(new JsonArray()); }
        }

        public override JsonObject AsObject
        {
            get { return Set(new JsonObject()); }
        }
        internal override void WriteToStringBuilder(StringBuilder aSb, int aIndent, int aIndentInc, JsonTextMode aMode)
        {
            aSb.Append("null");
        }
    }
    // End of JSONLazyCreator

    internal static class Json
    {
        internal static JsonNode Parse(string aJson)
        {
            return JsonNode.Parse(aJson);
        }
    }
}
