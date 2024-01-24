using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Avalonia.PropertyGrid.Services;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.MVVM.ViewModels.Tools;
using ReactiveUI;
using System.ComponentModel;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.ViewModels.Candidates;

public class CandidateSkillsViewModel : ViewModelBase
{
    private readonly Candidate _candidate;
    public CandidateSkillsViewModel(Candidate candidate, IProperties properties)
    {
        _candidate = candidate;
        Properties = properties;
        LocalizationService.Default.OnCultureChanged += CultureChanged;
        SourceCandidateSkills = new ObservableCollectionExtended<CandidateSkill>(_candidate.CandidateSkills);
        SourceCandidateSkills.ToObservableChangeSet()
            .ObserveOn(RxApp.MainThreadScheduler)
            .Bind(out _candidateSkills)
            .Subscribe();

        _candidateSkills.ToList().ForEach(x => x.PropertyChanged += ItemPropertyChanged);

        this.WhenAnyValue(x => x.SelectedCandidateSkill)
            .Subscribe(
                x =>
                {
                    if (Properties != null)
                    {
                        Properties.SelectedItem = x;
                        Properties.SelectedTypeName = LocalizationService.Default["CandidateSkill"]; ;
                    }
                }
            );

        DeleteCandidateSkillCmd = ReactiveCommand.Create(
            (object obj) =>
            {
                SourceCandidateSkills.Remove((CandidateSkill)obj);
            }
        );

        CreateCandidateSkillCmd = ReactiveCommand.Create(
            () =>
            {
                var _newCandidateSkill = new CandidateSkill()
                {
                    Candidate = _candidate,
                    CandidateId = _candidate.Id,
                    //TODO: add skill and senioriry values
                    Seniority = new Seniority
                    {
                        Id = 1,
                        Name = SeniorityNames.Unknown,
                        Enabled =true
                    }                                        
                };
                SourceCandidateSkills.Add(_newCandidateSkill);
                _newCandidateSkill.PropertyChanged += ItemPropertyChanged;
                SelectedCandidateSkill = _newCandidateSkill;
            }
        );            
    }

    private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        this.RaisePropertyChanged(nameof(IsValid));
    }

    public bool IsValid
    {
        get
        {
            if(CandidateSkills.Select(x => x.Skill).Distinct().Count() != CandidateSkills.Count())
            {
                return false;
            }
            return true;
        }
    }    

    private void CultureChanged(object? sender, EventArgs e)
    {
        if (Properties != null)
        {
            Properties.SelectedTypeName = LocalizationService.Default["CandidateSkill"];
        }
    }

    public IProperties? Properties { get; set; }

    #region CandidateSkills
    public ObservableCollectionExtended<CandidateSkill> SourceCandidateSkills;
    private readonly ReadOnlyObservableCollection<CandidateSkill> _candidateSkills;
    public ReadOnlyObservableCollection<CandidateSkill> CandidateSkills => _candidateSkills;
    #endregion

    #region SelectedCandidateSkill
    private CandidateSkill? _selectedCandidateSkill;
    public CandidateSkill? SelectedCandidateSkill
    {
        get => _selectedCandidateSkill;
        set => this.RaiseAndSetIfChanged(ref _selectedCandidateSkill, value);
    }
    #endregion  

    public IReactiveCommand CreateCandidateSkillCmd { get; }
    public IReactiveCommand DeleteCandidateSkillCmd { get; }        
}
