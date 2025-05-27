using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using ToDoApi.Models;

namespace ToDoApi.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<ToDoItem> _collection;

        public MongoDbService()
        {
            // Connect to MongoDB and get the "todos" collection from the "ToDoDb" database
            var client = new MongoClient("mongodb+srv://solingorann:Nayasolin0225@cluster0.5z3ci.mongodb.net/ToDoDb?retryWrites=true&w=majority&appName=Cluster0");
            var database = client.GetDatabase("ToDoDb");
            _collection = database.GetCollection<ToDoItem>("todos");
        }

        // Insert a new ToDo item into the collection
        public async Task CreateAsync(ToDoItem item) =>
            await _collection.InsertOneAsync(item);

        // Get all ToDo items
        public async Task<List<ToDoItem>> GetAllAsync() =>
            await _collection.Find(_ => true).ToListAsync();

        // Get all ToDo items (duplicate of GetAllAsync, consider removing one)
        public async Task<List<ToDoItem>> GetAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        // Replace an existing item by id with a new one
        public async Task<bool> UpdateAsync(string id, ToDoItem updatedItem)
        {
            var result = await _collection.ReplaceOneAsync(x => x.Id == id, updatedItem);
            return result.ModifiedCount > 0;
        }

        // Delete a ToDo item by ID
        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _collection.DeleteOneAsync(x => x.Id == id);
            return result.DeletedCount > 0;
        }
    }
}
