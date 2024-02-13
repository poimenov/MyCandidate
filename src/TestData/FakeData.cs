using Bogus;
using MyCandidate.Common;
using System.Diagnostics.CodeAnalysis;

namespace TestData
{
    public static class FakeData
{
    public static IEnumerable<Candidate> GetCandidates(int count, int cityCount, int skillsCount)
    {
        var phoneFaker = new Faker<CandidateResource>()
            .RuleFor(x => x.ResourceTypeId, f => 2)
            .RuleFor(x => x.Value, f => f.Person.Phone);

        var emailFaker = new Faker<CandidateResource>()
            .RuleFor(x => x.ResourceTypeId, f => 3)
            .RuleFor(x => x.Value, f => f.Person.Email);

        var urlFaker = new Faker<CandidateResource>()
            .RuleFor(x => x.ResourceTypeId, f => 4)
            .RuleFor(x => x.Value, f => f.Person.Avatar);

        var skypeFaker = new Faker<CandidateResource>()
            .RuleFor(x => x.ResourceTypeId, f => 5)
            .RuleFor(x => x.Value, f => f.Person.UserName);

        var locationFaker = new Faker<Location>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.CityId, f => f.Random.Int(1, cityCount))
            .RuleFor(x => x.Address, f => $"{f.Person.Address.Suite}, {f.Person.Address.Street}");

        var skillFaker = new Faker<CandidateSkill>()
            .RuleFor(x => x.SkillId, f => f.Random.Int(1, skillsCount))
            .RuleFor(x => x.SeniorityId, f => f.Random.Int(1, 5));

        var candidateFaker = new Faker<Candidate>()
            .RuleFor(x => x.LastName, f => f.Person.LastName)
            .RuleFor(x => x.FirstName, f => f.Person.FirstName)
            .RuleFor(x => x.BirthDate, f => f.Person.DateOfBirth)
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.CreationDate, f => f.Date.Between(DateTime.Today.AddMinutes(-40), DateTime.Today.AddMinutes(-30)))
            .RuleFor(x => x.LastModificationDate, f => f.Date.Between(DateTime.Today.AddMinutes(-20), DateTime.Today.AddMinutes(-10)))
            .RuleFor(x => x.CreationDate, f => f.Date.Between(DateTime.Today.AddMinutes(-40), DateTime.Today.AddMinutes(-30)))
            .RuleFor(x => x.Location, (f, x) =>            
            {
                return locationFaker.Generate();
            })
            .RuleFor(x => x.CandidateSkills, (f, x) => 
            {
                var skills =  skillFaker.Generate(f.Random.Int(1, 12));                
                return skills.Distinct(new CandidateSkillComparer()).ToList();
            })
            .RuleFor(x => x.CandidateResources, (f, x) =>
            {
                return new List<CandidateResource>
                {
                    phoneFaker.Generate(),
                    emailFaker.Generate(),
                    urlFaker.Generate(),
                    skypeFaker.Generate()
                };               
            });
            var retVal = new List<Candidate>();
            retVal.AddRange(candidateFaker.Generate(count));
            return retVal;
    }

    public static IEnumerable<Country> GetCountries(int countryCount, int cityCount)
    {
        var cityFaker = new Faker<City>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.Name, f => f.Address.City());


        var countryFaker = new Faker<Country>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.Name, f => f.Address.Country())
            .RuleFor(x => x.Cities, (f, x) =>
            {
                return cityFaker.Generate(cityCount);
            });

        return countryFaker.Generate(countryCount);
    }

    public static IEnumerable<Company> GetCompanies(int companyCount, int officeCount, int cityCount)
    {
        var locationFaker = new Faker<Location>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.CityId, f => f.Random.Int(1, cityCount))
            .RuleFor(x => x.Address, f => $"{f.Person.Address.Suite}, {f.Person.Address.Street}");

        var officeFaker = new Faker<Office>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.Location, (f, x) =>            
            {
                return locationFaker.Generate();
            })            
            .RuleFor(x => x.Name, f => "Office");


        var companyFaker = new Faker<Company>()
            .RuleFor(x => x.Enabled, f => true)
            .RuleFor(x => x.Name, f => f.Company.CompanyName())
            .RuleFor(x => x.Officies, (f, x) =>
            {
                return officeFaker.Generate(officeCount);
            });

        return companyFaker.Generate(companyCount);
    }    
}

    public class CandidateSkillComparer : IEqualityComparer<CandidateSkill>
    {
        public bool Equals(CandidateSkill? x, CandidateSkill? y)
        {
            if (Object.ReferenceEquals(x, y)) return true;
            return x.SkillId == y.SkillId;
        }

        public int GetHashCode([DisallowNull] CandidateSkill obj)
        {
            return obj.SkillId.GetHashCode();
        }
    }
}
