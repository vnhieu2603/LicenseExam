using Microsoft.OData.ModelBuilder;
using Microsoft.OData.Edm;
using WebAPI.Models;

namespace WebAPI.Extensions
{
    public class IEdmModel
    {
        public static Microsoft.OData.Edm.IEdmModel GetEdmModel()
        {
            ODataConventionModelBuilder modelBuilder = new ODataConventionModelBuilder();
            modelBuilder.EntitySet<Exam>("Exam");
            modelBuilder.EntitySet<Question>("Question");


            return modelBuilder.GetEdmModel();
        }
    }
}
