using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWebAPI
{
    public abstract class CsvableBase
    {
        public virtual string ToCsv()
        {
            string output = "";

            var properties = GetType().GetProperties();

            for (var i = 0; i < properties.Length; i++)
            {
                output += PreProcess(properties[i].GetValue(this).ToString());
                if (i != properties.Length - 1)
                {
                    output += ",";
                }
            }

            return output;
        }

        public virtual string ToCsv(string[] propertyNames, bool isIgnore)
        {
            string output = "";
            bool isFirstPropertyWritten = false;


            var properties = GetType().GetProperties();

            for (var i = 0; i < properties.Length; i++)
            {
                if (isIgnore)
                {
                    if (!propertyNames.Contains(properties[i].Name))
                    {
                        if (isFirstPropertyWritten)
                        {
                            output += ",";
                        }
                        output += PreProcess(properties[i].GetValue(this).ToString());

                        if (!isFirstPropertyWritten)
                        {
                            isFirstPropertyWritten = true;
                        }
                    }
                }
                else
                {
                    if (propertyNames.Contains(properties[i].Name))
                    {
                        if (isFirstPropertyWritten)
                        {
                            output += ",";
                        }
                        output += PreProcess(properties[i].GetValue(this).ToString());

                        if (!isFirstPropertyWritten)
                        {
                            isFirstPropertyWritten = true;
                        }
                    }
                }
            }

            return output;
        }

        public virtual string ToCsv(int[] propertyIndexes, bool isIgnore)
        {
            string output = "";

            bool isFirstPropertyWritten = false;

            var properties = GetType().GetProperties();

            for (var i = 0; i < properties.Length; i++)
            {
                if (isIgnore)
                {
                    if (!propertyIndexes.Contains(i))
                    {
                        if (isFirstPropertyWritten)
                        {
                            output += ",";
                        }

                        output += PreProcess(properties[i].GetValue(this).ToString());

                        if (!isFirstPropertyWritten)
                        {
                            isFirstPropertyWritten = true;
                        }
                    }
                }
                else
                {
                    if (propertyIndexes.Contains(i))
                    {
                        if (isFirstPropertyWritten)
                        {
                            output += ",";
                        }

                        output += PreProcess(properties[i].GetValue(this).ToString());

                        if (!isFirstPropertyWritten)
                        {
                            isFirstPropertyWritten = true;
                        }
                    }
                }

            }

            return output;
        }
        private string PreProcess(string input)
        {
            input = input.Replace('ı', 'i')
                .Replace('ç', 'c')
                .Replace('ö', 'o')
                .Replace('ş', 's')
                .Replace('ü', 'u')
                .Replace('ğ', 'g')
                .Replace('İ', 'I')
                .Replace('Ç', 'C')
                .Replace('Ö', 'O')
                .Replace('Ş', 'S')
                .Replace('Ü', 'U')
                .Replace('Ğ', 'G')
                .Replace("\"", @"""")
                .Trim();
            if (input.Contains(","))
            {
                input = @""" + input + """;
            }
            return input;
        }
        public virtual void AssignValuesFromCsv(string[] propertyValues)
        {
            var properties = GetType().GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType
                    .IsSubclassOf(typeof(CsvableBase)))
                {
                    var instance = Activator.CreateInstance(properties[i].PropertyType);
                    var instanceProperties = instance.GetType().GetProperties();
                    var propertyList = new List<string>();

                    for (var j = 0; j < instanceProperties.Length; j++)
                    {
                        propertyList.Add(propertyValues[i + j]);
                    }
                    var m = instance.GetType().GetMethod("AssignValuesFromCsv", new Type[] { typeof(string[]) });
                    m.Invoke(instance, new object[] { propertyList.ToArray() });
                    properties[i].SetValue(this, instance);

                    i += instanceProperties.Length;
                }
                else
                {
                    var type = properties[i].PropertyType.Name;
                    switch (type)
                    {
                        case "Int32":
                            properties[i].SetValue(this,
                                            int.Parse(propertyValues[i]));
                            break;
                        default:
                            properties[i].SetValue(this, propertyValues[i]);
                            break;
                    }
                }
            }
        }
    }
}
