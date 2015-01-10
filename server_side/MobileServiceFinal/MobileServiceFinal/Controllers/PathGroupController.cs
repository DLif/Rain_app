using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.WindowsAzure.Mobile.Service;
using MobileServiceFinal.DataObjects;
using MobileServiceFinal.Models;
using Microsoft.WindowsAzure.Mobile.Service.Security; /* for facebook authentication */

namespace MobileServiceFinal.Controllers
{
    public class PathGroupController : TableController<PathGroup>
    {
        [AuthorizeLevel(AuthorizationLevel.User)] 
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PathGroup>(context, Request, Services);
        }

        // GET tables/PathGroup
        public IQueryable<PathGroup> GetAllPathGroup()
        {
            return Query(); 
        }

        // GET tables/PathGroup/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<PathGroup> GetPathGroup(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/PathGroup/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<PathGroup> PatchPathGroup(string id, Delta<PathGroup> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/PathGroup/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPathGroup(PathGroup item)
        {
            PathGroup current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/PathGroup/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePathGroup(string id)
        {
             return DeleteAsync(id);
        }

    }
}