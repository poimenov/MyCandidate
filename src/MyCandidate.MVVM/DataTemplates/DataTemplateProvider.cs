using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.PropertyGrid.Services;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Converters;
using MyCandidate.MVVM.Models;
using ReactiveUI;

namespace MyCandidate.MVVM.DataTemplates;

public static class DataTemplateProvider
{                
    public static FuncDataTemplate<ResourceType> ResourceTypeName { get; }
        = new FuncDataTemplate<ResourceType>(
            (resourceType) => resourceType is not null, BuildResourceTypeName);

    private static Control BuildResourceTypeName(ResourceType resourceType)
    {
        return new TextBlock
                {
                    Text = GetComboBoxItemText(resourceType?.Name),
                    Margin = new Thickness(4, 0),
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
                };
    }            

    public static FuncDataTemplate<ResourceType> ResourceTypeImage { get; }
        = new FuncDataTemplate<ResourceType>(
            (resourceType) => resourceType is not null, BuildResourceTypeImage);

    private static Control BuildResourceTypeImage(ResourceType resourceType)
    {
        return new Avalonia.Svg.Svg(new Uri(ResourceTypeNameToSvgPathConverter.BASE_PATH))
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

    public static FuncDataTemplate<ResourceModel> ResourceLink { get; }
            = new FuncDataTemplate<ResourceModel>(
                (resource) => resource is not null,
                BuildResourceLink);

    private static Control BuildResourceLink(ResourceModel resource)
    {
        ICommand? command = null;
        var content = new Binding(nameof(resource.Value));
        content.Converter = new ResourceValueConverter();
        switch (resource.ResourceType.Name)
        {
            case "Path":
            case "Url":
                command = ReactiveCommand.Create(
                    () =>
                    {
                        Open(resource.Value);
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
            [!ToolTip.TipProperty] = new Binding(nameof(resource.Value)) 
        };
        
        retVal.Classes.Add("link");

        return retVal;
    }

    public static void Open(string path)
    {
        if (File.Exists(path) || Uri.IsWellFormedUriString(path, UriKind.Absolute))
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

    private static string GetComboBoxItemText(string itemName)
    {
        string retVal = "Unknown Resource type";
        switch(itemName)
        {
            case ResourceTypeNames.Path:
                retVal = LocalizationService.Default["Target_Path"];
                break;
            case ResourceTypeNames.Mobile:
                retVal = LocalizationService.Default["Phone_Number"];
                break;
            case ResourceTypeNames.Email:
                retVal = LocalizationService.Default["Email_Address"];
                break;
            case ResourceTypeNames.Url:
                retVal = LocalizationService.Default["Web_Address"];
                break;
            case ResourceTypeNames.Skype:
                retVal = LocalizationService.Default["Skype"];
                break;                                                
        }

        return retVal;
    }    
}
