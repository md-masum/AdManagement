using AdCore.Entity;

namespace AdCore.Interface.Service
{
    public interface ITestService
    {
        IList<Test> GetAll();
        Test GetById(string id);
        Test Create(Test testToCreate);
        Test Update(Test testToUpdate);
        bool Delete(string id);
    }
}
