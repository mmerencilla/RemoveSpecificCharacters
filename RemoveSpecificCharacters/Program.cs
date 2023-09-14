using System;
using System.Net;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Dynamic;

namespace RemoveSpecificCharacters
{
    class Program
    {
        static void Main()
        {
            try
            {
                WebClient client = new WebClient();
                var oldJson = client.DownloadString("https://coderbyte.com/api/challenges/json/json-cleaning");
                string[] toBeRemove = { "N/A", "-", "" };

                Console.WriteLine("OLD JSON:");
                Console.WriteLine(JsonConvert.DeserializeObject<dynamic>(oldJson));
                Console.WriteLine("=========================================================================");
                Console.WriteLine("");

                var newJson = RemovePropertyFromJson(oldJson, toBeRemove);
                Console.WriteLine("NEW JSON:");
                Console.WriteLine(newJson);
                Console.WriteLine("=========================================================================");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }

        private static dynamic RemovePropertyFromJson(string jsonContent, string[] toBeRemove)
        {
            var result = JsonConvert.SerializeObject(ProcessJsonPropertiesRemoval(jsonContent, toBeRemove));
            return JsonConvert.DeserializeObject<dynamic>(result);
        }

        private static IDictionary<string, object> ProcessJsonPropertiesRemoval(string jsonContent, string[] toBeRemove)
        {
            dynamic obj = JsonConvert.DeserializeObject<ExpandoObject>(jsonContent);
            var toRemove = new List<string>();
            var toUpdate = new List<dynamic>();

            IDictionary<string, object> personDictionary = (IDictionary<string, object>)obj;

            foreach (KeyValuePair<string, object> item in personDictionary)
            {
                if (item.Value.GetType().Equals(typeof(string)) && Array.Exists(toBeRemove, e => e == (string)item.Value))
                {
                    toRemove.Add(item.Key);
                }
                else if (item.Value.GetType().Equals(typeof(Int64)) || item.Value.GetType().Equals(typeof(int)))
                {
                    //Do nothing
                }
                else if (item.Value.GetType().Equals(typeof(ExpandoObject)))
                {
                    toUpdate.Add(new
                    {
                        KeyName = item.Key,
                        KeyValue = RemovePropertyFromJson(JsonConvert.SerializeObject(item.Value), toBeRemove)
                    });
                }
                else if (item.Value.GetType().Equals(typeof(List<object>)))
                {
                    var list = (List<object>)item.Value;
                    list.Remove("-");

                    toUpdate.Add(new
                    {
                        KeyName = item.Key,
                        KeyValue = list
                    });
                }
            }

            // Remove property
            foreach (var item in toRemove)
                personDictionary.Remove(item);

            // Update values
            foreach (var item in toUpdate)
                personDictionary[item.KeyName] = item.KeyValue;

            return personDictionary;
        }
    }
}