using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace IvinkaSplitEvents
{
    class Program
    {
        private const string SplitString = " Concert Tickets ";

        static void Main(string[] args)
        {
            var artistName = "A Day To Remember";
            var headCity = "Paris";
            var inputString = "Dec. 10, 2016 Sat 16:30 A Day To Remember Red Hill Concert Tickets Dec. 13, 2016 Tue 19:00 A Day To Remember Hindmarsh Concert Tickets Dec. 14, 2016 Wed 19:00 A Day To Remember Melbourne Concert Tickets Dec. 16, 2016 Fri 18:30 A Day To Remember Sydney Concert Tickets Dec. 18, 2016 Sun 18:00 A Day To Remember Brisbane Concert Tickets Jan. 22, 2017 Sun 18:00 A Day To Remember Cardiff Concert Tickets Jan. 23, 2017 Mon 18:30 A Day to Remember in Glasgow Glasgow Concert Tickets Jan. 25, 2017 Wed 16:00 A Day To Remember Birmingham Concert Tickets Jan. 27, 2017 Fri 19:00 A Day To Remember London Concert Tickets Jan. 28, 2017 Sat 19:00 A Day To Remember Leeds Concert Tickets Jan. 30, 2017 Mon 19:00 A Day To Remember Oberhausen Concert Tickets Jan. 31, 2017 Tue 19:00 A Day To Remember Leipzig Concert Tickets Feb. 3, 2017 Fri 18:30 A Day To Remember Hamburg Concert Tickets Feb. 4, 2017 Sat 19:00 A Day To Remember Berlin Concert Tickets Feb. 5, 2017 Sun 20:00 A Day To Remember Vienna Concert Tickets Feb. 7, 2017 Tue 19:00 A Day To Remember Milan Concert Tickets Feb. 11, 2017 Sat 19:00 A Day To Remember Stuttgart Concert Tickets Feb. 12, 2017 Sun 18:30";

            //var createdObjects = CreatedObjects(SplitString, artistName, headCity, inputString);

            var engine = new FileHelperEngine<Event>();

            //engine.WriteFile("Output.csv", createdObjects);

            var records = engine.ReadFile("Input.csv");

            var finalResults = new List<Event>();

            foreach (var record in records)
            {
                var createdObjects = CreatedObjects(SplitString, record.Artist, record.City, record.Date, record.Photo);

                finalResults.AddRange(createdObjects);
            }

            var writeEngine = new FileHelperEngine<Event>();

            writeEngine.WriteFile("Output.csv", finalResults);
        }

        private static List<Event> CreatedObjects(string splitString, string artistName, string headCity, string inputString, string photoUrl)
        {
            if (artistName.ToLower().Contains(headCity.ToLower()))
            {
                artistName = artistName.Split(new string[] { " in " }, new StringSplitOptions())[0];
            }
            
            if (Regex.Matches(artistName, @"\((.*)Event(.*)\)").Count > 0)
            {
                artistName = artistName.Split(new string[] { " (" }, new StringSplitOptions())[0];
            }

            artistName = artistName.Trim();
            headCity = headCity.Trim();

            var inputArray = inputString.Split(new string[] { splitString }, new StringSplitOptions());

            var artistSplitString = " " + artistName + " ";
            var regex = new Regex(Regex.Escape("in "));
            

            var createdObjects = new List<Event>();

            foreach (var item in inputArray)
            {
                var currentItemArray = item.Split(new string[] { artistSplitString }, new StringSplitOptions());

                if (currentItemArray.Length == 2)
                {
                    var currentEvent = new Event();
                    currentEvent.Artist = artistName;
                    currentEvent.Photo = photoUrl;
                    currentEvent.Date = currentItemArray[0];

                    var currentCity = currentItemArray[1];
                    if (currentCity.StartsWith("in "))
                    {
                        currentCity = regex.Replace(currentCity, "", 1);
                    }

                    var currentCityLength = currentCity.Length;
                    if (currentCityLength % 2 == 1)
                    {
                        for (int i = 0; i < currentCityLength / 2; i++)
                        {
                            var currentChar = currentCity[i];
                            var offsetedChar = currentCity[i + currentCityLength / 2 + 1];
                            if (currentChar != offsetedChar)
                            {
                                break;
                            }
                            else if (currentChar == offsetedChar && currentCityLength == i * 2 + 3)
                            {
                                currentCity = currentCity.Substring(0, currentCityLength / 2);
                            }
                        }
                    }

                    

                    currentEvent.City = currentCity;

                    createdObjects.Add(currentEvent);
                }
                else if (currentItemArray.Length == 1)
                {
                    var currentEvent = new Event();
                    currentEvent.Artist = artistName;
                    currentEvent.City = headCity;
                    currentEvent.Date = currentItemArray[0];

                    createdObjects.Add(currentEvent);
                }
            }

            return createdObjects;
        }
    }

    [DelimitedRecord(",")]
    public class Event
    {
        [FieldOrder(1)]
        public string Artist { get; set; }

        [FieldOrder(2)]
        public string Photo { get; set; }

        [FieldOrder(3)]
        public string Date { get; set; }

        [FieldOrder(0)]
        public string City { get; set; }
    }



    [DelimitedRecord(",")]
    public class EventFromFile
    {
        
        public string Artist { get; set; }

        
        public string Photo { get; set; }

       
        public string City { get; set; }

        
        //[FieldConverter(ConverterKind.Date, "mmm. d, yyyy ddd HH:mm")]
        public string DateAndTime;
    }


}
