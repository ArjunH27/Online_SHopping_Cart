using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class AdminController : Controller
    {
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        public ActionResult Homepage()
        {
            try
            {

                DateTime today = System.DateTime.Now.Date;

                //Registered user count is obtained 
                var UserCount = (from u in db.User_Table
                                 where u.UserIsDeleted == false
                                 select u).Count();
                ViewBag.UserCount = UserCount;

                //New users of today obtained 
                var NewUserCount = (from u in db.User_Table
                                    where u.UserCreatedDate == today
                                    select u).Count();
                ViewBag.NewUserCount = NewUserCount;

                //New users of today which are unauthorized obtained 
                var unauthorised = (from u in db.User_Table
                                    where u.UserIsDeleted == true && u.UserCreatedDate == today
                                    select u).Count();
                ViewBag.unauthorised = unauthorised;

                //New users of today which are authorized obtained
                var NewRegisteredUserCount = (from u in db.User_Table
                                              where u.UserIsDeleted == false && u.UserCreatedDate == today
                                              select u).Count();
                ViewBag.NewRegisteredUserCount = NewRegisteredUserCount;

                //Sellers count obatained from User Table 
                var SellerCount = (from u in db.User_Table
                                   where u.UserIsDeleted == false && u.Roleid == 2
                                   select u).Count();
                ViewBag.SellerCount = SellerCount;

                //Admin count obatained from User Table 
                var AdminCount = (from u in db.User_Table
                                  where u.UserIsDeleted == false && u.Roleid == 1
                                  select u).Count();
                ViewBag.AdminCount = AdminCount;

                //Buyers count obatained from User Table
                var BuyerCount = (from u in db.User_Table
                                  where u.UserIsDeleted == false && u.Roleid == 4
                                  select u).Count();
                ViewBag.BuyerCount = BuyerCount;

                //Service Providers count obatained from User_Table by checking role and whether it is authorized
                var Servicecount = (from u in db.User_Table
                                    where u.UserIsDeleted == false && u.Roleid == 3
                                    select u).Count();
                ViewBag.Servicecount = Servicecount;

                //Base Categories count from BaseCategory table 
                var BaseCategoriesCount = (from b in db.BaseCategory_Table
                                           where b.BaseCatIsDeleted == false
                                           select b).Count();
                ViewBag.BaseCategoriesCount = BaseCategoriesCount;

                //Product Categories count from ProductCategory table 
                var ProductCategoriesCount = (from p in db.ProductCategory_Table
                                              where p.ProductCatIsDeleted == false
                                              select p).Count();
                ViewBag.ProductCategoriesCount = ProductCategoriesCount;

                //Location Categories count from Location table 
                var LocationsCount = (from l in db.Location_Table
                                      where l.LocationIsDeleted == false
                                      select l).Count();
                ViewBag.LocationsCount = LocationsCount;

                //Today'ssales obtained from Order table 
                var sales = (from o in db.Order_Table
                             where o.OrderStatus == 1
                             && o.OrderCreatedDate == today
                             select o).Count();
                ViewBag.sales = sales;

                //Today's order obtained from Order table 
                var orders = (from o in db.Order_Table
                              where o.OrderStatus == 0
                              && o.OrderCreatedDate == today
                              select o).Count();
                ViewBag.orders = orders;
                return View();
            }
            catch
            {
                return View("Error");
            }
        }
        #region Manage Role
        /// <summary>
        /// To Manage different roles in Online Shopping Site.
        /// Admin can add new roles.Also can edit existing roles
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageRole()
        {
            try
            {
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //Getting roles created from Role_Table 
                var roles = (from r in db.Role_Table where r.RoleIsDeleted == false select r).ToList();

                ViewBag.Roles = roles;
                return View();
            }
            catch
            {
                return View("Error");
            }

        }

        /// <summary>
        /// Post function of Manage Role to create new roles.
        /// New role is added to the Role_Table
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        [HttpPost]
        //new role passed  as parameter
        public ActionResult ManageRole(Role_Table role)
        {
            try
            {
                //Rest of values are that are passed from view are added
                role.RoleCreatedBy = Session["Admin"].ToString(); 
                role.RoleUpdatedBy = Session["Admin"].ToString();
                role.RoleCreatedDate = DateTime.Now;
                role.RoleUpdateDate = DateTime.Now;
                role.RoleIsDeleted = false;
                //Adding values to Role_Table
                db.Role_Table.Add(role);
                db.SaveChanges();
                return RedirectToAction("ManageRole");
            }
            catch
            {
                return View("Error");

            }
            
        }
        /// <summary>
        /// This is to edit role details in a grid.Inline editing is done by ajax method.
        /// </summary>
        /// <param name="RoleId"></param>
        /// <param name="RoleName"></param>
        /// <param name="RoleDescription"></param>
        /// <returns></returns>
        [HttpPost]
        //updated values are passed as parameters 
        public JsonResult RoleEdit(int RoleId, string RoleName, string RoleDescription)
        {
            
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //Details of patricular role is obtained using it's id
                Role_Table role = db.Role_Table.Find(RoleId);
                role.RoleName = RoleName;
                role.RoleDesc = RoleDescription;
                role.RoleUpdatedBy = Session["Admin"].ToString();
                role.RoleIsDeleted = false;
                role.RoleUpdateDate = DateTime.Now;
                db.SaveChanges();
                //passing url to which control to be navigated is stored in a variable                                                            
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRole", "Admin");
                return Json(new { RoleId = RoleId, RoleName = RoleName, RoleDesc = RoleDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
           
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
                //Details of patricular role to be deleted is obtained using it's id
                Role_Table role = db.Role_Table.Find(RoleId);
                role.RoleIsDeleted = true;
                db.SaveChanges();
                bool result = true;
                //passing url to which control to be navigated is stored in a variable
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageRole", "Admin");
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
            
                //Details of roles that has same name of Entered rolename obtained
                var validateName = db.Role_Table.FirstOrDefault(x => x.RoleName == RoleName && x.RoleIsDeleted == false);
                //If such details exist false is returned       
                if (validateName != null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                //If details not exist true is returned which dispalys error message given with validation of Role name
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
           
        }
        #endregion
        #region Manage Base Categories
        /// <summary>
        /// To Manage different base categories of products in  a Online Shopping Site
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageBaseCategories()
        {
            try
            {
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                var categories = (from b in db.BaseCategory_Table where b.BaseCatIsDeleted == false select b).ToList();
                ViewBag.BaseCategories = categories;
                return View();
            }
            catch
            {
                return View("Error");
            }
            
        }
        /// <summary>
        /// To create new base categories in Online Shopping site.New base category is added to the BaseCategory_Table
        /// </summary>
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        [HttpPost]
        //new base category  passed as a parameter
        public ActionResult ManageBaseCategories(BaseCategory_Table category)
        {
            try
            {
                category.BaseCatCreateDate = DateTime.Now;
                category.BaseCatCreatedBy = Session["Admin"].ToString();
                category.BaseCatUpdateDate = DateTime.Now;
                category.BaseCatUpdatedBy = Session["Admin"].ToString();
                category.BaseCatIsDeleted = false;
                //Adding values to Role_Table                                                                         
                db.BaseCategory_Table.Add(category);
                db.SaveChanges();
                return RedirectToAction("ManageBaseCategories");
            }
            catch
            {
                return View("Error");
            }
          

        }
        /// <summary>
        /// This is to edit details of a particular base category in a grid.Inline editing is done by ajax method.
        /// </summary>
        /// <param name="BaseCatId"></param>
        /// <param name="BaseCatName"></param>
        /// <param name="BaseCatDescription"></param>
        /// <returns></returns>
        [HttpPost]
        //updated values are passed as parameters of post function through ajax method
        public JsonResult BaseCategoryEdit(int BaseCatId, string BaseCatName, string BaseCatDescription)
        {
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //Details of patricular base category to be edited is obtained using it's id
                BaseCategory_Table category = db.BaseCategory_Table.Find(BaseCatId);
                category.BaseCatName = BaseCatName;
                category.BaseCatDesc = BaseCatDescription;
                category.BaseCatUpdatedBy = Session["Admin"].ToString();
                category.BaseCatIsDeleted = false;
                category.BaseCatUpdateDate = DateTime.Now;
                db.SaveChanges();
                //passing url to which control to be navigated is stored in a variable
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageBaseCategories", "Admin");
                //Values updated and url is returned back to ajax success function
                return Json(new { BaseCatId = BaseCatId, BaseCatName = BaseCatName, BaseCatDesc = BaseCatDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        ///  This is to delete details of a a particular base category 
        ///  Inline deleting is done by ajax method
        /// </summary>
        /// <param name="BaseCatId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult BaseCategoryDelete(int BaseCatId)
        {

            ShoppingCartDbEntities db = new ShoppingCartDbEntities();
            // Details of patricular role to be deleted is obtained using it's id
            BaseCategory_Table basecategory = db.BaseCategory_Table.Find(BaseCatId);
            //Record obtained is soft  deleted                          
            basecategory.BaseCatIsDeleted = true;
            db.SaveChanges();
            bool result = true;
            //passing url to which control to be navigated is stored in a variable
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageBaseCategories", "Admin");
            //Values updated and url is returned back to ajax success function  
            return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
        }
            
        /// <summary>
        /// To avoid duplicate entry of existing base category
        /// This function called on validation of base category name.
        /// </summary>
        /// <param name="BaseCatName"></param>
        /// <returns></returns>
        public JsonResult IsBaseCategoryNameExist(string BaseCatName)
        {
           
                //Details of base category that has same name of Entered rolename 
                var validateName = db.BaseCategory_Table.Where(x => x.BaseCatName == BaseCatName && x.BaseCatIsDeleted == false).FirstOrDefault();
                //If  such details exist false is returned 
                if (validateName != null)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
                //If details exist true is returned which dispalys error message given in Remote validation
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
           
        }
        #endregion
        #region Manage Product Categories
        /// <summary>
        /// To Manage different product categories of products which comes under base category in  a Online Shopping Site
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageProductCategories()
        {

            try
            {
                //Base Category deatils obatined
                var categoryexist = (from b in db.BaseCategory_Table where b.BaseCatIsDeleted == false select b).ToList();
                //Base Category Name and id passed to select list for dropdown               
                SelectList selectlist = new SelectList(categoryexist, "BaseCatId", "BaseCatName");
                ViewBag.basecategory = selectlist;
                //Details of product category  obtained by joining two tables BaseCategory_Table and ProductCategory_Table
                var details = (from c in db.ProductCategory_Table
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

                ViewBag.ProductCategories = details.ToList();

                return View();
            }
            catch
            {
                return View("Error");
            }
            
        }
        /// <summary>
        /// To create new product categories under a base category in Online Shopping site.New product  category is added to the ProductCategory_Table which has foreign key reference to BaseCategory_Table
        /// </summary>
        /// <param name="categories"></param>
        /// <param name="basecategory"></param>
        /// <returns></returns>
        [HttpPost]
        //new product category is passed as object to post function ManageProductCategories
        public ActionResult ManageProductCategories(ProductCategory_Table categories, string basecategory)
        {
            try
            {
                categories.BaseCatid = Convert.ToInt32(basecategory);
                categories.ProductCatCreatedDate = DateTime.Now;
                categories.ProductCatCreatedBy = Session["Admin"].ToString();
                categories.ProductCatUpdatedDate = DateTime.Now;
                categories.ProductCateUpdatedBy = Session["Admin"].ToString();
                categories.ProductCatIsDeleted = false;
                //Adding values to ProductCategory_Table                                                           
                db.ProductCategory_Table.Add(categories);
                db.SaveChanges();
                return RedirectToAction("ManageProductCategories");
            }
            catch
            {
                return View("Error");
            }
            

        }
        /// <summary>
        /// This is to edit details of a particular product category in a grid.Inline editing is done by ajax method.
        /// </summary>
        /// <param name="ProductCatId"></param>
        /// <param name="ProductCatName"></param>
        /// <param name="ProductCatDescription"></param>
        /// <param name="BaseCatid"></param>
        /// <returns></returns>
        [HttpPost]
        //updated values are passed as parameters of post function through ajax method

        public JsonResult ProductCategoryEdit(int ProductCatId, string ProductCatName, string ProductCatDescription, int BaseCatid)
        {
           
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //Details of patricular product category to be edited is obtained using it's id
                ProductCategory_Table category = db.ProductCategory_Table.Find(ProductCatId);
                category.BaseCatid = BaseCatid;
                category.ProductCatName = ProductCatName;
                category.ProductCatDesc = ProductCatDescription;
                category.ProductCatUpdatedDate = DateTime.Now;
                category.ProductCateUpdatedBy = Session["Admin"].ToString();
                category.ProductCatIsDeleted = false;
                db.SaveChanges();
                //passing url to which control to be navigated is stored in a variable
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageProductCategories", "Admin");
                return Json(new { ProductCatId = ProductCatId, ProductCatName = ProductCatName, ProductCatDesc = ProductCatDescription, BaseCatid = BaseCatid, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        /// This is to delete details of a a particular product category 
        ///  Inline deleting is done by ajax method
        /// </summary>
        /// <param name="ProductCatId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ProductCategoryDelete(int ProductCatId)
        {
            
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                // Details of patricular role to be deleted is obtained using it's id
                ProductCategory_Table procategory = db.ProductCategory_Table.Find(ProductCatId);
                //Obtained data record is soft deleted                       
                procategory.ProductCatIsDeleted = true;
                db.SaveChanges();
                bool result = true;
                //passing url to which control to be navigated is stored in a variable
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageProductCategories", "Admin");
                return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
           
        }
        /// <summary>
        /// To avoid duplicate entry of existing product category  
        /// This function called on validation of product category name.
        /// </summary>
        /// <param name="ProductCatName"></param>
        /// <returns></returns>
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
        #endregion
        #region Manage Location
        /// <summary>
        /// To Manage different locations.
        /// New locations can be added and existing one can be edited or deleted
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageLocation()
        {
            try
            {
                //Locaion details are obtained 
                var locations = (from l in db.Location_Table where l.LocationIsDeleted == false select l).ToList();
                ViewBag.Locations = locations;
                return View();
            }
            catch
            {
                return View("Error");
            }
           
        }
        [HttpPost]
        //updated values are passed as parameters of post function through ajax method
        public ActionResult ManageLocation(Location_Table locations)
        {
            try
            {
                locations.LocationCreatedBy = Session["Admin"].ToString();
                locations.LocationUpdatedBy = Session["Admin"].ToString();
                locations.LocationCreatedDate = DateTime.Now;
                locations.LocationUpdatedDate = DateTime.Now;
                locations.LocationIsDeleted = false;
                //Adding values to ProductCategory_Table                                                                      
                db.Location_Table.Add(locations);
                db.SaveChanges();
                return RedirectToAction("ManageLocation");
            }
            catch
            {
                return View("Error");
            }

       
        }
        [HttpPost]
        //updated values are passed as parameters
        public JsonResult LocationEdit(int LocationId, string LocationName, int LocationPIN, string LocationDescription)
        {
            
                //Details of patricular product category to be edited is obtained using it's id
                Location_Table locations = db.Location_Table.Find(LocationId);
                locations.LocationName = LocationName;
                locations.LocationPIN = LocationPIN;
                locations.LocationDesc = LocationDescription;
                locations.LocationUpdatedBy = Session["Admin"].ToString();
                locations.LocationUpdatedDate = DateTime.Now;
                locations.LocationIsDeleted = false;
                db.SaveChanges();
                //passing url to which control to be navigated is stored in a variable
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin");
                return Json(new { LocationId = LocationId, LocationName = LocationName, LocationPIN = LocationPIN, LocationDesc = LocationDescription, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
           

        }
        /// <summary>
        /// To delete existing locations
        /// </summary>
        /// <param name="LocationId"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LocationDelete(int LocationId)
        {
            
                // Details of patricular role to be deleted is obtained using it's id
                Location_Table locations = db.Location_Table.Find(LocationId);
                locations.LocationIsDeleted = true;
                db.SaveChanges();
                bool result = true;
                var redirectUrl = new UrlHelper(Request.RequestContext).Action("ManageLocation", "Admin");
                return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
         
        }
        /// <summary>
        ///To avoid duplicate entry of already existing location
        /// </summary>
        /// <param name="LocationName"></param>
        /// <returns></returns>
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
        #endregion
        #region Manage User
        /// <summary>
        /// ManageUser function is used by the Admin to manage users in a Online Shopping Site .
        /// Admin has right to  accept or decline users except buyers .So roles of each user has to checked to assign the right to the admin.
        /// Viewmodel is created to include details of users from User_Table and roles from Role_Table
        /// On accepting notification send as email to the user
        /// </summary>
        /// <returns></returns>
        public ActionResult ManageUser()
        {
            try
            {
                List<Viewmodel> vlist = new List<Viewmodel>();
                foreach (var s in db.User_Table)
                {
                    Viewmodel vm = new Viewmodel();
                    //Values are stored in object of viewmodel 
                    vm.UserId = s.UserId;
                    vm.Roleid = s.Roleid;
                    vm.FirstName = s.FirstName;
                    vm.LastName = s.LastName;
                    vm.UserEmail = s.UserEmail;
                    vm.UserAddress = s.UserAddress;
                    vm.UserCreatedDate = s.UserCreatedDate.Date;
                    vm.UserIsDeleted = s.UserIsDeleted;
                    //Rolename corresponding to Roleid in User_Table is obtained and stored in viewmodel
                    var user = (from r in db.Role_Table where r.RoleId == vm.Roleid select r.RoleName).FirstOrDefault();
                    vm.RoleName = user;
                    if (vm.Roleid != 4)
                        vlist.Add(vm);
                }
                return View(vlist.ToList());
            }
            catch
            {
                return View("Error");
            }
         
        }

        /// <summary>
        /// AcceptUser is called on ajax sucess when Admin accept a particular user.
        /// On accepting user is made active and email will send to the user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>

        [HttpPost]
        public JsonResult AcceptUser(int UserId)

        {
            
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //To accept a user its details is taken 
                var confirmuser = (from u in db.User_Table where u.UserId == UserId select u).FirstOrDefault();
                //User is accepted              
                confirmuser.UserIsDeleted = false;
                confirmuser.UserUpdatedDate = DateTime.Now;
                confirmuser.UserUpdateBy = Session["Admin"].ToString();
                TempData["user"] = confirmuser.UserName;
                //if user is a seller its products are accepted
                if (confirmuser.Roleid == 2)
                {
                    var seller = (from s in db.Product_Table where s.SellerId == UserId select s).ToList();
                    foreach (var item in seller)
                    {
                        item.ProductIsDeleted = false;
                    }
                }
                //if user is a service provider its services are accepted
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
                return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
           
        }


        /// <summary>
        /// DeclineUser is called on ajax sucess when Admin  decline a particular user.
        /// On declining user is made active and email will send to the user
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>


        [HttpPost]
        public JsonResult DeclineUser(int UserId)

        {
           
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                //To decline user its details is taken
                var confirmuser = (from u in db.User_Table where u.UserId == UserId select u).FirstOrDefault();
                //User is declined                
                confirmuser.UserIsDeleted = true;
                confirmuser.UserUpdatedDate = DateTime.Now;
                confirmuser.UserUpdateBy = Session["Admin"].ToString();
                TempData["user"] = confirmuser.UserName;
                //if user is a seller its products are removed
                if (confirmuser.Roleid == 2)
                {
                    var seller = (from s in db.Product_Table where s.SellerId == UserId select s).ToList();
                    foreach (var item in seller)
                    {
                        item.ProductIsDeleted = true;
                    }
                }
                //if user is a service provider its services are removed
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
                return Json(new { result, Url = redirectUrl }, JsonRequestBehavior.AllowGet);
            
        }
        /// <summary>
        /// To send the mail on accept or decline of a user
        /// </summary>
        /// <returns></returns>
        public ActionResult SendMail()
        {
            try
            {
                var sender = ConfigurationManager.AppSettings["SenderEmail"];
                var senderPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];
                string username = TempData["user"].ToString();
                var user = (from u in db.User_Table where u.UserName == username select u).FirstOrDefault();
                if (ModelState.IsValid)
                {
                    MailMessage mail = new MailMessage();



                    var toAddress = user.UserEmail;

                    if (user.UserIsDeleted == true)
                    {
                        mail.Subject = "ShopFactory Acknowledgement";
                        mail.Body = "Sorry your request was decline as we are not satisfied with your profile.";
                    }
                    else
                    {
                        mail.Subject = "ShopFactory Acknowledgement";
                        mail.Body = "Hi, your request is accepted .Login with your credentials ";
                    }
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new System.Net.NetworkCredential(sender, senderPassword); // admin name and password   
                    smtp.EnableSsl = true;
                    smtp.Send(sender, toAddress, mail.Subject, mail.Body);
                    TempData["message"] = "Mail Sent to the user";
                    return RedirectToAction("ManageUser", "Admin");
                }
                else
                {
                    return RedirectToAction("ManageUser", "Admin");
                }
            }
            catch
            {
                return View("Error");
            }


        }
        #endregion
        #region profile settings
        /// <summary>
        /// To view or edit admin profile
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult profile()
        {
            try
            {
                ViewBag.fill_msg = TempData["fill_msg"];
                // name of logged user obtained from session
                string name = Session["Admin"].ToString();
                // Details of logged user obtained
                User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                return View(obj);
            }
            catch
            {
                return View("Error");

            }
         
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
            try
            {
                if (obj.FirstName != null && obj.LastName != null && obj.UserEmail != null && obj.UserAddress != null && obj.UserPhno != null)
                {
                    string name = Session["Admin"].ToString();
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
            catch
            {
                return View("Error");
            }
            
        }

        /// <summary>
        /// Tp change the password of a logged user
        /// </summary>
        /// <returns></returns>
        public ActionResult ChangePassword()
        {
            try
            {

                ViewBag.ChangePassword = TempData["ChangePassword"];
                return View();
            }
            catch
            {
                return View("Error");
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                User_Table obj = new User_Table();
                string name = Session["Admin"].ToString();
                User_Table details = (from a in db.User_Table where a.UserName == name select a).FirstOrDefault();
                if (details.Password == model.OldPassword)
                {
                    if (details.Password == model.NewPassword)
                    {
                        TempData["ChangePassword"] = ConstantFile.PasswordConflict;
                    }
                    else if (model.NewPassword == model.ConfirmPassword)
                    {
                        details.Password = model.NewPassword;
                        db.SaveChanges();
                        TempData["ChangePassword"] = ConstantFile.PasswordChangeSuccess;
                    }
                    else
                    {
                        TempData["ChangePassword"] = ConstantFile.ConfirmPasswordConflict;
                    }
                }
                else
                {
                    TempData["ChangePassword"] = ConstantFile.OldPassworWrong;
                }
                return RedirectToAction("ChangePassword");
            }
            catch
            {
                return View("Error");

            }
        

        }

        /// <summary>
        /// To logout from Online Shopping Site for  a user logged in 
        /// </summary>
        public void logout()
        {

            Session["Admin"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }
        #endregion
        #region Error

        /// <summary>
        /// Custom Error Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Error()
        {
            return View();
        }
        #endregion
    }
}