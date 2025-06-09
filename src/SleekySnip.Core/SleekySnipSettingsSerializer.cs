using System.Xml;
using System.IO;

namespace SleekySnip.Core;

public static class SleekySnipSettingsSerializer
{
    public static SleekySnipSettings Load(string path)
    {
        var settings = new SleekySnipSettings();
        if (!File.Exists(path))
            return settings;

        var doc = new XmlDocument();
        doc.Load(path);
        var root = doc.DocumentElement;
        if (root == null)
            return settings;

        if (root["Hotkey"] != null)
            settings.Hotkey = root["Hotkey"]!.InnerText;
        if (root["CaptureMode"] != null)
            settings.CaptureMode = root["CaptureMode"]!.InnerText;
        if (root["OutputFolder"] != null)
            settings.OutputFolder = root["OutputFolder"]!.InnerText;
        return settings;
    }

    public static void Save(SleekySnipSettings settings, string path)
    {
        var doc = new XmlDocument();
        var declaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
        doc.AppendChild(declaration);

        var root = doc.CreateElement("SleekySnipSettings");
        doc.AppendChild(root);

        var hotkey = doc.CreateElement("Hotkey");
        hotkey.InnerText = settings.Hotkey;
        root.AppendChild(hotkey);

        var capture = doc.CreateElement("CaptureMode");
        capture.InnerText = settings.CaptureMode;
        root.AppendChild(capture);

        var folder = doc.CreateElement("OutputFolder");
        folder.InnerText = settings.OutputFolder;
        root.AppendChild(folder);

        var xmlWriterSettings = new XmlWriterSettings { Indent = true };
        using var writer = XmlWriter.Create(path, xmlWriterSettings);
        doc.Save(writer);
    }
}

