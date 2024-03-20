using System.Xml.Linq;
using MyCandidate.Common;

namespace MyCandidate.MVVM.Extensions;

public static class VacancyExtension
{
    public static XElement ToXml(this Vacancy obj)
    {
        XElement retVal = new XElement("Vacancy", new XAttribute("title", obj.Name),
                                                new XAttribute("enabled", obj.Enabled),
                                                new XAttribute("created", obj.CreationDate),
                                                new XAttribute("modified", obj.LastModificationDate));
        retVal.Add(new XElement("Description", new XCData(obj.Description)));
        retVal.Add(new XElement("VacancyStatus", new XAttribute("name", obj.VacancyStatus.Name)));
        var company = new XElement("Company", new XAttribute("name", obj.Office.Company.Name));
        var office = new XElement("Office", new XAttribute("name", obj.Office.Name));
        var location = new XElement("Location", new XAttribute("country", obj.Office.Location.City.Country.Name),
                                                new XAttribute("city", obj.Office.Location.City.Name), 
                                                new XAttribute("address", obj.Office.Location.Address));
        office.Add(location);
        company.Add(office);
        retVal.Add(company);

        var skills = new XElement("Skills");
        foreach(var skill in  obj.VacancySkills)
        {
            skills.Add(new XElement("Skill", new XAttribute("name", skill.Skill.Name), 
                                            new XAttribute("seniority", skill.Seniority.Name)));
        }
        retVal.Add(skills);

        var resources = new XElement("Resources");
        foreach(var resource in  obj.VacancyResources)
        {
            resources.Add(new XElement("Resource", new XAttribute("type", resource.ResourceType.Name), 
                                            new XAttribute("value", resource.Value)));
        }
        retVal.Add(resources);        

        return retVal;
    }
}
