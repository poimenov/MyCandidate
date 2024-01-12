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
    public CandidateResourceExt(CandidateResource candidateResource)
    {
        if (candidateResource != null)
        {
            CandidateId = candidateResource.CandidateId;
            Candidate = candidateResource.Candidate;
            Id = candidateResource.Id;
            ResourceTypeId = candidateResource.ResourceTypeId;
            ResourceType = candidateResource.ResourceType;
            OnChangeResourceType(ResourceType.Name);
            Value = candidateResource.Value;
        }
        else
        {
            ResourceTypeId = 1;
            ResourceType = new ResourceType { Id = 1, Name = "Path", Enabled = true };
            OnChangeResourceType(ResourceType.Name);
            Value = string.Empty;
        }

        this.WhenAnyValue(x => x.ResourceType)
            .Subscribe(
                x =>
                {
                    if (x != null)
                    {
                        OnChangeResourceType(x.Name);
                    }
                }
            );
    }

    private string _value;
    [Browsable(false)]
    public override string Value
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
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

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Target Path")]
    [Watermark("Document Path")]
    [VisibilityPropertyCondition(nameof(this.ResourceType.Name), "Path")]
    [PathBrowsable(Filters = "All files (*.*)|*.*")]
    public string PathValue
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Phone Number")]
    [Phone]
    [VisibilityPropertyCondition(nameof(this.ResourceType.Name), "Mobile")]
    public string MobileValue
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Email Address")]
    [EmailAddress]
    [VisibilityPropertyCondition(nameof(this.ResourceType.Name), "Email")]
    public string EmailValue
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Web Address")]
    [Url]
    [VisibilityPropertyCondition(nameof(this.ResourceType.Name), "Url")]
    public string UrlValue
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }

    [StringLength(500, MinimumLength = 2)]
    [DisplayName("Skype")]
    [VisibilityPropertyCondition(nameof(this.ResourceType.Name), "Skype")]
    public string SkypeValue
    {
        get => _value;
        set => this.RaiseAndSetIfChanged(ref _value, value);
    }
    /*
    [ConditionTarget]
    [Browsable(false)]
    public bool IsShowPath { get; set; } = true;

    [ConditionTarget]
    [Browsable(false)]
    public bool IsShowMobile { get; set; } = false;

    [ConditionTarget]
    [Browsable(false)]
    public bool IsShowEmail { get; set; } = false;

    [ConditionTarget]
    [Browsable(false)]
    public bool IsShowUrl { get; set; } = false;

    [ConditionTarget]
    [Browsable(false)]
    public bool IsShowSkype { get; set; } = false;
    */
    private void OnChangeResourceType(string resourceTypeName)
    {
        /*
        switch (resourceTypeName)
        {
            case "Mobile":
                IsShowPath = false;
                IsShowMobile = true;
                IsShowEmail = false;
                IsShowUrl = false;
                IsShowSkype = false;                              
                break;
            case "Email":
                IsShowPath = false;
                IsShowMobile = false;
                IsShowEmail = true;
                IsShowUrl = false;
                IsShowSkype = false;
                break;
            case "Url":
                IsShowPath = false;
                IsShowMobile = false;
                IsShowEmail = false;
                IsShowUrl = true;
                IsShowSkype = false;
                break;
            case "Skype":
                IsShowPath = false;
                IsShowMobile = false;
                IsShowEmail = false;
                IsShowUrl = false;
                IsShowSkype = true;
                break;
            default://"Path"
                IsShowPath = true;
                IsShowMobile = false;
                IsShowEmail = false;
                IsShowUrl = false;
                IsShowSkype = false;
                break;
        }
        RaisePropertyChanged(nameof(IsShowPath));
        RaisePropertyChanged(nameof(IsShowMobile));
        RaisePropertyChanged(nameof(IsShowEmail));
        RaisePropertyChanged(nameof(IsShowUrl));
        RaisePropertyChanged(nameof(IsShowSkype));   
        */       
    }
    
}
