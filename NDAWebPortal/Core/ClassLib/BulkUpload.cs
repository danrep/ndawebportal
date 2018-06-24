using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NDAWebPortal.Core.ClassLib
{
    public static class BulkUpload
    {
        public static List<T> GetFileDataToList<T>(string filePath, CsvHelper.Configuration.CsvConfiguration configuration)
        {
            List<T> records = new List<T>();

            try
            {
                using (var fileReader = new StreamReader(filePath))
                using (var csvReader = new CsvHelper.CsvReader(fileReader, configuration))
                {
                    records = csvReader.GetRecords<T>().ToList();
                    ActivityLogger.Log("INFO", $"{new FileInfo(filePath).Name} has {records.Count}");
                }
            }
            catch (Exception ex)
            {
                ActivityLogger.Log(ex);
            }

            return records;
        }
    }
}
