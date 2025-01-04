using Avalonia.Controls.Templates;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.Metadata;
using System;
using System.Collections.Generic;
using Location = RemnantOverseer.Models.Location;
using RemnantOverseer.Models;

// Why?
// https://stackoverflow.com/questions/74003533/avalonia-treeview-template-selector

namespace RemnantOverseer.Controls;
internal class ZoneTreeTemplateSelector : ITreeDataTemplate
{
    [Content]
    public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = [];

    public InstancedBinding? ItemsSelector(object item)
    {
        var key = $"{item.GetType().Name}Template";
        return ((TreeDataTemplate)AvailableTemplates[key]).ItemsSelector(item);

        //var key = GetKey(item);
        //return ((TreeDataTemplate)AvailableTemplates[key]).ItemsSelector(item);

        //else
        //    return null;
    }

    // Check if we can accept the provided data
    public bool Match(object? data)
    {
        // Do a stronger check here?
        return data != null;
        //return (data is Zone || data is Location || data is Item);
    }

    // Build the DataTemplate here
    Control? ITemplate<object?, Control?>.Build(object? param)
    {
        if (param == null) return null;
        var key = $"{param.GetType().Name}Template";
        return AvailableTemplates[key].Build(param);

        //if (param == null) return null;
        //// check whats here
        //string key = param.GetType().Name;

        //// decide which template to build and return
        //key = GetKey(param);
        //return AvailableTemplates[key].Build(param);
    }

    private string GetKey(object obj)
    {
        return obj switch
        {
            Zone => "ZoneTemplate",
            Location => "LocationTemplate",
            Item => "ItemTemplate",
            _ => throw new ArgumentException("Unknown type in passed to the selector")
        };
    }
}