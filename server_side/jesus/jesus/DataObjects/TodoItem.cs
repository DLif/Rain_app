using Microsoft.WindowsAzure.Mobile.Service;

namespace jesus.DataObjects
{
    
    public class TodoItem : EntityData
    {
        public string Text { get; set; }

        public bool Complete { get; set; }
    }
      
    /*
    public class TodoItem : EntityData
    {
        public TodoItem()
        {
            Items = new List<Item>();
        }
        public string Text { get; set; }
        public bool Complete { get; set; }
        public virtual ICollection<Item> Items { get; set; }
    }
     * */
}