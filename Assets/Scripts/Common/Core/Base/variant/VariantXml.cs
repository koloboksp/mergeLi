using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Atom.Variant
{
    public sealed partial class Var
    {
        //-----------------------------------------------------------------------------------------
        private static void SaveToXml(XmlDocument doc, XmlElement parent, Var v)
        {
            if (v.IsComplex())
            {
                for (var i = 0; i != v.Count; i++)
                {
                    var value = v.GetValue(i);

                    var node = doc.CreateElement(Conversion.ToString(value.Type));

                    if (v.IsTree() || v.IsObject())
                    {
                        var key = v.GetKey(i);
                        node.SetAttribute("Name", key);
                    }
                    if (value.IsObject())
                        node.SetAttribute("Type", Conversion.ToString(value.Data));
                    SaveToXml(doc, node, value);
                    parent.AppendChild(node);
                }
            }
            else
            {
                if (!v.IsNull())
                    parent.SetAttribute("Value", v.ToString());
            }
        }
        //-----------------------------------------------------------------------------------------
        public static void SaveToFileXml(string fileName, Var v)
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                SaveToStreamXml(fs, v);
            }
        }
        //-----------------------------------------------------------------------------------------
        private static void SaveToStreamXml(Stream stream, Var v)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement(Conversion.ToString(v.Type));

            if (v.IsComplex())
            {
                if (v.IsObject())
                    node.SetAttribute("Type", Conversion.ToString(v.Data));
                SaveToXml(doc, node, v);
            }
            else
            {
                if (!v.IsNull())
                    node.SetAttribute("Value", v.ToString());
            }

            doc.AppendChild(node);
            doc.Save(stream);
        }

        //-----------------------------------------------------------------------------------------
        public static string SaveToStringXml(Var v)
        {
            var doc = new XmlDocument();
            var node = doc.CreateElement(Conversion.ToString(v.Type));

            if (v.IsComplex())
            {
                if (v.IsObject())
                    node.SetAttribute("Type", Conversion.ToString(v.Data));
                SaveToXml(doc, node, v);
            }
            else
            {
                if (!v.IsNull())
                    node.SetAttribute("Value", v.ToString());
            }

            doc.AppendChild(node);

            using (var stringWriter = new StringWriter())
            {
                using (var xmlTextWriter = new XmlTextWriter(stringWriter) { Formatting = Formatting.Indented })
                {
                    doc.WriteTo(xmlTextWriter);
                    return stringWriter.ToString();
                }
            }
        }

        //-----------------------------------------------------------------------------------------
        private static Var LoadFromXml(XmlNode node, Var parent)
        {
            VarType type;

            try
            {
                type = Conversion.ToVarType(node.Name);
            }
            catch (Exception)
            {
                throw new Exception("invalid xml format type:" + node.Name + " not determined");
            }  
            
            if (node.Attributes == null)
                throw new Exception("invalid xml format. no attributes");

            Var ret;

            if (type == VarType.Tree || type == VarType.Object)
            {
                byte[] objectType = null;
                if (type == VarType.Object)
                {
                    if (node.Attributes["Type"] == null)
                        throw new Exception("invalid xml format. Type attribute missing");

                    objectType = Conversion.ToByteArray(node.Attributes["Type"].Value);
                }

                ret = new Var
                {
                    Type = type,
                    Data = objectType,
                    mTreeVars = new SortedList(),
                    Parent = parent
                };

                foreach (XmlNode xmlNode in node.ChildNodes)
                {
                    if (XmlNodeType.Element == xmlNode.NodeType)
                    {
                        var xmlAttributes = xmlNode.Attributes;

                        if (null == xmlAttributes)
                        {
                            throw new Exception("invalid xml format. no attributes");
                        }

                        var xmlAttributeName = xmlAttributes["Name"];

                        if (null == xmlAttributeName)
                        {
                            throw new Exception("invalid xml format. Name attribute missing");
                        }

                        try
                        {
                            ret.mTreeVars.Add(xmlAttributeName.Value, LoadFromXml(xmlNode, ret));
                        }
                        catch (Exception e)
                        {
                            throw new Exception("node name: " + xmlAttributeName.Value, e);
                        }

                    }
                }
            }
            else if (type == VarType.List)
            {
                ret = new Var { Type = type, mListVars = new List<Var>(), Parent = parent };
                var index = 0;
                foreach (XmlNode xmlNode in node.ChildNodes)
                {
                    try
                    {
                        ret.mListVars.Add(LoadFromXml(xmlNode, ret));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("node name: " + index, e);
                    }

                    ++index;
                }
            }
            else
            {
                if (type != VarType.Null)
                {
                    var value = node.Attributes["Value"];

                    if (value == null)
                        throw new Exception("invalid xml format. Value attribute missing");

                    ret = Conversion.ToVar(type, value.Value);
                }
                else
                    ret = new Var();

                ret.Parent = parent;
            }

            return ret;
        }
        //-----------------------------------------------------------------------------------------
        public static Var LoadFromFileXml(string fileName)
        {
            var doc = new XmlDocument();
            doc.Load(fileName);

            if (doc.DocumentElement == null)
                throw new Exception("invalid xml format ");

            return LoadFromXml(doc.DocumentElement, null);
        }

        //-----------------------------------------------------------------------------------------
        public static Var LoadFromStreamXml(Stream stream)
        {
            var doc = new XmlDocument();
            doc.Load(stream);

            if (doc.DocumentElement == null)
                throw new Exception("invalid xml format");

            return LoadFromXml(doc.DocumentElement, null);
        }

        //-----------------------------------------------------------------------------------------
        public static Var LoadFromStringXml(string text)
        {
            var doc = new XmlDocument();
            doc.Load(new StringReader(text));

            if (doc.DocumentElement == null)
                throw new Exception("invalid xml format");

            return LoadFromXml(doc.DocumentElement, null);
        }
        //-----------------------------------------------------------------------------------------
    }
}