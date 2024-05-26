using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorApp.Data
{
    public class Driver
    {
        public int ID { get; set; }
        public string Name { get; set; }
    
        public Driver(int ID,string Name)
        {
            this.ID = ID;
            this.Name = Name;

            DB.DataBase.saveDriver(this);
        }

        public Driver()
        {
            
        }

        public void deleteDriver()
        {
            DB.DataBase.deleteDriver(this);
        }

        public static void deleteDriver(Driver ToBeDeleted)
        {
            DB.DataBase.deleteDriver(ToBeDeleted);
        }  
    }
}
