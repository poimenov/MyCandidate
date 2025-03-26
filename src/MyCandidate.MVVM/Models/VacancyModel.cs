using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;

namespace MyCandidate.MVVM.Models;

public class VacancyModel : ReactiveObject, ISkillValueList
{
    private readonly Vacancy _vacancy;
    public VacancyModel(Vacancy vacancy)
    {
        _vacancy = vacancy;
    }

    [Browsable(false)]
    public Vacancy Vacancy => _vacancy;

    [DisplayName("Name")]
    [Category("Main")]
    public string Name => _vacancy.Name;   

    [DisplayName("Company")]
    [Category("Main")]
    public string CompanyName => _vacancy?.Office?.Company?.Name ?? string.Empty;
    
    [DisplayName("Office")]
    [Category("Main")]
    public string OfficeName => _vacancy?.Office?.Name ?? string.Empty;   
    
    [DisplayName("Vacancy_Status")]
    [Category("Main")]
    public string VacancyStatusName => _vacancy?.VacancyStatus?.Name ?? string.Empty;  

    [DisplayName("Enabled")]
    [Category("Main")]
    public bool Enabled => _vacancy.Enabled;             

    [DisplayName("Skills")]
    [Category("Main")]
    public IEnumerable<SkillValue> Skills => _vacancy.VacancySkills.Select(x => new SkillValue(x.SkillId, x.SeniorityId));      

    [DisplayName("Description")]
    [MultilineText]
    [Category("Main")]
    public string Description => _vacancy.Description; 

    [DisplayName("Creation_Date")]
    public string Created => _vacancy.CreationDate.ToString("G");

    [DisplayName("Last_Modification_Date")]
    public string Updated => _vacancy.LastModificationDate.ToString("G");        
}
