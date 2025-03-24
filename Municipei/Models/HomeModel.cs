using Google.Cloud.Firestore;
using Municipei.Models;
using MunModel;
namespace Municipei.Models
{
    [FirestoreData] 
    public class HomeModel 
    {
        [FirestoreProperty("id")]
        public int Id { get; set; }

        [FirestoreProperty("name")]
        public string? Name { get; set; }

        [FirestoreProperty("password")]
        public string? Password { get; set; }

        [FirestoreProperty("cpf")]
        public string? Cpf { get; set; }

        [FirestoreProperty("munPR")]
        public string? MunPR { get; set; } // municipio

        [FirestoreProperty("occupation")]
        public string? Occupation { get; set; }

        [FirestoreProperty("phone")]
        public string? Phone { get; set; }

        [FirestoreProperty("email")]
        public string? Email { get; set; }

         public List<MunicipiosList> Municipios { get; set; } = new();
    }
}
