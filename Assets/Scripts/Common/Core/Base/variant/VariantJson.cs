using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Atom.Variant
{
    public sealed partial class Var
    {
        //-----------------------------------------------------------------------------------------
        private static JToken SaveToJson(Var v)
        {
            JToken token;

            if (v.IsNull())
                token = JValue.CreateNull();
            else if (v.IsList())
            {
                token = new JArray();

                for (var i = 0; i != v.Count; ++i)
                    token.Value<JArray>().Add(SaveToJson(v.GetValue(i)));
            }
            else if (v.IsTree())
            {
                token = new JObject();

                for (var i = 0; i != v.Count; ++i)
                    token.Value<JObject>().Add(v.GetKey(i), SaveToJson(v.GetValue(i)));
            }
            else if (v.IsBool())
                token = new JValue((bool)v);
            else if (v.IsInt())
                token = new JValue((int)v);
            else if (v.IsFloat())
                token = new JValue((float)v);
            else if (v.IsString())
                token = new JValue((string)v);
            else
                throw new VariantUnsupportedFunctionalityException("conversion to json node", v.Type);

            return token;
        }
        //-----------------------------------------------------------------------------------------
        public static void SaveToFileJson(string fileName, Var v)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            var data = SaveToStringJson(v);

            Conversion.StringToFile(fileName, data);
        }
        //-----------------------------------------------------------------------------------------
        public static string SaveToStringJson(Var v)
        {
            var token = SaveToJson(v);

            var sb = new StringBuilder();
            var sw = new StringWriter(sb);

            using (var jw = new JsonTextWriter(sw))
            {
                jw.Formatting = Formatting.Indented;
                jw.IndentChar = ' ';
                jw.Indentation = 4;
                token.WriteTo(jw);
            }

            return sb.ToString(); 
        }
        //-----------------------------------------------------------------------------------------
        private static Var LoadFromJson(JToken node, Var parent)
        {
            Var ret;

            switch (node.Type)
            {
                case JTokenType.Object:
                    {
                        ret = CreateTree();

                        foreach (var i in (JObject)node)
                        {
                            var varName = i.Key;
                            var varValue = i.Value;

                            ret.Add(varName, LoadFromJson(varValue, ret));
                        }
                    }
                    break;
                case JTokenType.Array:
                    {
                        ret = CreateList();

                        foreach (var i in (JArray)node)
                            ret.Add(LoadFromJson(i, ret));

                    }
                    break;
                case JTokenType.Boolean: ret = new Var(node.ToObject<bool>()); break;
                case JTokenType.Integer: ret = new Var(node.ToObject<int>()); break;
                case JTokenType.Float: ret = new Var(node.ToObject<float>()); break;
                case JTokenType.String: ret = new Var(node.ToObject<string>()); break;
                default: ret = new Var(); break;
            }

            ret.Parent = parent;
            return ret;
        }
        //-----------------------------------------------------------------------------------------
        public static Var LoadFromFileJson(string fileName)
        {
            var data = Conversion.FileToString(fileName);
            var root = JObject.Parse(data);
            return LoadFromJson(root, null);
        }
        //-----------------------------------------------------------------------------------------
        public static Var LoadFromStringJson(string text)
        {
            var root = JObject.Parse(text);
            return LoadFromJson(root, null);
        }
        //-----------------------------------------------------------------------------------------
    }
}