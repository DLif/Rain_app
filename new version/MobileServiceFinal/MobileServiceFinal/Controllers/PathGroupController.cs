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
        
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<PathGroup>(context, Request, Services);
        }

        // GET tables/PathGroup
        public IQueryable<PathGroup> GetAllPathGroup()
        {
                    // Get the logged-in user.
       // var currentUser = User as ServiceUser;

        return Query().Where(todo => true == true);
        }

        public IQueryable<PathGroup> GetUserPathGroups(string UserId)
        {
            return Query().Where(item => item.UserId == UserId);
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
            // Get the logged-in user.
            //var currentUser = User as ServiceUser;

            // Set the user ID on the item.
           // item.UserId = currentUser.Id;

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