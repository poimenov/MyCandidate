using System;
using System.Collections.Generic;
using Dock.Avalonia.Controls;
using Dock.Model.Controls;
using Dock.Model.Core;
using Dock.Model.ReactiveUI;
using Dock.Model.ReactiveUI.Controls;
using MyCandidate.MVVM.ViewModels.Tools;

namespace MyCandidate.MVVM.ViewModels;

public class DockFactory : Factory
{
    private IRootDock? _rootDock;
    private IDocumentDock? _documentDock;
    private IProportionalDock? _rightDock;
    private ITool? _properties;

    public DockFactory()
    {
    }
    
    public override IRootDock CreateLayout()
    {                
        var documentDock = new DocumentDock
        {
            IsCollapsable = false,
            VisibleDockables = CreateList<IDockable>(),
            CanCreateDocument = false
        };  
        var properties = new PropertiesViewModel();
        var rightDock = new ProportionalDock
        {
            Proportion = 0.25,
            CanClose = false,
            IsCollapsable = false,
            Orientation = Orientation.Vertical,
            ActiveDockable = null,
            VisibleDockables = CreateList<IDockable>
            (
                new ToolDock
                {
                    ActiveDockable = properties,
                    CanClose = false,
                    IsCollapsable = false,
                    VisibleDockables = CreateList<IDockable>(properties),
                    Alignment = Alignment.Right,
                    GripMode = GripMode.Visible
                }
            )
        };  

        var mainLayout = new ProportionalDock
        {
            Orientation = Orientation.Horizontal,
            VisibleDockables = CreateList<IDockable>
            (
                documentDock,
                new ProportionalDockSplitter(),
                rightDock
            )
        };                    
                
        var rootDock = CreateRootDock();
        rootDock.IsCollapsable = false;
        rootDock.ActiveDockable = mainLayout;
        rootDock.DefaultDockable = mainLayout;
        _documentDock = documentDock;
        _rightDock = rightDock;
        _rootDock = rootDock;
        _properties = properties;

        return rootDock;
    }

    public override void InitLayout(IDockable layout)
    {
        ContextLocator = new Dictionary<string, Func<object?>>
        {
            ["Documents"] = () => layout,
            ["Properties"] = () => layout
        };

        DockableLocator = new Dictionary<string, Func<IDockable?>>()
        {
            ["Root"] = () => _rootDock,
            ["Documents"] = () => _documentDock,
            ["Tools"] = () => _rightDock,
            ["Properties"] = () => _properties
        };

        HostWindowLocator = new Dictionary<string, Func<IHostWindow?>>
        {
            [nameof(IDockWindow)] = () => new HostWindow()
        };

        base.InitLayout(layout);
    }    
}
