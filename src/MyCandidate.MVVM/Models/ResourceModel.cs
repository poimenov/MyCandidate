using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;
using PropertyModels.ComponentModel.DataAnnotations;

namespace MyCandidate.MVVM.Models
{
    public enum ResourceModelType
    {
        CandidateResource,
        VacancyResource
    }
    public class ResourceModel : ReactiveObject
    {
        public ResourceModel(ResourceModelType resourceModelType)
        {
            ResourceModelType = resourceModelType;
            ResourceTypeId = 1;
            ResourceType = new ResourceType { Id = 1, Name = ResourceTypeNames.Path, Enabled = true };
            Value = string.Empty;
        }

        public ResourceModel(CandidateResource candidateResource)
        {
            ResourceModelType = ResourceModelType.CandidateResource;
            if (candidateResource != null)
            {
                Id = candidateResource.Id;
                ResourceTypeId = candidateResource.ResourceTypeId;
                ResourceType = candidateResource.ResourceType;
                Value = candidateResource.Value;
            }
            else
            {
                ResourceTypeId = 1;
                ResourceType = new ResourceType { Id = 1, Name = ResourceTypeNames.Path, Enabled = true };
                Value = string.Empty;
            }
        }

    public ResourceModel(VacancyResource vacancyResource)
    {
        ResourceModelType = ResourceModelType.VacancyResource;
        if (vacancyResource != null)
        {
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

        [Browsable(false)]
        public ResourceModelType ResourceModelType { get; set; }

        [Browsable(false)]
        public int Id { get; set; }

        [Browsable(false)]
        public int ResourceTypeId { get; set; }

        private string _value;
        [Required]
        [StringLength(500, MinimumLength = 2)]
        [Browsable(false)]
        public string Value
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
        public ResourceType ResourceType
        {
            get => _resourceType;
            set
            {
                this.RaiseAndSetIfChanged(ref _resourceType, value);
            }
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return false;
            }

            var retVal = true;
            var context = new ValidationContext(this);

            switch (ResourceType.Name)
            {
                case ResourceTypeNames.Path:
                    retVal = File.Exists(PathValue);
                    break;
                case ResourceTypeNames.Url:
                    retVal = Uri.IsWellFormedUriString(UrlValue, UriKind.Absolute);
                    break;
                case ResourceTypeNames.Mobile:
                    context.MemberName = nameof(MobileValue);
                    retVal = Validator.TryValidateProperty(MobileValue, context, null);
                    break;
                case ResourceTypeNames.Email:
                    context.MemberName = nameof(EmailValue);
                    retVal = Validator.TryValidateProperty(EmailValue, context, null);
                    break;
            }

            return retVal;
        }

        public CandidateResource ToCandidateResource(Candidate candidate)
        {
            return new CandidateResource
            {
                Id = this.Id,
                Candidate = candidate,
                CandidateId = candidate.Id,
                ResourceType = this.ResourceType,
                ResourceTypeId = this.ResourceTypeId,
                Value = this.Value
            };
        }

        public VacancyResource ToVacancyResource(Vacancy vacancy)
        {
            return new VacancyResource
            {
                Id = this.Id,
                Vacancy = vacancy,
                VacancyId = vacancy.Id,
                ResourceType = this.ResourceType,
                ResourceTypeId = this.ResourceTypeId,
                Value = this.Value
            };
        }
    }
}
