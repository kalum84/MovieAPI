using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MovieWebAPI
{
    public class CsvReader<T> where T : CsvableBase, new()
    {
        static Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
        public IEnumerable<T> Read(string filePath, bool hasHeaders)
        {
            var objects = new List<T>();
            using (var sr = new StreamReader(filePath))
            {
                bool headersRead = false;
                string line;
                do
                {
                    line = sr.ReadLine();

                    if (line != null && headersRead)
                    {
                        String[] Fields = CSVParser.Split(line);


                        // clean up the fields (remove " and leading spaces)
                        for (int i = 0; i < Fields.Length; i++)
                        {
                            Fields[i] = Fields[i].TrimStart(' ', '"');
                            Fields[i] = Fields[i].TrimEnd('"');
                        }

                        var obj = new T();
                        obj.AssignValuesFromCsv(Fields);
                        objects.Add(obj);
                    }
                    if (!headersRead)
                    {
                        headersRead = true;
                    }
                } while (line != null);
            }

            return objects;
        }
    }
}
