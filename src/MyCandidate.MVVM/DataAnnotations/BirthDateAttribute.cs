using System;
using System.ComponentModel.DataAnnotations;
using Avalonia.PropertyGrid.Services;
using MyCandidate.MVVM.Extensions;

namespace MyCandidate.MVVM.DataAnnotations;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class BirthDateAttribute : ValidationAttribute
{
    private readonly bool _required; 
    private const int MIN_AGE = 18;

    public BirthDateAttribute()
    {
        _required = true;
    }

    public BirthDateAttribute(bool required)
    {
        _required = required;
    }   

    public bool Required
    {
        get => _required;
    }

    public override bool IsValid(object value)
    {
        if(value == null)
        {
            return !Required;
        }

        DateTime birthDate;
        if(value is DateTimeOffset dateTimeOffset)
        {
            birthDate = dateTimeOffset.Date;
        }
        else if(value is DateTime dateTime)
        {
            birthDate = dateTime;
        }
        else
        {
            throw new ArgumentException();
        }

        if(birthDate > DateTime.Today || birthDate.GetAge() < MIN_AGE)
        {
            return false;
        }

        return true;
    }

    public override string FormatErrorMessage(string name)
    {
        return string.Format(LocalizationService.Default["BirthdateValidationMessage"], MIN_AGE);
    }
}
