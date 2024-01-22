using System;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.PropertyGrid.Controls;
using Avalonia.PropertyGrid.Controls.Factories;
using DynamicData;
using DynamicData.Binding;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using MyCandidate.MVVM.ViewModels.Tools;

namespace MyCandidate.MVVM.Views.Tools.CellEdit;

public class SkillCellEditFactory : AbstractCellEditFactory
{
    private readonly IDataAccess<SkillCategory> _skillCategories;
    private readonly IDataAccess<Skill> _skills;  
    private readonly IVacancySkills _vacancySkills; 
    private readonly ICandidateSkills _candidateSkills; 
    
    public SkillCellEditFactory()
    {
        _skillCategories = ((App)Application.Current).GetRequiredService<IDataAccess<SkillCategory>>();
        _skills = ((App)Application.Current).GetRequiredService<IDataAccess<Skill>>();
        _vacancySkills = ((App)Application.Current).GetRequiredService<IVacancySkills>();
        _candidateSkills = ((App)Application.Current).GetRequiredService<ICandidateSkills>();
    }
    
    public SkillCellEditFactory(IDataAccess<SkillCategory> skillCategories, 
    IDataAccess<Skill> skills, IVacancySkills vacancySkills, ICandidateSkills candidateSkills)
    {
        _skillCategories = skillCategories;
        _skills = skills;
        _vacancySkills = vacancySkills;
        _candidateSkills = candidateSkills;
    }

    public override bool Accept(object accessToken)
    {
        return accessToken is ExtendedPropertyGrid;
    }    

    public override Control HandleNewProperty(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        if (propertyDescriptor.PropertyType != typeof(Skill))
        {
            return null;
        }

        var vm = new SkillViewModel(_skillCategories, _skills);
        var control = new SkillView
        {
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Stretch,
            DataContext = vm
        };

        vm.WhenPropertyChanged(x => x.Skill)
        .Subscribe(
            x =>
            {
                if (x.Value is Skill skill)
                {
                    if(target is CandidateSkill candidateSkill)
                    {
                        if(candidateSkill.Id != 0)
                        {
                            var originalCandidateSkill = _candidateSkills.Get(candidateSkill.Id);
                            if(originalCandidateSkill.SkillId != skill.Id)
                            {
                                candidateSkill.RaisePropertyChanged(nameof(CandidateSkill.Skill));
                            }
                        }
                        else
                        {
                            candidateSkill.RaisePropertyChanged(nameof(CandidateSkill.Skill));
                            candidateSkill.Skill.RaisePropertyChanged(nameof(CandidateSkill.Skill.Name));
                        }                        
                    }
                    else if(target is VacancySkill vacancySkill)
                    {
                        if(vacancySkill.Id != 0)
                        {
                            var originalVacancySkill = _vacancySkills.Get(vacancySkill.Id);
                            if(originalVacancySkill.SkillId != skill.Id)
                            {
                                vacancySkill.RaisePropertyChanged(nameof(VacancySkill.Skill));
                            }
                        }
                        else
                        {
                            vacancySkill.RaisePropertyChanged(nameof(VacancySkill.Skill));
                            vacancySkill.Skill.RaisePropertyChanged(nameof(VacancySkill.Skill.Name));
                        }
                    }
                    SetAndRaise(context, context.CellEdit, skill);
                }
            }
        );        

        return control;
    }

    public override bool HandlePropertyChanged(PropertyCellContext context)
    {
        var propertyDescriptor = context.Property;
        var target = context.Target;
        var control = context.CellEdit;

        if (propertyDescriptor.PropertyType != typeof(Skill))
        {
            return false;
        }

        ValidateProperty(control, propertyDescriptor, target);

        if (control is SkillView vw
                && vw.DataContext is SkillViewModel vm)
        {
            if(target is CandidateSkill candidateSkill)
            {
                if(candidateSkill.Skill == null)
                {
                    candidateSkill.Skill = _skills.ItemsList.First();
                }

                vm.Skill = candidateSkill.Skill;
                return true;
            }
            else if(target is VacancySkill vacancySkill)
            {
                if(vacancySkill.Skill == null)
                {
                    vacancySkill.Skill = _skills.ItemsList.First();
                }

                vm.Skill = vacancySkill.Skill;
                return true;
            }
        }

        return false;
    }
}
