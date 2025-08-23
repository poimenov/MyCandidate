using MyCandidate.Common;
using MyCandidate.Common.Interfaces;

namespace MyCandidate.DataAccess;

public class DictionaryCreator
{
    private readonly Database _database;
    public DictionaryCreator(Database database)
    {
        _database = database;
    }

    public void Create()
    {
        using (var transaction = _database.Database.BeginTransaction())
        {
            CreateResourceTypes();
            CreateVacancyStatuses();
            CreateSelectionStatuses();
            CreateSeniorities();
            transaction.Commit();
        }
    }

    private void CreateResourceTypes()
    {
        if (!_database.ResourceTypes.Any())
        {
            _database.ResourceTypes.AddRange(
                new ResourceType { Name = ResourceTypeNames.Path, Enabled = true },
                new ResourceType { Name = ResourceTypeNames.Mobile, Enabled = true },
                new ResourceType { Name = ResourceTypeNames.Email, Enabled = true },
                new ResourceType { Name = ResourceTypeNames.Url, Enabled = true },
                new ResourceType { Name = ResourceTypeNames.Skype, Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateVacancyStatuses()
    {
        if (!_database.VacancyStatuses.Any())
        {
            _database.VacancyStatuses.AddRange(
                new VacancyStatus { Name = VacancyStatusNames.New, Enabled = true },
                new VacancyStatus { Name = VacancyStatusNames.InProgress, Enabled = true },
                new VacancyStatus { Name = VacancyStatusNames.Closed, Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateSelectionStatuses()
    {
        if (!_database.SelectionStatuses.Any())
        {
            _database.SelectionStatuses.AddRange(
                new SelectionStatus { Name = SelectionStatusNames.SetContact, Enabled = true },
                new SelectionStatus { Name = SelectionStatusNames.PreScreen, Enabled = true },
                new SelectionStatus { Name = SelectionStatusNames.TechnicalInterview, Enabled = true },
                new SelectionStatus { Name = SelectionStatusNames.FinalInterview, Enabled = true },
                new SelectionStatus { Name = SelectionStatusNames.Rejected, Enabled = true },
                new SelectionStatus { Name = SelectionStatusNames.Accepted, Enabled = true }
                );
            _database.SaveChanges();
        }
    }

    private void CreateSeniorities()
    {
        if (!_database.Seniorities.Any())
        {
            _database.Seniorities.AddRange(
                new Seniority { Name = SeniorityNames.Unknown, Enabled = true },
                new Seniority { Name = SeniorityNames.Intern, Enabled = true },
                new Seniority { Name = SeniorityNames.Junior, Enabled = true },
                new Seniority { Name = SeniorityNames.Middle, Enabled = true },
                new Seniority { Name = SeniorityNames.Senior, Enabled = true }
                );
            _database.SaveChanges();
        }
    }
}
