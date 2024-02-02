using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace MyCandidate.MVVM.Models;

public class VacancyResourceExt : VacancyResource
{
    public VacancyResourceExt()
    {
        ResourceTypeId = 1;
        ResourceType = new ResourceType { Id = 1, Name = ResourceTypeNames.Path, Enabled = true };
        Value = string.Empty;        
    }

    public VacancyResourceExt(VacancyResource vacancyResource)
    {
        if (vacancyResource != null)
        {
            VacancyId = vacancyResource.VacancyId;
            Vacancy = vacancyResource.Vacancy;
            Id = vacancyResource.Id;
            ResourceTypeId = vacancyResource.ResourceTypeId;
            ResourceType = vacancyResource.ResourceType;
            Value = vacancyResource.Value;
        }
        else
        {
            ResourceTypeId = 1;
            ResourceType = new ResourceType { Id = 1, Name = ResourceTypeNames.Path, Enabled = true };
            Value = string.Empty;
        }        
    }

    private string _value;
    [Browsable(false)]
    public override string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Target_Path")]
    [Watermark("Document Path")]
    [VisibilityPropertyCondition(nameof(ResourceTypeId), 1)]
    [PathBrowsable(Filters = "All files (*.*)|*.*")]
    public string PathValue
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            RaisePropertyChanged(nameof(Value));
        }        
    } 

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Web_Address")]
    [Url]
    [VisibilityPropertyCondition(nameof(ResourceTypeId), 4)]
    public string UrlValue
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            RaisePropertyChanged(nameof(Value));
        }  
    }  

    private ResourceType _resourceType;
    [Required]
    [ConditionTarget]
    [DisplayName("Resource_Type")]
    public override ResourceType ResourceType
    {
        get => _resourceType;
        set
        {
            this.RaiseAndSetIfChanged(ref _resourceType, value);
        }
    } 

    public bool IsValid()
    {
        if(string.IsNullOrWhiteSpace(Value))
        {
            return false;
        }

        var retVal = true;
        
        switch(ResourceType.Name)
        {
            case ResourceTypeNames.Path:
                retVal = File.Exists(PathValue);
                break;
            case ResourceTypeNames.Url:
                retVal = Uri.IsWellFormedUriString(UrlValue, UriKind.Absolute);
                break;                                         
        }

        return retVal;
    } 

    public VacancyResource ToVacancyResource()
    {
        return new VacancyResource
        {
            Id = this.Id,
            Vacancy = this.Vacancy,
            VacancyId = this.VacancyId,
            ResourceType = this.ResourceType,
            ResourceTypeId = this.ResourceTypeId,
            Value = this.Value             
        };
    }            
}
