using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class UserController : Controller
    {
        //Model object
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        // GET: User


        #region  Register

        /// <summary>
        /// Create Method is used for Registration
        /// </summary>
        /// <returns></returns>


        public ActionResult create()
        {
            try
            {
                List<Role_Table> role = new List<Role_Table>();
                role = db.Role_Table.Where(x => x.RoleIsDeleted == false).ToList();
                var rolelist = new List<SelectListItem>();
                foreach (var item in role)
                {
                    rolelist.Add(new SelectListItem
                    {
                        Text = item.RoleName.ToString(),
                        Value = item.RoleId.ToString()
                    });
                }
                ViewBag.rolename = rolelist;
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();
        }
        [HttpPost]
        public ActionResult create(User_Table User)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    User.UserCreatedDate = System.DateTime.Now;
                    User.UserUpdatedDate = System.DateTime.Now;
                    User.UserCreatedBy = User.UserName;
                    User.UserUpdateBy = User.UserName;
                    if (User.Roleid == 1 || User.Roleid == 2 || User.Roleid == 3)
                    {
                        User.UserIsDeleted = true;
                        db.User_Table.Add(User);
                        db.SaveChanges();
                        return RedirectToAction("login");
                    }
                    else
                    {
                        User.UserIsDeleted = false;
                        db.User_Table.Add(User);
                        db.SaveChanges();
                        return RedirectToAction("login");
                    }
                }
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();
        }

        #endregion

        #region Login

        /// <summary>
        /// login is used for Login to application
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult login()
        {
            try
            {
                ViewBag.NewPassword = TempData["NewPassword"];
                ViewBag.InValidUser = TempData["InValidUser"];
                ViewBag.InValidPassword = TempData["InValidPassword"];
                ViewBag.Autharization = TempData["Autharization"];
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();
        }

        [HttpPost]
        public ActionResult login(string user, string password)
        {
            try
            {
                User_Table User = db.User_Table.Where(x => x.UserName == user).FirstOrDefault();

                if (User != null)
                {
                    Role_Table Role = db.Role_Table.Where(x => x.RoleId == User.Roleid).FirstOrDefault();
                    if (User.Password == password)
                    {
                        if (User.UserIsDeleted == false)
                        {

                            if (Role.RoleName == "Super_Admin")
                            {
                                Session["Admin"] = User.UserName;
                                return RedirectToAction("Homepage", "Admin");
                            }
                            else if (Role.RoleName == "Seller")
                            {
                                Session["Seller"] = User.UserName;
                                return RedirectToAction("Index", "Seller");
                            }
                            else if (Role.RoleName == "Courier_Service")
                            {
                                Session["Service"] = User.UserName;
                                Session["name"] = User.FirstName;
                                return RedirectToAction("Service_Home", "Service");
                            }
                            else if (Role.RoleName == "Buyer")
                            {
                                Session["Buyer"] = User.UserName;
                                Session["name"] = User.FirstName;

                                string name = User.UserName;
                                int id = User.UserId;
                                var oder_id = db.Order_Table.Where(x => x.Userid == id & x.OrderStatus == 0 & x.OrderIsDeleted == false).Select(x => x.OrderId).FirstOrDefault();
                                int count = db.OrderDetail_Table.Where(x => x.Orderid == oder_id).Count();
                                Session["count"] = count;
                                return RedirectToAction("loader", "User");
                            }
                            else
                            {
                                return RedirectToAction("Error");
                            }
                        }
                        else
                        {
                            TempData["Autharization"] = ConstantFile.Autharization;
                            return RedirectToAction("login", "User");
                        }

                    }
                    else
                    {
                        TempData["InValidPassword"] = ConstantFile.InValidPassword;
                        return RedirectToAction("login", "User");


                    }
                }
                else
                {
                    TempData["InValidUser"] = ConstantFile.InValidUser;
                    return RedirectToAction("login", "User");
                }
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();
        }
        #endregion

        #region Forget Password

        /// <summary>
        /// forget_pass is used for retrevial of password
        /// </summary>
        /// <returns></returns>

        [HttpGet]
        public ActionResult forget_pass()
        {
            ViewBag.CorrectCredentials = TempData["CorrectCredentials"];
            return View();
        }

        [HttpPost]
        public ActionResult forget_pass(string user, string email)
        {
            try
            {
                User_Table User = db.User_Table.Where(x => x.UserName == user & x.UserEmail == email).FirstOrDefault();
                if (User != null)
                {
                    User.Password = "user123";
                    db.SaveChanges();
                    TempData["NewPassword"] = ConstantFile.NewPassword;
                    return RedirectToAction("SendMail", new { email = email });
                }
                else
                {
                    TempData["CorrectCredentials"] = ConstantFile.CorrectCredentials;
                    return RedirectToAction("forget_pass", "User");
                }
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();
        }

        /// <summary>
        /// Mail is send to user with reset Password
        /// </summary>
        /// <returns></returns>

        public ActionResult SendMail(string email)
        {
            try
            {
                MailMessage mail = new MailMessage();

                var sender = ConfigurationManager.AppSettings["SenderEmail"];
                var senderPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];
              

                var toAddress = email;

               

                string mailSubject = " New Password ";
                string mailBody = "New Password : user123";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(sender, senderPassword); // admin name and password   
                smtp.EnableSsl = true;
                smtp.Send(sender, toAddress, mailSubject, mailBody);


                return RedirectToAction("login");
            }
            catch
            {
                Response.Redirect("Error");
            }
            return View();

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

        #region Loader

        /// <summary>
        /// Loader page for the Buyer(user) 
        /// </summary>
        /// <returns></returns>
        public ActionResult loader()
        {
            return View();
        }

        #endregion

        #region validations

        /// <summary>
        /// Checks password and confirm Password is matching during registration
        /// </summary>
        /// <returns></returns>

        public JsonResult confirm_pass(string pass, string cp)
        {
            int res = 0;
            if (pass == cp)
            {
                res = 1;
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Checks if username Already Exist
        /// </summary>
        /// <returns></returns>

        public JsonResult IsNameExist(string UserName)
        {
            var validateName = db.User_Table.Where(x => x.UserName == UserName && x.UserIsDeleted == false).FirstOrDefault();   //Details of base category that has same name of Entered rolename and which are active is taken

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
        /// Checks if User Mailid Already Exist
        /// </summary>
        /// <returns></returns>

        public JsonResult IsmailExist(string UserEmail)
        {
            var validateName = db.User_Table.Where(x => x.UserEmail == UserEmail && x.UserIsDeleted == false).FirstOrDefault();   //Details of base category that has same name of Entered rolename and which are active is taken

            if (validateName != null)                                                                                                          //If no such details exist false is returned 
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else                                                                                                                                //If details exist true is returned which dispalys error message given with validation of Base Category
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
    }

}

        


        

       


   