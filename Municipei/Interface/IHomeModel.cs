using Google.Cloud.Firestore;
using Municipei.Models;

namespace Municipei.Interface
{
    public interface IHomeModel
    {
         Task<HomeModel> RegisterClient(HomeModel model, CollectionReference user);

         Task<HomeModel> Login(string email, string pass, CollectionReference user);

         Task<string> SendCode(string email, CollectionReference user);

    }
}
