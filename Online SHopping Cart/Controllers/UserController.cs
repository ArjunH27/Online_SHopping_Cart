﻿using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
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
            try { 
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
        public ActionResult create(User_Table obj)
        {
            try { 
            if (ModelState.IsValid)
            {
                obj.UserCreatedDate = System.DateTime.Now;
                obj.UserUpdatedDate = System.DateTime.Now;
                if (obj.Roleid == 1 || obj.Roleid == 2 || obj.Roleid == 3)
                {
                    obj.UserCreatedBy = obj.UserName;
                    obj.UserUpdateBy = obj.UserName;
                    obj.UserIsDeleted = true;
                    db.User_Table.Add(obj);
                    db.SaveChanges();
                    return RedirectToAction("login");
                }
                else
                {
                    obj.UserCreatedBy = obj.UserName;
                    obj.UserUpdateBy = obj.UserName;
                    obj.UserIsDeleted = false;
                    db.User_Table.Add(obj);
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
            try { 
            ViewBag.cmsg = TempData["cmsg"];
            ViewBag.message1 = TempData["message1"];
            ViewBag.message2 = TempData["message2"];
            ViewBag.message3 = TempData["message3"];
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
            try { 
            User_Table obj = db.User_Table.Where(x => x.UserName == user).FirstOrDefault();
          
            if (obj != null)
            {
                Role_Table robj = db.Role_Table.Where(x => x.RoleId == obj.Roleid).FirstOrDefault();
                if (obj.Password == password)
                {
                    if (obj.UserIsDeleted == false)
                    {

                        if (robj.RoleName == "Super_Admin")
                        {
                            Session["Admin"] = obj.UserName;
                            return RedirectToAction("Homepage", "Admin");
                        }
                        else if (robj.RoleName == "Seller")
                        {
                            Session["Seller"] = obj.UserName;
                            return RedirectToAction("Index", "Seller");
                        }
                        else if (robj.RoleName == "Courier_Service")
                        {
                            Session["Service"] = obj.UserName;
                            Session["name"] = obj.FirstName;
                            return RedirectToAction("Service_Home", "Service");
                        }
                        else if (robj.RoleName == "Buyer")
                        {
                            Session["Buyer"] = obj.UserName;
                            Session["name"] = obj.FirstName;
                           
                            string name = obj.UserName;
                            int id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
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
                        TempData["message3"] = "Not an Autharized User";
                        return RedirectToAction("login", "User");
                    }

                }
                else
                {
                    TempData["message2"] = "Password Dont Match";
                    return RedirectToAction("login", "User");


                }
            }
            else
            {
                TempData["message1"] = "User Does Not Exist";
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
            ViewBag.for_valid = TempData["for_valid"];
            return View();
        }

        [HttpPost]
        public ActionResult forget_pass(string user,string email)
        {
            try { 
            User_Table obj = db.User_Table.Where(x => x.UserName == user & x.UserEmail == email).FirstOrDefault();
            if(obj!=null)
            {
                obj.Password = "user123";
                db.SaveChanges();
                TempData["cmsg"] = "New Psssword is send to your Mail";
                return RedirectToAction("SendMail", new { email = email });
            }
            else
            {
                TempData["for_valid"] = "Enter the Correct Credentials";
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
            try {    
                MailMessage mail = new MailMessage();

                var fromAddress = "factoryforshop@gmail.com";

                var toAddress = email;

                const string fromPassword = "shopfactory123";

                string mailSubject = " New Password ";
                string mailBody = "New Password : user123";
                mail.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "smtp.gmail.com";
                smtp.Port = 587;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new System.Net.NetworkCredential(fromAddress, fromPassword); // admin name and password   
                smtp.EnableSsl = true;
                smtp.Send(fromAddress, toAddress, mailSubject, mailBody);


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
            if(pass==cp)
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

        


        

       


   