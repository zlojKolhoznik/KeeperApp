using KeeperApp.Records;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;

namespace KeeperApp.Services
{
    public class RecordsSerializationService
    {
        private readonly JsonSerializerOptions jsonOptions;

        public RecordsSerializationService()
        {
            jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
        }

        public async Task SerializeRecordsAsync(IEnumerable<Record> records, StorageFile file)
        {
            if (!file.Name.EndsWith(".json"))
            {
                throw new ArgumentException("File must have a .json extension", nameof(file));
            }
            string json = JsonSerializer.Serialize(records, jsonOptions);
            await FileIO.WriteTextAsync(file, json);
        }

        public async Task<IEnumerable<Record>> DeserializeRecordsAsync(StorageFile file)
        {
            string json = await FileIO.ReadTextAsync(file);
            var records = JsonSerializer.Deserialize<IEnumerable<Record>>(json, jsonOptions);
            return records is null ? throw new JsonException("Failed to deserialize records from JSON") : records;
        }
    }
}
