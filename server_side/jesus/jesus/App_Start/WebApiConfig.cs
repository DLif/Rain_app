using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Web.Http;
using jesus.DataObjects;
using jesus.Models;
using Microsoft.WindowsAzure.Mobile.Service;
using Microsoft.WindowsAzure.Mobile.Service.Security;


namespace jesus
{
    public static class WebApiConfig
    {
        public static void Register()
        {
            // Use this class to set configuration options for your mobile service
            ConfigOptions options = new ConfigOptions();
          //  options.LoginProviders.Remove(typeof(AzureActiveDirectoryLoginProvider));
            //options.LoginProviders.Add(typeof(AzureActiveDirectoryExtendedLoginProvider));

            // Use this class to set WebAPI configuration options
            HttpConfiguration config = ServiceConfig.Initialize(new ConfigBuilder(options));

            // To display errors in the browser during development, uncomment the following
            // line. Comment it out again when you deploy your service for production use.
            // config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

            Database.SetInitializer(new MobileServiceInitializer());
        }
    }

    public class MobileServiceInitializer : DropCreateDatabaseIfModelChanges<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {

            /* new by shachar 
            List<Item> items = new List<Item>
            {
                new Item { Id = "1", ItemName = "Milk" },
                new Item { Id = "2", ItemName = "Eggs" }
            };

                        List<TodoItem> todoItems = new List<TodoItem>
            {
                new TodoItem { Id = "1", Text = "Grocery", Complete = false, Items=items }
            };
            */

            List<TodoItem> todoItems2 = new List<TodoItem>
            {
                new TodoItem { Id = "1", Text = "First item", Complete = false },
                new TodoItem { Id = "2", Text = "Second item", Complete = false },
            };

            foreach (TodoItem todoItem in todoItems2)
            {
                context.Set<TodoItem>().Add(todoItem);
            }

            base.Seed(context);
        }
    }
}

