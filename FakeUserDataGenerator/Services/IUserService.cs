using System.Security.Cryptography;
using System.Text;
using FakeUserDataGenerator.Models;

namespace FakeUserDataGenerator.Services;

public interface IUserService
{ 
    List<UserData> GenerateUserData(string seed, 
        string country, 
        double errors, 
        int firstItem = 1);

    //public Task<byte[]> ExportToExcel(IFormCollection formCollection);
}

public class UserService : IUserService
{
    public List<UserData> GenerateUserData(string seed, string country, double errors, int firstItem = 1)
    {
        var userData = new List<UserData>();
        seed += firstItem / 50;
        var random = new Random(seed.GetHashCode());

        var dataset = Path.Combine(Environment.CurrentDirectory, "Datasets", country);

        var isMale = random.Next() > int.MaxValue / 2;
        var names = isMale
            ? File.ReadAllLines(Path.Combine(dataset, "MaleNames.txt"))
            : File.ReadAllLines(Path.Combine(dataset, "FemaleNames.txt"));
        string[] surnames;
        if (country.Equals("Russia"))
            surnames = isMale
                ? File.ReadAllLines(Path.Combine(dataset,"MaleSurnames.txt"))
                : File.ReadAllLines(Path.Combine(dataset,"FemaleSurnames.txt"));
        else
            surnames =File.ReadAllLines(Path.Combine(dataset,"Surnames.txt"));
        var cities = File.ReadAllLines(Path.Combine(dataset,"Cities.txt"));
        var streets = File.ReadAllLines(Path.Combine(dataset,"Streets.txt"));

        var middleNames = Array.Empty<string>();
        if (country.Equals("Russia"))
            middleNames = isMale
                ? File.ReadAllLines(Path.Combine(dataset,"MaleMiddleNames.txt"))
                : File.ReadAllLines(Path.Combine(dataset,"FemaleMiddleNames.txt"));
        var operatorCodes = File.ReadAllLines(Path.Combine(dataset,"OperatorCodes.txt"));

        if (firstItem == 0)
            firstItem = 1;
        
        for (var i = firstItem; i < firstItem + 50; i++)
        {
            var name = names[random.Next(0, names.Length)];
            var surname = surnames[random.Next(0, surnames.Length)];
            var middleName = "";
            if (country.Equals("Russia"))
                middleName =  middleNames[random.Next(0, middleNames.Length)];
            var city = cities[random.Next(0, cities.Length)];
            var street = streets[random.Next(0, streets.Length)];
            var address = char.ToUpper(city[0]) + city[1..] + ", " + char.ToUpper(street[0]) + street[1..] + ", " +
                          (random.Next() > int.MaxValue / 2
                              ? $"{random.Next(999)}, {random.Next(999)}"
                              : $"{random.Next(999)}");
            name = char.ToUpper(surname[0]) + surname[1..] + " " + char.ToUpper(name[0]) + name[1..] +
                       (country.Equals("Russia") ? " " + char.ToUpper(middleName[0]) + middleName[1..] : "");
            var phoneNumber = (country.Equals("Russia") ? "+7 " :
                                  country.Equals("Poland") ? "+48 " :
                                  "+1 ") +
                              $"({operatorCodes[random.Next(0, operatorCodes.Length)]}) {random.Next(100, 999)}-{random.Next(10, 99)}-{random.Next(10, 99)}";
            
            var id = i + seed + name + address + phoneNumber;
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(id));
                id = new Guid(hash).ToString();
            }
            
            userData.Add(new UserData
            {
                Index = i.ToString(),
                Id = id,
                Name = name,
                Address = address,
                PhoneNumber = phoneNumber
            });
        }

        return InsertErrors(random, userData, country, errors);
    }

    private static List<UserData> InsertErrors(Random random, List<UserData> userData, string country, double errors)
    {
        foreach (var user in userData)
        {
            var initErrors = errors;
            var chars = "1234567890";
            switch (country)
            {
                case "Russia":
                    chars = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюя";
                    break;
                case "USA":
                    chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case "Poland":
                    const string lowercaseChars = "aąbcćdeęfghijklłmnńoópqrsśtuvwxyzźż";
                    chars += lowercaseChars + lowercaseChars.ToUpper();
                    break;
            }

            while (errors > 0.99)
            {
                switch (random.Next(0, 3))
                {
                    case 0:
                        user.Name = RandomizeOperation(random, user.Name, chars, 30, 5);
                        break;
                    case 1:
                        user.Address = RandomizeOperation(random, user.Address, chars, 40, 30);
                        break;
                    case 2:
                        user.PhoneNumber = RandomizeOperation(random, user.PhoneNumber, chars, 20, 18);
                        break;
                }

                errors -= 1;
            }

            if (errors != 0 && random.NextDouble() >= errors)
                switch (random.Next(0, 3))
                {
                    case 0:
                        user.Name = RandomizeOperation(random, user.Name, chars, 30, 5);
                        break;
                    case 1:
                        user.Address = RandomizeOperation(random, user.Address, chars, 40, 30);
                        break;
                    case 2:
                        user.PhoneNumber = RandomizeOperation(random, user.PhoneNumber, chars, 20, 18);
                        break;
                }

            errors = initErrors;
        }
    
        return userData;

    }

    private static string RandomizeOperation(Random random, string input, string chars, int upperLimit, int lowerLimit)
    {
        var randomizedValue = 
            input.Length >= upperLimit ? 
            random.Next(2,4) : 
            input.Length <= lowerLimit ? 
                random.Next(1,3) : 
                random.Next(1,4);
        
        return randomizedValue switch
        {
            1 => InsertChar(random, input, chars),
            2 => SwitchChars(random, input),
            3 => DeleteChar(random, input),
            _ => input
        };
    }

    private static string InsertChar(Random rand, string input, string chars)
    {
        return input.Insert(rand.Next(0, input.Length), chars[rand.Next(0, chars.Length)].ToString());
    }
    
    private static string SwitchChars(Random rand, string input)
    {
        var randPos = rand.Next(0,input.Length - 1);
        var strChar = input.ToCharArray();
        strChar[randPos] = input[randPos + 1];
        strChar[randPos + 1] = input[randPos];
        return new string(strChar);
    }
    
    private static string DeleteChar(Random rand, string input)
    {
        return input.Remove(rand.Next(input.Length),1);
    }
}

