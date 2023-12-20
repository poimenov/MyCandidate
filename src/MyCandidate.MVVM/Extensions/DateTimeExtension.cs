using System;

namespace MyCandidate.MVVM.Extensions;

public static class DateTimeExtension
{
    public static string GetAge(this DateTime birthday)
    {
        DateTime now = DateTime.Today;
        int age = now.Year - birthday.Year;
        if (now < birthday.AddYears(age))
            age--;

        return age.ToString();
    }
}
