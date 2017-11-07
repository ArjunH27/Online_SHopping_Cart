using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class AdminController : Controller
    {

        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        public ActionResult Homepage()
        {
            DateTime today = System.DateTime.Now.Date;

            var UserCount = (from u in db.User_Table where u.UserIsDeleted == false select u).Count();
            ViewBag.UserCount = UserCount;
            var NewUserCount = (from u in db.User_Table where u.UserCreatedDate == today select u).Count();
            ViewBag.NewUserCount = NewUserCount;
            var unauthorised = (from u in db.User_Table where u.UserIsDeleted == true && u.UserCreatedDate == today select u).Count();
            ViewBag.unauthorised = unauthorised;
            var NewRegisteredUserCount = (from u in db.User_Table where u.UserIsDeleted == false && u.UserCreatedDate == today select u).Count();
            ViewBag.NewRegisteredUserCount = NewRegisteredUserCount;
            var SellerCount = (from u in db.User_Table where u.UserIsDeleted == false && u.Roleid == 2 select u).Count();
            ViewBag.SellerCount = SellerCount;
            var AdminCount = (from u in db.User_Table where u.UserIsDeleted == false && u.Roleid == 1 select u).Count();
            ViewBag.AdminCount = AdminCount;
            var BuyerCount = (from u in db.User_Table where u.UserIsDeleted == false && u.Roleid == 4 select u).Count();
            ViewBag.BuyerCount = BuyerCount;
            var Servicecount = (from u in db.User_Table where u.UserIsDeleted == false && u.Roleid == 3 select u).Count();
            ViewBag.Servicecount = Servicecount;
            var BaseCategoriesCount = (from b in db.BaseCategory_Table where b.BaseCatIsDeleted == false select b).Count();
            ViewBag.BaseCategoriesCount = BaseCategoriesCount;
            var ProductCategoriesCount = (from p in db.ProductCategory_Table where p.ProductCatIsDeleted == false select p).Count();
            ViewBag.ProductCategoriesCount = ProductCategoriesCount;
            var LocationsCount = (from l in db.Location_Table where l.LocationIsDeleted == false select l).Count();
            ViewBag.LocationsCount = LocationsCount;
            var sales = (from o in db.Order_Table where o.OrderStatus == 1 && o.OrderCreatedDate == today select o).Count();
            ViewBag.sales = sales;
            var orders = (from o in db.Order_Table where o.OrderStatus == 0 && o.OrderCreatedDate == today select o).Count();
            ViewBag.orders = orders;
            return View();

        }
        /// <summary>
        /// To Manage different roles in Online Shopping Site
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRole()
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            var roles = (from r in db.Role_Table where r.RoleIsDeleted == false select r).ToList();/*Getting roles created from Role_Table if 
                                                                                                role that is checking whether value of RoleIsDeleted=0*/
            ViewBag.Roles = roles;      //passing list to Viewbag
            //ViewBag.message = null;
            return View();
        }

        /// <summary>
        /// To create new roles in Online Shopping site.New role is added to the Role_Table
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageRole(Role_Table role)                                           //new object of Role_Table are passed to post function ManageRole
        {

            role.RoleCreatedBy = Session["user"].ToString();
            role.RoleUpdatedBy = Session["user"].ToString();
            role.RoleCreatedDate = DateTime.Now;
            role.RoleUpdateDate = DateTime.Now;
            role.RoleIsDeleted = false;
            db.Role_Table.Add(role);                                                               //Adding values to Role_Table
            db.SaveChanges();         
            return RedirectToAction("ManageRole");

        }
        /// <summary>
        /// This is to edit role details in a grid.Inline editing is done by ajax method.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="RoleName"></param>
        /// <param name="RoleDescription"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RoleEdit(int RoleId, string RoleName, string RoleDescription)      //updated values are passed as parameters of post function through ajax method
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            Role_Table role = db.Role_Table.Find(RoleId);                                     //Details of patricular role is obtained using it's id
            role.RoleName = RoleName;                                                         /*Rest of values are updated*/
            role.RoleDesc = RoleDescription;
            role.RoleUpdatedBy = Session["user"].ToString();
            role.RoleIsDeleted = false;
            role.RoleUpdateDate = DateTime.Now;
            db.SaveChanges();                                                                 //Db is updated
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRole", "Admin");   //passing url to which control to be navigated is stored in a variable
            return Json(new { RoleId = RoleId, RoleName = RoleName, RoleDesc = RoleDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);  //Values updated and url is returned back to ajax success function

        }
        /// <summary>
        ///  This is to delete  details of a particular role from a grid.When data deleted from grid data that colyumn isdeleted of that record is set to 1 .
        ///  By this that record become in active,So that will not appear in grid .Inline deleting is done by ajax method
        /// </summary>
        /// <param name="RoleId"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult RoleDelete(int RoleId)
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            Role_Table role = db.Role_Table.Find(RoleId);                                         //Details of patricular role to be deleted is obtained using it's id
            role.RoleIsDeleted = true;



            db.SaveChanges();                                                                     //Db is updated
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRole", "Admin");//passing url to which control to be navigated is stored in a variable
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// IsRoleNameExist is used to avoid duplicate rolename in Role_Table .
        /// This function called on validation of role name.This to check whether the entered rolename already exist.
        /// If it exist then error message will be diaplayed and it will not be added.
        /// 
        /// </summary>
        /// <param name="RoleName"></param>
        /// <returns></returns>
        public JsonResult IsRoleNameExist(string RoleName)
        {
            var validateName = db.Role_Table.FirstOrDefault(x => x.RoleName == RoleName && x.RoleIsDeleted == false);         //Details of roles that has same name of Entered rolename and which are active is taken
            if (validateName != null)                                                                                       //If no such details exist false is returned 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else                                                                                                            //If details exist true is returned which dispalys error message given with validation of Role name
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// To Manage different base categories of products in  a Online Shopping Site
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageBaseCategories()
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            var categories = (from b in db.BaseCategory_Table where b.BaseCatIsDeleted == false select b).ToList();
            ViewBag.BaseCategories = categories;
            return View();
        }
        /// <summary>
        /// To create new base categories in Online Shopping site.New base category is added to the BaseCategory_Table
        /// </summary>
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageBaseCategories(BaseCategory_Table category)                                          //new object of BaseCategory_Table are passed to post function ManageBaseCategories
        {
          
            category.BaseCatCreateDate = DateTime.Now;
            category.BaseCatCreatedBy = Session["user"].ToString();
            category.BaseCatUpdateDate = DateTime.Now;
            category.BaseCatUpdatedBy = Session["user"].ToString();
            category.BaseCatIsDeleted = false;                                                                             /* Values are added */
            db.BaseCategory_Table.Add(category);                                                                       //Adding values to Role_Table
            db.SaveChanges();

          
            return RedirectToAction("ManageBaseCategories");


        }
        /// <summary>
        /// This is to edit details of a particular base category in a grid.Inline editing is done by ajax method.
        /// </summary>
        /// <param name="BaseCatId"></param>
        /// <param name="BaseCatName"></param>
        /// <param name="BaseCatDescription"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BaseCategoryEdit(int BaseCatId, string BaseCatName, string BaseCatDescription)   //updated values are passed as parameters of post function through ajax method
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            BaseCategory_Table category = db.BaseCategory_Table.Find(BaseCatId);                            //Details of patricular base category to be edited is obtained using it's id
            category.BaseCatName = BaseCatName;
            category.BaseCatDesc = BaseCatDescription;

            category.BaseCatUpdatedBy = Session["user"].ToString();
            category.BaseCatIsDeleted = false;
            category.BaseCatUpdateDate = DateTime.Now;
            db.SaveChanges();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageBaseCategories", "Admin");  //passing url to which control to be navigated is stored in a variable
            return Json(new { BaseCatId = BaseCatId, BaseCatName = BaseCatName, BaseCatDesc = BaseCatDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet); //Values updated and url is returned back to ajax success function

        }
        /// <summary>
        ///  This is to delete details of a a particular base category from a grid.When data deleted from grid data that colyumn is deleted of that record is set to 1 .
        ///  By this that record become in active,So that will not appear in grid .Inline deleting is done by ajax method
        /// </summary>
        /// <param name="BaseCatId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BaseCategoryDelete(int BaseCatId)
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            BaseCategory_Table basecategory = db.BaseCategory_Table.Find(BaseCatId);                            // Details of patricular role to be deleted is obtained using it's id
            basecategory.BaseCatIsDeleted = true;                                                                   //Obtained data record is made inactive by setting value of isdeleted field to one
            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageBaseCategories", "Admin");    //passing url to which control to be navigated is stored in a variable
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);                       //Values updated and url is returned back to ajax success function

        }
        /// <summary>
        /// IsBaseCategoryNameExist is used to avoid duplicate base category name in BaseCategory_Table .
        /// This function called on validation of base category name.This to check whether the entered base category already exist.
        /// If it exist then error message will be diaplayed and it will not be added.
        /// </summary>
        /// <param name="BaseCatName"></param>
        /// <returns></returns>
        public JsonResult IsBaseCategoryNameExist(string BaseCatName)
        {
            var validateName = db.BaseCategory_Table.Where(x => x.BaseCatName == BaseCatName && x.BaseCatIsDeleted == false).FirstOrDefault();   //Details of base category that has same name of Entered rolename and which are active is taken

            if (validateName != null)                                                                                                          //If no such details exist false is returned 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else                                                                                                                                //If details exist true is returned which dispalys error message given with validation of Base Category
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// To Manage different product categories of products which comes under base category in  a Online Shopping Site
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageProductCategories()
        {
            var categoryexist = (from b in db.BaseCategory_Table where b.BaseCatIsDeleted == false select b).ToList();
                                   //list passed to viewbag to display in dropdown
            SelectList selectlist = new SelectList(categoryexist, "BaseCatId", "BaseCatName");
            ViewBag.basecategory = selectlist;
            var details = (from c in db.ProductCategory_Table                                       //Details of product category table obtained by joining two tables BaseCategory_Table and ProductCategory_Table
                           join e in db.BaseCategory_Table on c.BaseCatid equals e.BaseCatId
                           where c.ProductCatIsDeleted == false
                           select new
                           {
                               c.ProductCatId,
                               c.ProductCatName,
                               c.ProductCatDesc,
                               e.BaseCatName,
                               e.BaseCatId
                           });


            ViewBag.ProductCategories = details.ToList();                                           //Obtained details passed to a viewbag

            return View();
        }
        /// <summary>
        /// To create new product categories under a base category in Online Shopping site.New product  category is added to the ProductCategory_Table which has foreign key reference to BaseCategory_Table
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="basecategory"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ManageProductCategories(ProductCategory_Table categories, string basecategory)             //new product category is passed as object of  ProductCategory_Table to post function ManageProductCategories
        {
           
            categories.BaseCatid = Convert.ToInt32(basecategory);
            categories.ProductCatCreatedDate = DateTime.Now;
            categories.ProductCatCreatedBy = Session["user"].ToString();
            categories.ProductCatUpdatedDate = DateTime.Now;
            categories.ProductCateUpdatedBy = Session["user"].ToString();
            categories.ProductCatIsDeleted = false;                                                             /* Values are added */
            db.ProductCategory_Table.Add(categories);                                                           //Adding values to ProductCategory_Table                                                                             
            db.SaveChanges();


      

            return RedirectToAction("ManageProductCategories");

        }
        [HttpPost]
        public JsonResult ProductCategoryEdit(int ProductCatId, string ProductCatName, string ProductCatDescription)   //updated values are passed as parameters of post function through ajax method
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            ProductCategory_Table category = db.ProductCategory_Table.Find(ProductCatId);                              //Details of patricular product category to be edited is obtained using it's id
            var emp = (from u in db.ProductCategory_Table                                                              //Base Category Id of product category to be edited is found
                       where ProductCatId == u.ProductCatId
                       select u.BaseCatid).FirstOrDefault();
            category.BaseCatid = emp;
            category.ProductCatName = ProductCatName;
            category.ProductCatDesc = ProductCatDescription;
            category.ProductCatUpdatedDate = DateTime.Now;
            category.ProductCateUpdatedBy = Session["user"].ToString();
            category.ProductCatIsDeleted = false;
            db.SaveChanges();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageProductCategories", "Admin");//passing url to which control to be navigated is stored in a variable
            return Json(new { ProductCatId = ProductCatId, ProductCatName = ProductCatName, ProductCatDesc = ProductCatDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);//Values updated and url is returned back to ajax success function

        }
        [HttpPost]
        public JsonResult ProductCategoryDelete(int ProductCatId)
        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            ProductCategory_Table procategory = db.ProductCategory_Table.Find(ProductCatId);                              // Details of patricular role to be deleted is obtained using it's id
            procategory.ProductCatIsDeleted = true;                                                                           //Obtained data record is made inactive by setting value of isdeleted field to one
            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageProductCategories", "Admin");            //passing url to which control to be navigated is stored in a variable
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);                                  //Values updated and url is returned back to ajax success function
        }
        public JsonResult IsProductCategoryNameExist(string ProductCatName)
        {
            var validateName = db.ProductCategory_Table.FirstOrDefault
                                (x => x.ProductCatName == ProductCatName && x.ProductCatIsDeleted == false);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult ManageLocation()
        {

            var locations = (from l in db.Location_Table where l.LocationIsDeleted == false select l).ToList();                //Active records are obtained from Location_Table
            ViewBag.Locations = locations;
            return View();
        }
        [HttpPost]
        public ActionResult ManageLocation(Location_Table locations)                                                       //new location is passed as object of  Location_Table to post function ManageLocation                         
        {


            //if (ModelState.IsValid)

            //{
            locations.LocationCreatedBy = Session["user"].ToString();
            locations.LocationUpdatedBy = Session["user"].ToString();
            locations.LocationCreatedDate = DateTime.Now;
            locations.LocationUpdatedDate = DateTime.Now;
            locations.LocationIsDeleted = false;                                                                       /* Values are added */
            db.Location_Table.Add(locations);                                                                          //Adding values to ProductCategory_Table  
            db.SaveChanges();

        
            return RedirectToAction("ManageLocation");

        }
        [HttpPost]
        public JsonResult LocationEdit(int LocationId, string LocationName, int LocationPIN, string LocationDescription)          //updated values are passed as parameters of post function through ajax method
        {

            Location_Table locations = db.Location_Table.Find(LocationId);                                                       //Details of patricular product category to be edited is obtained using it's id
            locations.LocationName = LocationName;
            locations.LocationPIN = LocationPIN;
            locations.LocationDesc = LocationDescription;
            locations.LocationUpdatedBy = Session["user"].ToString();
            locations.LocationUpdatedDate = DateTime.Now;
            locations.LocationIsDeleted = false;
            db.SaveChanges();
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin");                           //passing url to which control to be navigated is stored in a variable
            return Json(new { LocationId = LocationId, LocationName = LocationName, LocationPIN = LocationPIN, LocationDesc = LocationDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);                                       //Values updated and url is returned back to ajax success function

        }

        [HttpPost]
        public JsonResult LocationDelete(int LocationId)
        {

            Location_Table locations = db.Location_Table.Find(LocationId);                               // Details of patricular role to be deleted is obtained using it's id
            locations.LocationIsDeleted = true;                                                          //Obtained data record is made inactive by setting value of isdeleted field to one
            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin");
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsLocationNameExist(string LocationName)
        {
            var validateName = db.Location_Table.FirstOrDefault
                                (x => x.LocationName == LocationName && x.LocationIsDeleted == false);
            if (validateName != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// ManageUser function is used by the Admin to manage users in a Online Shopping Site .
        /// Admin has right to  accept or decline users except buyers .So roles of each user has to checked to assign the right to the admin.
        /// Viewmodel is created to include details of users from User_Table and roles from Role_Table
        /// On accepting notification send as email to the user
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageUser()
        {
            List<Viewmodel> vlist = new List<Viewmodel>();                                                               //List of view model is created.
            foreach (var s in db.User_Table)                                                                              //Values are taken from usertable and stored in viewmodel
            {
                Viewmodel vm = new Viewmodel();
                vm.UserId = s.UserId;
                vm.Roleid = s.Roleid;
                vm.FirstName = s.FirstName;
                vm.LastName = s.LastName;
                vm.UserEmail = s.UserEmail;
                vm.UserAddress = s.UserAddress;
                vm.UserCreatedDate = s.UserCreatedDate.Date;
                vm.UserIsDeleted = s.UserIsDeleted;
                var user = (from r in db.Role_Table where r.RoleId == vm.Roleid select r.RoleName).FirstOrDefault();      //Rolename corresponding to Roleid in User_Table is obtained and stored in viewmodel
                vm.RoleName = user;
                if (vm.Roleid != 4)
                    vlist.Add(vm);                                                                                            //Values are added to list of type viewmodel 
            }
            return View(vlist.ToList());
        }
        /// <summary>
        /// AcceptUser is called on ajax sucess when Admin clicks on any of the Accept button to accept a particular user.
        /// On accepting user is made active and email will send to the user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult AcceptUser(int UserId)

        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            var confirmuser = (from u in db.User_Table where u.UserId == UserId select u).FirstOrDefault();                 //To accept user its details is taken from db
            confirmuser.UserIsDeleted = false;                                                                               //User is accepted by setting UserIsDeleted = false that user is made active
            confirmuser.UserUpdatedDate = DateTime.Now;
            confirmuser.UserUpdateBy = Session["user"].ToString();
            TempData["user"] = confirmuser.UserName;
            if (confirmuser.Roleid == 2)
            {
                var seller = (from s in db.Product_Table where s.SellerId == UserId select s).ToList();
                foreach (var item in seller)
                {
                    item.ProductIsDeleted = false;
                }
            }
            else if (confirmuser.Roleid == 3)
            {
                var service = (from sp in db.Service_Table where sp.ServiceProviderid == UserId select sp).ToList();
                foreach (var item in service)
                {
                    item.ServiceIsDeleted = false;

                }

            }

            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("SendMail", "Admin");                          //passing url to which control to be navigated is stored in a variable
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);                                   //Values updated and url is returned back to ajax success function



        }
        [HttpPost]
        public JsonResult DeclineUser(int UserId)

        {
            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            var confirmuser = (from u in db.User_Table where u.UserId == UserId select u).FirstOrDefault();                  //To decline user its details is taken from db
            confirmuser.UserIsDeleted = true;                                                                                //User is accepted by setting UserIsDeleted = false that user is made active
            confirmuser.UserUpdatedDate = DateTime.Now;
            confirmuser.UserUpdateBy = Session["user"].ToString();
            TempData["user"] = confirmuser.UserName;
            if (confirmuser.Roleid == 2)
            {
                var seller = (from s in db.Product_Table where s.SellerId == UserId select s).ToList();
                foreach (var item in seller)
                {
                    item.ProductIsDeleted = true;
                }
            }
            else if (confirmuser.Roleid == 3)
            {
                var service = (from sp in db.Service_Table where sp.ServiceProviderid == UserId select sp).ToList();
                foreach (var item in service)
                {
                    item.ServiceIsDeleted = true;

                }

            }
            db.SaveChanges();
            bool result = true;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("SendMail", "Admin");                           //passing url to which control to be navigated is stored in a variable
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);                                    //Values updated and url is returned back to ajax success function





        }

        [HttpGet]
        public ActionResult profile()
        {

            ViewBag.fill_msg = TempData["fill_msg"];
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {

            if (obj.FirstName != null && obj.LastName != null && obj.UserEmail != null && obj.UserAddress != null && obj.UserPhno != null)
            {
                string name = Session["user"].ToString();
                User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                user.FirstName = obj.FirstName;
                user.LastName = obj.LastName;
                user.UserEmail = obj.UserEmail;
                user.UserAddress = obj.UserAddress;
                user.UserPhno = obj.UserPhno;
                user.UserUpdatedDate = System.DateTime.Now;
                db.SaveChanges();
            }
            else
            {
                TempData["fill_msg"] = "Please Enter The Details";
                return RedirectToAction("profile");
            }
            return View();
        }
        public ActionResult SendMail()
        {
            string username = TempData["user"].ToString();
            var user = (from u in db.User_Table where u.UserName == username  select u).FirstOrDefault();
            if (ModelState.IsValid)
            {
                MailMessage mail = new MailMessage();
                string mailSubject, mailBody;
                var fromAddress = "factoryforshop@gmail.com";//change using project mail

                var toAddress = user.UserEmail;

                const string fromPassword = "shopfactory123";
                if(user.UserIsDeleted==true)
                {  
                     mailSubject = "ShopFactory Acknowledgement";
                     mailBody = "Sorry your request was decline as we are not satisfied with your profile.";
                }
                else
                {
                     mailSubject = "ShopFactory Acknowledgement";
                     mailBody = "Hi, your request is accepted .Login with your credentials ";
                }
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword); // admin name and password   
                smtp.EnableSsl = true;
                smtp.Send(fromAddress, toAddress, mailSubject, mailBody);
                TempData["message"] = "Mail Sent to the user";
                return RedirectToAction("ManageUser","Admin");
            }
            else
            {
                return RedirectToAction("ManageUser", "Admin");
            }
        }

        public ActionResult ChangePassword()
        {
            
            ViewBag.message = TempData["message"];
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User_Table obj = new User_Table();
            string name = Session["user"].ToString();
            User_Table details = (from a in db.User_Table where a.UserName == name select a).FirstOrDefault();
            if (details.Password == model.OldPassword)
            {
                if (details.Password == model.NewPassword)
                {
                    TempData["message"] = "your old password and new password are same!!!";
                }
                else if (model.NewPassword == model.ConfirmPassword)
                {
                    details.Password = model.NewPassword;
                    db.SaveChanges();
                    TempData["message"] = "password changes successfully!!";
                }
                else
                {
                    TempData["message"] = "confirm password an new password does not match";
                }
            }
            else
            {
                TempData["message"] = "your old password is incorrect ";
            }
            return RedirectToAction("ChangePassword");
        }

        public void logout()
        {
            Session["user"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }
    }
}