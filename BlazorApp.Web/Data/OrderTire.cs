using System;
namespace BlazorApp.Data
{
    public class OrderTire
    {
        public Tire OrderedTire{get;set;}
        public string NameOfUser;
        public DateTime TimeOfOrder;

        public OrderTire(Tire OrderedTire)
        {
            this.OrderedTire = OrderedTire;
            this.NameOfUser = User.CurrentUser.UserName;
            this.TimeOfOrder = DateTime.Now;

            //Insert into DB
        }
    }
}