using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Xsl;

namespace MyCandidate.MVVM.Extensions;

public static class XmlDocumentExtension
{
    public static async Task SaveAsync(this XmlDocument obj, string filePath, XslCompiledTransform? xslt, XsltArgumentList? args)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write,
            FileShare.None, bufferSize: 4096, useAsync: true))
        {
            var settings = new XmlWriterSettings { Async = true };
            if (xslt != null)
            {
                settings.Indent = true;
                settings.CloseOutput = true;
                settings.OmitXmlDeclaration = true;
                settings.Encoding = Encoding.UTF8;
                await using (XmlWriter writer = XmlWriter.Create(fs, settings))
                {
                    xslt.Transform(obj, args, writer);
                    await writer.FlushAsync();
                }
            }
            else
            {
                await using (var writer = XmlWriter.Create(fs, settings))
                {
                    obj.Save(writer);
                    await writer.FlushAsync();
                }
            }
        }
    }
}
