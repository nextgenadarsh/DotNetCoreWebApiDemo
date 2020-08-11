using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreWebApiDemo.Models;
using MongoDB.Driver;

namespace DotNetCoreWebApiDemo.Services
{
    public class MongoDbStudentService : IStudentService
    {
        private readonly IMongoCollection<Student> studentCollection;

        public MongoDbStudentService(IStudentDbSettings studentDbSettings)
        {
            var client = new MongoClient(studentDbSettings.ConnectionString);
            var database = client.GetDatabase(studentDbSettings.Database);
            studentCollection = database.GetCollection<Student>(studentDbSettings.Collection);
        }

        public async Task<List<Student>> GetAsync()
        {
            var students = await studentCollection.FindAsync(student => true);
            return await students.ToListAsync();
        }

        public async Task<Student> GetAsync(string studentId)
        {
            var students = await studentCollection.FindAsync<Student>(student => student.Id == studentId);
            return await students.FirstOrDefaultAsync();
        }

        public async Task<Student> CreateAsync(Student student) {
            await studentCollection.InsertOneAsync(student);
            return student;
        }

        public async Task<bool> UpdateAsync(Student updatedStudent)
        {
            var res = await studentCollection.ReplaceOneAsync(student => student.Id == updatedStudent.Id, updatedStudent);
            return res.MatchedCount > 0;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var res = await studentCollection.DeleteOneAsync(student => student.Id == id);
            return res.DeletedCount > 0;
        }

    }
}
