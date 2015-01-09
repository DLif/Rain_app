using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using pleaseWork.DataObjects;
using pleaseWork.Models;

namespace pleaseWork.Controllers
{
    public class MyTableController : TableController<MyTable>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<MyTable>(context, Request, Services);
        }

        // GET tables/MyTable
        public IQueryable<MyTable> GetAllMyTable()
        {
            return Query(); 
        }

        // GET tables/MyTable/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<MyTable> GetMyTable(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/MyTable/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<MyTable> PatchMyTable(string id, Delta<MyTable> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/MyTable
        public async Task<IHttpActionResult> PostMyTable(MyTable item)
        {
            MyTable current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/MyTable/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteMyTable(string id)
        {
             return DeleteAsync(id);
        }

    }
}