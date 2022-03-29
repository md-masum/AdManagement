using AdCore.Entity;
using AdCore.Interface.Repository;

namespace AdRepository
{
    public class TestRepository : ITestRepository
    {
        private readonly IList<Test> _tests;
        public TestRepository()
        {
            _tests = new List<Test>();
        }
        public IList<Test> GetAll()
        {
            return _tests;
        }
        
        public Test GetById(string id)
        {
            var data = _tests.FirstOrDefault(c => c.Id == id);
            if (data == null) throw new Exception("Data not found by provided id");
            return data;
        }
        
        public Test Create(Test testToCreate)
        {
            var test = new Test
            {
                Name = testToCreate.Name
            };

            _tests.Add(test);

            return test;
        }
        
        public Test Update(Test testToUpdate)
        {
            var test = _tests.FirstOrDefault(c => c.Id == testToUpdate.Id);
            if (test == null) throw new Exception("Test not found");

            test.Name = testToUpdate.Name;
            return test;
        }
        
        public bool Delete(string id)
        {
            var test = _tests.FirstOrDefault(c => c.Id == id);
            if (test is null) throw new Exception("test not found");
            _tests.Remove(test);

            return true;
        }
    }
}
