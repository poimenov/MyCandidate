using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Svg;
using Dock.Model.Core;
using MyCandidate.MVVM.ViewModels.Candidates;
using MyCandidate.MVVM.ViewModels.Vacancies;

namespace MyCandidate.MVVM.Converters;

public static class ThemeConverter
{
    public static FuncValueConverter<IDockable, Image> DockableToIconConverter { get; } =
        new FuncValueConverter<IDockable, Image>(
            dockable =>
            {

                var targetType = dockable.GetType();
                var baseUri = new Uri(ResourceTypeNameToSvgPathConverter.BASE_PATH);
                var svgPath = "/Assets/svg/accessories-dictionary-svgrepo-com.svg";
                var typeDictionary = new Dictionary<Type, string> {
                            { typeof(CandidateSearchViewModel), "/Assets/svg/search-svgrepo-com.svg" },
                            { typeof(CandidateViewModel), "/Assets/svg/businessman-svgrepo-com.svg" },
                            { typeof(VacancySearchViewModel), "/Assets/svg/search-svgrepo-com.svg" },
                            { typeof(VacancyViewModel), "/Assets/svg/feedback-svgrepo-com.svg" },
                        };
                if (typeDictionary.ContainsKey(targetType))
                {
                    svgPath = typeDictionary[targetType];
                }

                return new Image
                {
                    Width = 16,
                    Height = 16,
                    Source = new SvgImage
                    {
                        Source = SvgSource.Load(svgPath, baseUri, null)
                    }
                };
            });
}
