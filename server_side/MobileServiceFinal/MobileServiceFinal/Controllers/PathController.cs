﻿using System.Linq;
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
    public class PathController : TableController<Path>
    {
        [AuthorizeLevel(AuthorizationLevel.User)] 
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            MobileServiceContext context = new MobileServiceContext();
            DomainManager = new EntityDomainManager<Path>(context, Request, Services);
        }

        // GET tables/Path
        public IQueryable<Path> GetAllPath()
        {
            return Query(); 
        }

        // GET tables/Path/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Path> GetPath(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Path/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Path> PatchPath(string id, Delta<Path> patch)
        {
             return UpdateAsync(id, patch);
        }

        // POST tables/Path/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task<IHttpActionResult> PostPath(Path item)
        {
            Path current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Path/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeletePath(string id)
        {
             return DeleteAsync(id);
        }

    }
}