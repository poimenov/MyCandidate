using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Avalonia.Platform;
using Avalonia.PropertyGrid.Services;
using MyCandidate.MVVM.Converters;

namespace MyCandidate.MVVM.Extensions;

public class XsltExtObject
{
    public static XslCompiledTransform GetTransform(string entityName, string format)
    {
        XslCompiledTransform retVal = new XslCompiledTransform();
        var path = $"{ResourceTypeNameToSvgPathConverter.BASE_PATH}/xslt/{entityName}.{format}.xslt";
        using (var stream = AssetLoader.Open(new Uri(path)))
        {
            using (var reader = XmlReader.Create(stream))
            {
                retVal.Load(reader, new XsltSettings(true, true), null);
            }
        }
        return retVal;
    }    
    
    public string GetLocalized(string key)
    {
        return LocalizationService.Default[key];
    }

    public string GetResourceText(string value)
    {
        if(File.Exists(value))
        {
            return Path.GetFileName(value);
        }
        else if(Uri.IsWellFormedUriString(value, UriKind.Absolute))
        {
            return new Uri((string)value).Host;
        }
        
        return value;        
    }

    public string? GetSvg(string svgFileName, int width, int height, bool asCssAttribute)
    {
        string ns="http://www.w3.org/2000/svg";
        var path = $"{ResourceTypeNameToSvgPathConverter.BASE_PATH}/{svgFileName}";
        var doc = new XmlDocument();
        using (var stream = AssetLoader.Open(new System.Uri(path)))
        {
            using (var reader = XmlReader.Create(stream))
            {
                doc.Load(reader);
            }
        }
        var attWidth = doc.CreateAttribute("width");
        attWidth.Value = $"{width}px";
        doc?.DocumentElement?.Attributes.SetNamedItem(attWidth);

        var attHeight = doc?.CreateAttribute("height");
        attHeight!.Value = $"{height}px";
        doc?.DocumentElement?.Attributes.SetNamedItem(attHeight);        

        var sRoot = doc?.DocumentElement?.OuterXml;
        if (!string.IsNullOrEmpty(sRoot) && asCssAttribute)
        {
            var sb = new StringBuilder();
            sb.AppendLine("background-image: url('data:image/svg+xml,\\");
            using (var reader = new StringReader(sRoot))
            {
                var line = reader.ReadLine();
                sb.AppendLine(string.Concat(line?.Replace("#","%23"), "\\"));
                while (line != null)
                {
                    line = reader.ReadLine();
                    if(line != null)
                    {
                        sb.AppendLine(string.Concat(line.Replace("#","%23"), "\\"));
                    }                    
                }
            }
            sb.AppendLine("');");
            return sb.ToString();
        }

        return doc?.DocumentElement?.OuterXml;
    }
}
