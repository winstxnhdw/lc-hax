#!/usr/bin/env dotnet-script

using System;
using System.Collections.Generic;
using System.IO;

using System.Xml;
using System.Xml.Linq;

string csprojPath = Environment.GetEnvironmentVariable("CSPROJ_PATH");

if (!File.Exists(csprojPath))
{
    WriteLine("CSPROJ_PATH environment variable is not set or invalid.");
    return;
}

XDocument document = XDocument.Load(csprojPath, LoadOptions.PreserveWhitespace);

Dictionary<string, string> packageReferences = document.Descendants("PackageReference").ToDictionary(
    element => element.Attribute("Include")?.Value.ToLower(),
    element => element.Attribute("Version")?.Value
);

foreach (XElement embeddedResource in document.Descendants("EmbeddedResource"))
{
    string includeAttribute = embeddedResource.Attribute("Include")?.Value;
    string[] path = includeAttribute.Split('/');

    if (!packageReferences.TryGetValue(path[1].ToLower(), out string version))
    {
        continue;
    }

    path[2] = version;
    embeddedResource.Attribute("Include")?.SetValue(string.Join('/', path));
}

using (XmlWriter xmlWriter = XmlWriter.Create(csprojPath, new XmlWriterSettings() { OmitXmlDeclaration = true }))
{
    document.Save(xmlWriter);
}
