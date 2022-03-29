using AdCore.Entity;

namespace AdCore.Interface.Repository
{
    public interface ITestRepository
    {
        IList<Test> GetAll();
        Test GetById(string id);
        Test Create(Test testToCreate);
        Test Update(Test testToUpdate);
        bool Delete(string id);
    }
}
