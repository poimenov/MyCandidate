using System.Xml.Linq;
using MyCandidate.Common;

namespace MyCandidate.MVVM.Extensions;

public static class CandidateExtension
{
    public static XElement ToXml(this Candidate obj)
    {
        XElement retVal = new XElement("Candidate", new XAttribute("firstName", obj.FirstName),
                                                new XAttribute("lastName", obj.LastName),
                                                new XAttribute("enabled", obj.Enabled),
                                                new XAttribute("title", obj.Title ?? string.Empty),
                                                new XAttribute("created", obj.CreationDate),
                                                new XAttribute("modified", obj.LastModificationDate));

        var location = new XElement("Location", new XAttribute("country", obj.Location.City.Country.Name),
                                                new XAttribute("city", obj.Location.City.Name), 
                                                new XAttribute("address", obj.Location.Address)); 
        retVal.Add(location);  

        var skills = new XElement("Skills");
        foreach(var skill in  obj.CandidateSkills)
        {
            skills.Add(new XElement("Skill", new XAttribute("name", skill.Skill.Name), 
                                            new XAttribute("seniority", skill.Seniority.Name)));
        }
        retVal.Add(skills);

        var resources = new XElement("Resources");
        foreach(var resource in  obj.CandidateResources)
        {
            resources.Add(new XElement("Resource", new XAttribute("type", resource.ResourceType.Name), 
                                                new XAttribute("value", resource.Value)));
        }
        retVal.Add(resources);                                                      

        return retVal;
    }
}
