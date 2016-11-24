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

            var inputArray = inputString.Split(new string[] { splitString }, StringSplitOptions.RemoveEmptyEntries);

            var artistSplitString = " " + artistName + " ";
            var regex = new Regex(Regex.Escape("in "));
            
            var createdObjects = new List<Event>();

            foreach (var item in inputArray)
            {
                var currentItemArray = item.Split(new string[] { artistSplitString }, StringSplitOptions.RemoveEmptyEntries);

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
