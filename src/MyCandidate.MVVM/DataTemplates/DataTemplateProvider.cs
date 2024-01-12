using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using MyCandidate.Common;
using MyCandidate.MVVM.Converters;
using ReactiveUI;

namespace MyCandidate.MVVM.DataTemplates;

public static class DataTemplateProvider
{
    public static FuncDataTemplate<ResourceType> ResourceTypeImage { get; }
        = new FuncDataTemplate<ResourceType>(
            (resourceType) => resourceType is not null, BuildResourceTypeImage);

    private static Control BuildResourceTypeImage(ResourceType resourceType)
    {
        var basePath = "avares://MyCandidate.MVVM/Assets";

        return new Avalonia.Svg.Svg(new System.Uri(basePath))
        {
            Width = 24,
            Height = 24,
            Stretch = Stretch.Uniform,  
            [!Avalonia.Svg.Svg.PathProperty] = new Binding(nameof(resourceType.Name))
            {
                Converter = new ResourceTypeNameToSvgPathConverter()
            },          
            [!ToolTip.TipProperty] = new Binding(nameof(resourceType.Name))            
        };        
    }

    public static FuncDataTemplate<CandidateResource> CandidateResourceLink { get; }
            = new FuncDataTemplate<CandidateResource>(
                (resource) => resource is not null,
                BuildCandidateResourceLink);

    private static Control BuildCandidateResourceLink(CandidateResource resource)
    {
        ICommand? command = null;
        var commandParameter = resource.Value;
        var content = new Binding(nameof(resource.Value));

        switch (resource.ResourceType.Name)
        {
            case "Path":
                command = ReactiveCommand.Create(
                    (object obj) =>
                    {
                        if (obj is string path && File.Exists(path))
                        {
                            Open(path);
                        }
                    }
                ); 
                content.Converter = new PathToFileNameConverter();
                break;
            case "Url":
                command = ReactiveCommand.Create(
                    (object obj) =>
                    {
                        if (obj is string url && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                        {
                            Open(url);
                        }
                    }
                );
                break;
            default:
                return new TextBlock() 
                    { 
                        [!TextBlock.TextProperty] = content,
                        [!ToolTip.TipProperty] = content 
                    };
        }



        var retVal = new Button()
        {
            [!Button.ContentProperty] = content,
            Command = command,
            CommandParameter = commandParameter,
            [!ToolTip.TipProperty] = new Binding(nameof(resource.Value))
        };
        retVal.Classes.Add("link");


        return retVal;
    }

    private static void Open(string path)
    {
        path = $"\"{path}\"";
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo("cmd", $"/c start {path}"));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", path);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", path);
        }
    }
}
