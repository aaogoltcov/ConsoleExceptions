using System.Globalization;
using System.Text;

namespace ConsoleExceptions;

public class Person(string name, string surname, string middleName, DateTime birthday, long phone, PersonGender gender)
{
    private readonly string _name = name;
    private readonly string _surname = surname;
    private readonly string _middleName = middleName;
    private readonly DateTime _birthday = birthday;
    private readonly long _phone = phone;
    private readonly PersonGender _gender = gender;

    public override string ToString()
    {
        return
            $"Name: {_name} Surname: {_surname} MiddleName: {_middleName} Birthday: {_birthday.ToShortDateString()} Phone: {_phone} Gender: {_gender}";
    }
    
    

    public static void FillPerson()
    {
        while (true)
        {
            Console.WriteLine(
                "Введите данные в формате Фамилия Имя Отчество Дата_рождения(строка формата dd.mm.yyyy) Номер_телефона(целое беззнаковое число без форматирования) Пол(символ латиницей f или m):");

            var input = Console.ReadLine();

            if (input == "exit")
            {
                break;
            }

            try
            {
                var personData = GetPersonData(input);
                var person = GetPerson(personData);
                
                SerializePersonToFile(person);
            
                Console.WriteLine($"Person: {person}");
            }
            catch (ArgumentException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            catch (FormatException e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
        }
    }

    private static string[] GetPersonData(string? input)
    {
        var splitInput = input?.Split(' ');

        return CheckPersonData(splitInput);
    }

    private static string[] CheckPersonData(string[]? splitInput)
    {
        if (splitInput is not { Length: 6 })
        {
            throw new ArgumentException("Неверное количество данных");
        }

        return splitInput;
    }

    private static DateTime GetDateTime(string dateTime)
    {
        try
        {
            return DateTime.ParseExact(dateTime, "dd.MM.yyyy", CultureInfo.InvariantCulture);
        }
        catch (FormatException e)
        {
            throw new FormatException("Неверный формат даты рождения", e);
        }
    }
    
    private static long GetPhone(string phone)
    {
        return long.TryParse(phone, out var phoneLong)
            ? phoneLong
            : throw new ArgumentException("Неверный формат номера телефона");
    }
    
    private static PersonGender GetGender(string gender)
    {
        return gender switch
        {
            "m" => PersonGender.Male,
            "f" => PersonGender.Female,
            _ => throw new ArgumentException("Неверный формат пола")
        };
    }
    
    private static void SerializePersonToFile(Person person)
    {
        try
        {
            var filename = person._surname + ".txt";

            var streamWriter = File.Open(filename, FileMode.Append);
            var bytes = Encoding.UTF8.GetBytes(person.ToString());
        
            streamWriter.Write(bytes);
            streamWriter.Close();
        
            Console.WriteLine("Файл записан.");
        }
        catch (IOException e)
        {
            throw new IOException("Ошибка записи файла", e);
        }
    }

    private static Person GetPerson(string[] personData)
    {
        var checkedPersonData = CheckPersonData(personData);
    
        var surname = checkedPersonData[0];
        var name = checkedPersonData[1];
        var middleName = checkedPersonData[2];

        if (surname == null || name == null || middleName == null)
        {
            throw new ArgumentException("Неверный формат ФИО");
        }
        
        var dateTime = GetDateTime(checkedPersonData[3]);
        var phone = GetPhone(checkedPersonData[4]);
        var gender = GetGender(checkedPersonData[5]);
        
        return new Person(name, surname, middleName, dateTime, phone, gender);
    }
}