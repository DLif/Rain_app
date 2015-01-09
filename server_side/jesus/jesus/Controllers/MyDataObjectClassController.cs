using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using jesus.DataObjects;
using jesus.Models;

namespace jesus.Controllers
{
    public class MyDataObjectClassController : TableController<MyDataObjectClass>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<MyDataObjectClass>(context, Request, Services);
        }

        // GET tables/MyDataObjectClass
        public IQueryable<MyDataObjectClass> GetAllMyDataObjectClass()
        {
            return Query(); 
        }

        // GET tables/MyDataObjectClass/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MyDataObjectClass> GetMyDataObjectClass(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MyDataObjectClass/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MyDataObjectClass> PatchMyDataObjectClass(string id, Delta<MyDataObjectClass> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/MyDataObjectClass/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostMyDataObjectClass(MyDataObjectClass item)
        {
            MyDataObjectClass current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/MyDataObjectClass/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMyDataObjectClass(string id)
        {
             return DeleteAsync(id);
        }

    }
}