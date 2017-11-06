using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart.ViewModel
{
    public class Viewmodel
    {

        public int UserId { get; set; }
        public int Roleid { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserEmail { get; set; }
        public string UserAddress { get; set; }
        public string RoleName { get; set; }
        public bool UserIsDeleted { get; set; }
        public System.DateTime UserCreatedDate { get; set; }
    }
}