using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using MyCandidate.Common;
using MyCandidate.Common.Interfaces;
using PropertyModels.ComponentModel;

namespace MyCandidate.MVVM.Models;

public class CandidateModel : ReactiveObject, ISkillValueList
{
    private readonly Candidate _candidate;
    public CandidateModel(Candidate candidate)
    {
        _candidate = candidate;
    }

    [Browsable(false)]
    public Candidate Candidate => _candidate;

    [Browsable(false)]
    public string FirstName => _candidate.FirstName;

    [Browsable(false)]
    public string LastName => _candidate.LastName;

    [Browsable(false)]
    public string CountryName => _candidate.Location.City.Country.Name;

    [Browsable(false)]
    public string CityName => _candidate.Location.City.Name;

    [DisplayName("Full_Name")]
    [Category("Main")]
    public string Name => _candidate.Name;

    [DisplayName("Date_Birth")]
    [Category("Main")]
    public string BirthDate => _candidate.BirthDate.ToLongDateString();

    [DisplayName("Address")]
    [Category("Main")]
    public string Address => _candidate.Location.Name;

    [DisplayName("Enabled")]
    [Category("Main")]
    public bool Enabled => _candidate.Enabled;    

    [DisplayName("Skills")]
    [Category("Main")]
    public IEnumerable<SkillValue> Skills => _candidate.CandidateSkills.Select(x => new SkillValue(x.SkillId, x.SeniorityId));

    [DisplayName("Creation_Date")]
    public string Created => _candidate.CreationDate.ToString("G");

    [DisplayName("Last_Modification_Date")]
    public string Updated => _candidate.LastModificationDate.ToString("G");
}
