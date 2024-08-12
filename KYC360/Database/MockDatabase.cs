using KYC360.Models;

namespace KYC360.Database
{
    public static class MockDatabase
    {
        private static readonly List<Entity> _entities;

        static MockDatabase()
        {
            _entities = GenerateMockData();
        }

        public static List<Entity> Entities => _entities;

        private static List<Entity> GenerateMockData()
        {
            Random random = new();
            List<Entity> entities = [];

            string[] firstNames = ["John", "Jane", "Alex", "Chris", "Pat", "Taylor", "Jordan", "Morgan", "Sam", "Casey"];
            string[] surnames = ["Smith", "Doe", "Brown", "Johnson", "Davis", "Miller", "Wilson", "Moore", "Taylor", "Anderson"];
            string[] cities = ["New York", "Los Angeles", "Chicago", "Houston", "Phoenix", "Philadelphia", "San Antonio", "San Diego", "Dallas", "San Jose"];
            string[] countries = ["USA", "Canada", "UK", "Germany", "France", "Australia", "India", "China", "Japan", "Brazil"];

            // Generate 50 entities
            for (int i = 1; i <= 50; i++)
            {
                entities.Add(new Entity
                {
                    Id = i.ToString(),
                    Gender = random.Next(0, 2) == 0 ? "Male" : "Female",
                    Deceased = random.Next(0, 5) == 0,
                    Addresses =
                    [
                        new Address
                        {
                            AddressLine = $"{random.Next(100, 999)} Main St",
                            City = cities[random.Next(cities.Length)],
                            Country = countries[random.Next(countries.Length)]
                        }
                    ],
                    Dates =
                    [
                        new Date
                        {
                            DateType = "Birth",
                            Value = DateTime.Now.AddYears(-random.Next(20, 80))
                        }
                    ],
                    Names =
                    [
                        new Name
                        {
                            FirstName = firstNames[random.Next(firstNames.Length)],
                            MiddleName = firstNames[random.Next(firstNames.Length)],
                            Surname = surnames[random.Next(surnames.Length)]
                        }
                    ]
                });
            }

            return entities;
        }
    }
}