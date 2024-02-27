using System.Collections.Generic;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Models;

public interface ISkillValueList
{
    IEnumerable<SkillValue> Skills {get;}
}
