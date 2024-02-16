namespace TestData
{
    public static class NameExtension
    {
        private static readonly string[] ItJobTitles =
           {
            "Computer systems manager",
            "Network architect",
            "Systems analyst",
            "IT coordinator",
            "Network administrator",
            "Network engineer",
            "Service desk analyst",
            "System administrator",
            "Wireless network engineer",
            "Database administrator",
            "Database analyst",
            "Data quality manager",
            "Database report writer",
            "SQL database administrator",
            "Big data engineer/architect",
            "Business intelligence specialist/analyst",
            "Business systems analyst",
            "Data analyst",
            "Data analytics developer",
            "Data modeling analyst",
            "Data scientist",
            "Data warehouse manager",
            "Data warehouse programming specialist",
            "Intelligence specialist",
            "Back-end developer",
            "Cloud/software architect",
            "Cloud/software developer",
            "Cloud/software applications engineer",
            "Cloud system administrator",
            "Cloud system engineer",
            "DevOps engineer",
            "Front-end developer",
            "Full-stack developer",
            "Java developer",
            "Platform engineer",
            "Release manager",
            "Reliability engineer",
            "Software engineer",
            "Software quality assurance analyst",
            "UI designer",
            "UX designer",
            "Web developer",
            "Application security administrator",
            "Artificial intelligence security specialist",
            "Cloud security specialist",
            "Cybersecurity hardware engineer",
            "Cyberintelligence specialist",
            "Cryptographer",
            "Data privacy officer",
            "Digital forensics analyst",
            "IT security engineer",
            "Information assurance analyst",
            "Security systems administrator",
            "Help desk support specialist",
            "IT support specialist",
            "Customer service representative",
            "Technical product manager",
            "Product manager",
            "Project manager",
            "Program manager",
            "Portfolio manager"
         };

        public static string ItJobTitle(this Bogus.DataSets.Name name)
        {
            return name.Random.ArrayElement(ItJobTitles);
        }
    }
}

