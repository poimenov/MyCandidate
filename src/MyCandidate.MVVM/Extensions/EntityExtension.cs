using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.MVVM.Extensions;

public static class EntityExtension
{
    public static bool IsValid(this Entity obj)
    {
        if (obj != null && !Validator.TryValidateObject(obj!, new ValidationContext(obj!), null, true))
        {
            return false;
        }
        return true;
    }
}
