using AdCore.Entity;
using AdCore.Interface.Repository;
using AdCore.Interface.Service;

namespace AdService
{
    public class TestService : ITestService
    {
        private readonly ITestRepository _testRepository;

        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }
        public IList<Test> GetAll()
        {
            return _testRepository.GetAll();
        }

        public Test GetById(string id)
        {
            return _testRepository.GetById(id);
        }

        public Test Create(Test testToCreate)
        {
            return _testRepository.Create(testToCreate);
        }

        public Test Update(Test testToUpdate)
        {
            return _testRepository.Update(testToUpdate);
        }

        public bool Delete(string id)
        {
            return _testRepository.Delete(id);
        }
    }
}
