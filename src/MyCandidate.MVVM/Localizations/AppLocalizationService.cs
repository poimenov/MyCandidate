using Avalonia.PropertyGrid.Localization;

namespace MyCandidate.MVVM.Localizations;

public class AppLocalizationService : AssemblyJsonAssetLocalizationService
    {
        public AppLocalizationService() :
            base(typeof(AppLocalizationService).Assembly)
        {
        }
}
