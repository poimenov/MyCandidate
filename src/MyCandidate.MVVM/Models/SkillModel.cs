using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;

namespace MyCandidate.MVVM.Models;

public class SkillModel : ReactiveObject
{
    public SkillModel()
    {
        //
    }
    public SkillModel(Skill skill, Seniority seniority)
    {
        this.Skill = skill;
        this.Seniority = seniority;
    }
    public SkillModel(int id, Skill skill, Seniority seniority)
    {
        this.Id = id;
        this.Skill = skill;
        this.Seniority = seniority;
    }
    [Browsable(false)]
    public int Id { get; set; }

    private Skill? _skill;
    [Required]
    [DisplayName("Skill")]
    public Skill? Skill
    {
        get => _skill;
        set => this.RaiseAndSetIfChanged(ref _skill, value);
    }

    private Seniority? _seniority;
    [Required]
    [DisplayName("Seniority")]
    public Seniority? Seniority
    {
        get => _seniority;
        set => this.RaiseAndSetIfChanged(ref _seniority, value);
    }

    public CandidateSkill ToCandidateSkill(Candidate candidate)
    {
        return new CandidateSkill
        {
            Id = this.Id,
            CandidateId = candidate.Id,
            Candidate = candidate,
            SkillId = this.Skill!.Id,
            Skill = this.Skill,
            SeniorityId = this.Seniority!.Id,
            Seniority = this.Seniority
        };
    }

    public VacancySkill ToVacancySkill(Vacancy vacancy)
    {
        return new VacancySkill
        {
            Id = this.Id,
            VacancyId = vacancy.Id,
            Vacancy = vacancy,
            SkillId = this.Skill!.Id,
            Skill = this.Skill,
            SeniorityId = this.Seniority!.Id,
            Seniority = this.Seniority
        };
    }

    public SkillValue ToSkillVaue()
    {
        return new SkillValue(this.Skill!.Id, this.Seniority!.Id);
    }
}
