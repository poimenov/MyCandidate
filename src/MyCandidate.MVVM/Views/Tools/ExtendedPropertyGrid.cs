using Avalonia;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Services;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.Views.Tools.CellEdit;

namespace MyCandidate.MVVM.Views.Tools;

public class ExtendedPropertyGrid : PropertyGrid
{
    private static App CurrentApplication => (App)Application.Current;
    static ExtendedPropertyGrid()
    {
        CellEditFactoryService.Default.AddFactory(new CountryCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Country>>()));
        CellEditFactoryService.Default.AddFactory(new SkillCategoryCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<SkillCategory>>()));
        CellEditFactoryService.Default.AddFactory(new CompanyCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Company>>()));
        CellEditFactoryService.Default.AddFactory(new LocationCellEditFactory(CurrentApplication.GetRequiredService<IDataAccess<Country>>(),
                                                                                CurrentApplication.GetRequiredService<IDataAccess<City>>(),
                                                                                CurrentApplication.GetRequiredService<IDataAccess<Office>>()));
        CellEditFactoryService.Default.AddFactory(new ResourceTypeCellEditFactory(CurrentApplication.GetRequiredService<IDictionariesDataAccess>()));
    }
}

