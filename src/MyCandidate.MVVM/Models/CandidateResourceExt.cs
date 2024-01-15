using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyCandidate.Common;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace MyCandidate.MVVM.Models;

public class CandidateResourceExt : CandidateResource
{
    public CandidateResourceExt()
    {
        ResourceTypeId = 1;
        ResourceType = new ResourceType { Id = 1, Name = "Path", Enabled = true };
        Value = string.Empty;
    }

    public CandidateResourceExt(CandidateResource candidateResource)
    {
        if (candidateResource != null)
        {
            CandidateId = candidateResource.CandidateId;
            Candidate = candidateResource.Candidate;
            Id = candidateResource.Id;
            ResourceTypeId = candidateResource.ResourceTypeId;
            ResourceType = candidateResource.ResourceType;
            Value = candidateResource.Value;
        }
        else
        {
            ResourceTypeId = 1;
            ResourceType = new ResourceType { Id = 1, Name = "Path", Enabled = true };
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
    [DisplayName("Phone_Number")]
    [Phone]
    [VisibilityPropertyCondition(nameof(ResourceTypeId), 2)]
    public string MobileValue
    {
        get => _value;
        set
        {
            this.RaiseAndSetIfChanged(ref _value, value);
            RaisePropertyChanged(nameof(Value));
        }  
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Email_Address")]
    [EmailAddress]
    [VisibilityPropertyCondition(nameof(ResourceTypeId), 3)]
    public string EmailValue
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

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Skype")]
    [VisibilityPropertyCondition(nameof(ResourceTypeId), 5)]
    public string SkypeValue
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
}
