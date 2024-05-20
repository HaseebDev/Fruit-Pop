//#define USE_SharpZipLib
/* * * * *
 * This is an extension of the SimpleJSON framework to provide methods to
 * serialize a JSON object tree into a compact binary format. Optionally the
 * binary stream can be compressed with the SharpZipLib when using the define
 * "USE_SharpZipLib"
 * 
 * Those methods where originally part of the framework but since it's rarely
 * used I've extracted this part into this seperate module file.
 * 
 * You can use the define "SimpleJSON_ExcludeBinary" to selectively disable
 * this extension without the need to remove the file from the project.
 * 
 * If you want to use compression when saving to file / stream / B64 you have to include
 * SharpZipLib ( http://www.icsharpcode.net/opensource/sharpziplib/ ) in your project and
 * define "USE_SharpZipLib" at the top of the file
 * 
 * 
 * The MIT License (MIT)
 * 
 * Copyright (c) 2012-2017 Markus Göbel (Bunny83)
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

namespace DTT.DailyRewards
{
#if !SimpleJSON_ExcludeBinary
    internal abstract partial class JsonNode
    {
        public abstract void SerializeBinary(System.IO.BinaryWriter aWriter);

        public void SaveToBinaryStream(System.IO.Stream aData)
        {
            var w = new System.IO.BinaryWriter(aData);
            SerializeBinary(w);
        }

#if USE_SharpZipLib
		public void SaveToCompressedStream(System.IO.Stream aData)
		{
			using (var gzipOut = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(aData))
			{
				gzipOut.IsStreamOwner = false;
				SaveToBinaryStream(gzipOut);
				gzipOut.Close();
			}
		}

		public void SaveToCompressedFile(string aFileName)
		{

			System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
			using(var F = System.IO.File.OpenWrite(aFileName))
			{
				SaveToCompressedStream(F);
			}
		}
		public string SaveToCompressedBase64()
		{
			using (var stream = new System.IO.MemoryStream())
			{
				SaveToCompressedStream(stream);
				stream.Position = 0;
				return System.Convert.ToBase64String(stream.ToArray());
			}
		}

#else
        public void SaveToCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public void SaveToCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public string SaveToCompressedBase64()
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public void SaveToBinaryFile(string aFileName)
        {
            System.IO.Directory.CreateDirectory((new System.IO.FileInfo(aFileName)).Directory.FullName);
            using (var f = System.IO.File.OpenWrite(aFileName))
            {
                SaveToBinaryStream(f);
            }
        }

        public string SaveToBinaryBase64()
        {
            using (var stream = new System.IO.MemoryStream())
            {
                SaveToBinaryStream(stream);
                stream.Position = 0;
                return System.Convert.ToBase64String(stream.ToArray());
            }
        }

        public static JsonNode DeserializeBinary(System.IO.BinaryReader aReader)
        {
            JsonNodeType type = (JsonNodeType)aReader.ReadByte();
            switch (type)
            {
                case JsonNodeType.ARRAY:
                    {
                        int count = aReader.ReadInt32();
                        JsonArray tmp = new JsonArray();
                        for (int i = 0; i < count; i++)
                            tmp.Add(DeserializeBinary(aReader));
                        return tmp;
                    }
                case JsonNodeType.OBJECT:
                    {
                        int count = aReader.ReadInt32();
                        JsonObject tmp = new JsonObject();
                        for (int i = 0; i < count; i++)
                        {
                            string key = aReader.ReadString();
                            var val = DeserializeBinary(aReader);
                            tmp.Add(key, val);
                        }
                        return tmp;
                    }
                case JsonNodeType.STRING:
                    {
                        return new JsonString(aReader.ReadString());
                    }
                case JsonNodeType.NUMBER:
                    {
                        return new JsonNumber(aReader.ReadDouble());
                    }
                case JsonNodeType.BOOLEAN:
                    {
                        return new JsonBool(aReader.ReadBoolean());
                    }
                case JsonNodeType.NULL_VALUE:
                    {
                        return JsonNull.CreateOrGet();
                    }
                default:
                    {
                        throw new Exception("Error deserializing JSON. Unknown tag: " + type);
                    }
            }
        }

#if USE_SharpZipLib
		public static JSONNode LoadFromCompressedStream(System.IO.Stream aData)
		{
			var zin = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(aData);
			return LoadFromBinaryStream(zin);
		}
		public static JSONNode LoadFromCompressedFile(string aFileName)
		{
			using(var F = System.IO.File.OpenRead(aFileName))
			{
				return LoadFromCompressedStream(F);
			}
		}
		public static JSONNode LoadFromCompressedBase64(string aBase64)
		{
			var tmp = System.Convert.FromBase64String(aBase64);
			var stream = new System.IO.MemoryStream(tmp);
			stream.Position = 0;
			return LoadFromCompressedStream(stream);
		}
#else
        public static JsonNode LoadFromCompressedFile(string aFileName)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JsonNode LoadFromCompressedStream(System.IO.Stream aData)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }

        public static JsonNode LoadFromCompressedBase64(string aBase64)
        {
            throw new Exception("Can't use compressed functions. You need include the SharpZipLib and uncomment the define at the top of SimpleJSON");
        }
#endif

        public static JsonNode LoadFromBinaryStream(System.IO.Stream aData)
        {
            using (var r = new System.IO.BinaryReader(aData))
            {
                return DeserializeBinary(r);
            }
        }

        public static JsonNode LoadFromBinaryFile(string aFileName)
        {
            using (var f = System.IO.File.OpenRead(aFileName))
            {
                return LoadFromBinaryStream(f);
            }
        }

        public static JsonNode LoadFromBinaryBase64(string aBase64)
        {
            var tmp = System.Convert.FromBase64String(aBase64);
            var stream = new System.IO.MemoryStream(tmp);
            stream.Position = 0;
            return LoadFromBinaryStream(stream);
        }
    }

    internal partial class JsonArray : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.ARRAY);
            aWriter.Write(_mList.Count);
            for (int i = 0; i < _mList.Count; i++)
            {
                _mList[i].SerializeBinary(aWriter);
            }
        }
    }

    internal partial class JsonObject : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.OBJECT);
            aWriter.Write(_mDict.Count);
            foreach (string k in _mDict.Keys)
            {
                aWriter.Write(k);
                _mDict[k].SerializeBinary(aWriter);
            }
        }
    }

    internal partial class JsonString : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.STRING);
            aWriter.Write(_mData);
        }
    }

    internal partial class JsonNumber : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.NUMBER);
            aWriter.Write(_mData);
        }
    }

    internal partial class JsonBool : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.BOOLEAN);
            aWriter.Write(_mData);
        }
    }
    internal partial class JsonNull : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {
            aWriter.Write((byte)JsonNodeType.NULL_VALUE);
        }
    }
    internal partial class JsonLazyCreator : JsonNode
    {
        public override void SerializeBinary(System.IO.BinaryWriter aWriter)
        {

        }
    }
#endif
}
