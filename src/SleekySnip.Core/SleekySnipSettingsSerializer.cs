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

        if (root["ScreenHotkey"] != null)
            settings.ScreenHotkey = root["ScreenHotkey"]!.InnerText;
        if (root["WindowHotkey"] != null)
            settings.WindowHotkey = root["WindowHotkey"]!.InnerText;
        if (root["RegionHotkey"] != null)
            settings.RegionHotkey = root["RegionHotkey"]!.InnerText;
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

        var screen = doc.CreateElement("ScreenHotkey");
        screen.InnerText = settings.ScreenHotkey;
        root.AppendChild(screen);

        var window = doc.CreateElement("WindowHotkey");
        window.InnerText = settings.WindowHotkey;
        root.AppendChild(window);

        var region = doc.CreateElement("RegionHotkey");
        region.InnerText = settings.RegionHotkey;
        root.AppendChild(region);

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

