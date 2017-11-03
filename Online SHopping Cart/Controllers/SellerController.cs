﻿using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.Controllers
{
    public class SellerController : Controller
    {
        public int sellercount = 0;
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        SellerViewModel svm = new SellerViewModel();
        // GET: Seller
        [HttpGet]
        public ActionResult Index()
        {
            Notification_Count();

            return View();
        }
        public void Notification_Count()
        {
            string name = Session["user"].ToString();
            int Id = db.User_Table.Where(x => x.UserName == name).Select(x => x.UserId).FirstOrDefault();
            List<OrderDetail_Table> orderlist = new List<OrderDetail_Table>();
            var product = (from a in db.Product_Table where a.SellerId == Id && a.ProductIsDeleted == false select a).ToList();

            var orders = (from o in db.Order_Table where o.OrderStatus == 1 && o.OrderIsDeleted == false && o.OrderNotification == "00" || o.OrderNotification == "01" select o).ToList();
            foreach (var item in orders)
            {
                foreach (var item1 in product)
                {
                    OrderDetail_Table order = (from or in db.OrderDetail_Table where item.OrderId == or.Orderid && or.Productid == item1.ProductId select or).FirstOrDefault();
                    if (order != null)
                    {
                        orderlist.Add(order);
                    }

                }
            }
            ViewBag.ordering = orderlist;
            Session["notif-count"] = orderlist.Count();
        }


        public void DisableNotification()
        {
            Order_Table obj = new Order_Table();
            var order = (from a in db.Order_Table select a).ToList();
            foreach (var item in order)
            {
                if (item.OrderNotification == "00")
                {
                    item.OrderNotification = "10";
                }
                else if (item.OrderNotification == "01")
                {
                    item.OrderNotification = "11";
                }

            }
            db.SaveChanges();
        }

        [HttpGet]
        public ActionResult Create()
        {
            Notification_Count();
            List<BaseCategory_Table> category = new List<BaseCategory_Table>();
            category = db.BaseCategory_Table.ToList();
            var productlist = new List<SelectListItem>();
            foreach (var item in category)
            {
                productlist.Add(new SelectListItem
                {
                    Text = item.BaseCatName.ToString(),
                    Value = item.BaseCatId.ToString(),

                });

                ViewBag.basecategory = productlist;

            }

            return View();
        }
        public ActionResult GetProductCat(string id)
        {
            int BaseCatId;
            List<SelectListItem> ProductcategoryList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(id))
            {
                BaseCatId = Convert.ToInt32(id);
                List<ProductCategory_Table> productCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == BaseCatId & x.ProductCatIsDeleted == false).ToList();
                foreach (var item in productCategory)
                {
                    ProductcategoryList.Add(new SelectListItem
                    {
                        Text = item.ProductCatName.ToString(),
                        Value = item.ProductCatId.ToString(),

                    });
                }

                ViewBag.procat = ProductcategoryList;
            }
            return Json(ProductcategoryList, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult Create(Image_Table model, Product_Table product)
        {
            int j;
            Notification_Count();
            Image_Table image = new Image_Table();
            if (ModelState.IsValid)
            {
                ShoppingCartDbEntities db = new ShoppingCartDbEntities();
                string name = Session["user"].ToString();
                int id = (from user in db.User_Table where user.UserName == name select user.UserId).FirstOrDefault();
                product.SellerId = id;
                product.ProductCreatedBy = Session["user"].ToString();
                product.ProductCreatedDate = DateTime.Now;
                product.ProductUpdatedBy = Session["user"].ToString();
                product.ProductUpdatedDate = DateTime.Now;
                product.ProductIsDeleted = false;
                db.Product_Table.Add(product);

                db.SaveChanges();


                object[] imgarray = new object[5];
                int p = product.ProductId;
                HttpPostedFileBase file = Request.Files["ImageData"];
                for (j = 0; j < Request.Files.Count; j++)
                {
                    file = Request.Files[j];

                    ContentRepository service = new ContentRepository();
                    image = service.UploadImageInDataBase(file, model);

                    Image_Table imageObj = new Image_Table();


                    imageObj.BinaryImage = image.BinaryImage;
                    imageObj.Productid = product.ProductId;
                    imageObj.ImageCreatedBy = Session["user"].ToString();
                    imageObj.ImageCreatedDate = DateTime.Now;
                    imageObj.ImageUpdatedBy = Session["user"].ToString();
                    imageObj.ImageUpdatedDate = DateTime.Now;
                    imageObj.ImageIsDeleted = false;
                    db.Image_Table.Add(imageObj);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("display");

        }
        [HttpPost]
        public JsonResult Edit(int id, string name, decimal price, string features, int stock)
        {
            Product_Table pt = db.Product_Table.Find(id);



            pt.ProductName = name;
            pt.ProductPrice = price;
            pt.ProductDesc = features;
            pt.ProductStock = stock;

            pt.ProductUpdatedBy = Session["user"].ToString();
            pt.ProductUpdatedDate = DateTime.Now;
            db.SaveChanges();

            return Json(new { id = id, ProductName = name, ProductPrice = price, ProductDesc = features, ProductStock = stock }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            Product_Table product = db.Product_Table.Find(id);
            product.ProductUpdatedBy = Session["user"].ToString();
            product.ProductUpdatedDate = DateTime.Now;
            product.ProductIsDeleted = true;
            var image = (from a in db.Image_Table where a.Productid == product.ProductId select a).ToList();
            foreach (var im in image)
            {
                im.ImageUpdatedBy = Session["user"].ToString();
                im.ImageUpdatedDate = DateTime.Now;
                im.ImageIsDeleted = true;
            }

            db.SaveChanges();
            bool result = true;
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult display()
        {
            Notification_Count();
            string uname = Session["user"].ToString();
            int uid = (from a in db.User_Table where a.UserName == uname select a.UserId).FirstOrDefault();
            List<SellerViewModel> obj1 = new List<SellerViewModel>();

            foreach (var b in db.Product_Table)

            {
                if (b.ProductIsDeleted == false && b.SellerId == uid)
                {




                    SellerViewModel obj = new SellerViewModel();

                    obj.ProductId = b.ProductId;
                    obj.ProductName = b.ProductName;
                    var name = (from a in db.ProductCategory_Table where a.ProductCatId == b.ProductCatid select a.ProductCatName).FirstOrDefault();
                    int id = (from c in db.ProductCategory_Table where c.ProductCatId == b.ProductCatid select c.BaseCatid).FirstOrDefault();
                    var catname = (from d in db.BaseCategory_Table where d.BaseCatId == id select d.BaseCatName).FirstOrDefault();
                    obj.BaseCatName = catname;
                    obj.ProductCatName = name;
                    obj.ProductDesc = b.ProductDesc;
                    obj.ProductPrice = b.ProductPrice;
                    obj.ProductStock = b.ProductStock;
                    var image = (from a in db.Image_Table where a.Productid == b.ProductId && a.ImageIsDeleted == false select a.BinaryImage).FirstOrDefault();

                    obj.BinaryImage = image;
                    if (obj.BinaryImage != null)
                    {
                        obj1.Add(obj);
                    }
                }
            }
            // ViewBag.pro = obj1;
            return View(obj1);
        }
        [HttpPost]
        public ActionResult display(string SearchKey)
        {
            List<SellerViewModel> prolist = new List<SellerViewModel>();
            Notification_Count();
            string uname = Session["user"].ToString();
            int uid = (from a in db.User_Table where a.UserName == uname select a.UserId).FirstOrDefault();
            List<SellerViewModel> obj2 = new List<SellerViewModel>();
            var search = from data in db.Product_Table
                         where SearchKey == "" ? true : data.ProductName.Contains(SearchKey)
                         select data;
            foreach (var b in search)

            {
                if (b.ProductIsDeleted == false && b.SellerId == uid)
                {





                    SellerViewModel obj = new SellerViewModel();

                    obj.ProductId = b.ProductId;
                    obj.ProductName = b.ProductName;
                    var name = (from a in db.ProductCategory_Table where a.ProductCatId == b.ProductCatid select a.ProductCatName).FirstOrDefault();
                    int id = (from c in db.ProductCategory_Table where c.ProductCatId == b.ProductCatid select c.BaseCatid).FirstOrDefault();
                    var catname = (from d in db.BaseCategory_Table where d.BaseCatId == id select d.BaseCatName).FirstOrDefault();
                    obj.BaseCatName = catname;
                    obj.ProductCatName = name;
                    obj.ProductDesc = b.ProductDesc;
                    obj.ProductPrice = b.ProductPrice;
                    obj.ProductStock = b.ProductStock;
                    var image = (from a in db.Image_Table where a.Productid == b.ProductId && a.ImageIsDeleted == false select a.BinaryImage).FirstOrDefault();

                    obj.BinaryImage = image;
                    obj2.Add(obj);
                }
            }
            if (obj2.Count > 0)
            {
                //ViewBag.pro = obj1;
                return View(obj2);
                // return Json(new { obj2 }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return View();
            }

        }


        [HttpPost]
        public ActionResult imagedisplay(int id)
        {
            Product_Table product = db.Product_Table.Find(id);
            object[] imagelist = new object[5];
            var image = (from a in db.Image_Table where a.Productid == product.ProductId && a.ImageIsDeleted == false select a).ToList();
            ViewBag.imlist = image.ToList();
            TempData["ID"] = id;



            return PartialView("imagedisplay", ViewBag.imlist);
        }
        public JsonResult DeleteImage(int id)
        {
            Image_Table image = db.Image_Table.Find(id);
            image.ImageIsDeleted = true;
            image.ImageUpdatedBy = Session["user"].ToString();
            image.ImageUpdatedDate = DateTime.Now;
            db.SaveChanges();
            bool result = true;
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult upload(Image_Table model)
        {
            if (model.BinaryImage != null)
            {
                Image_Table image = new Image_Table();
                int j;
                object[] imgarray = new object[5];
                int p = Convert.ToInt32(TempData["ID"]);
                HttpPostedFileBase file = Request.Files["ImageData"];
                for (j = 0; j < Request.Files.Count; j++)
                {
                    file = Request.Files[j];

                    ContentRepository service = new ContentRepository();
                    image = service.UploadImageInDataBase(file, model);

                    Image_Table imageObj = new Image_Table();


                    imageObj.BinaryImage = image.BinaryImage;
                    imageObj.Productid = Convert.ToInt32(TempData["ID"]);
                    imageObj.ImageCreatedBy = Session["user"].ToString();
                    imageObj.ImageCreatedDate = DateTime.Now;
                    imageObj.ImageUpdatedBy = Session["user"].ToString();
                    imageObj.ImageUpdatedDate = DateTime.Now;
                    imageObj.ImageIsDeleted = false;
                    db.Image_Table.Add(imageObj);
                    db.SaveChanges();

                }
            }
            return RedirectToAction("display");
        }

        public ActionResult Notification()
        {
            Notification_Count();
            string name = Session["user"].ToString();
            List<SellerViewModel> list = new List<SellerViewModel>();

            int id = (from a in db.User_Table where a.UserName == name select a.UserId).FirstOrDefault();
            var product = (from b in db.Product_Table where b.SellerId == id && b.ProductIsDeleted == false select b.ProductId).ToList();
            foreach (var item in product)
            {
                SellerViewModel obj = new SellerViewModel();
                var order = (from d in db.OrderDetail_Table where d.Productid == item && d.Serviceid != null select d).ToList();

                foreach (var item1 in order)
                {
                    if (item1 != null)
                    {
                        var service = (from c in db.Service_Table where c.ServiceId == item1.Serviceid && c.ServiceIsDeleted != true select c.ServiceName).FirstOrDefault();
                        obj.ServiceName = service;
                        obj.OrderId = item1.Orderid;
                        list.Add(obj);
                    }
                }



            }

            ViewBag.orderlist = list;
            return View();
        }
        public ActionResult changepassword()
        {
            Notification_Count();
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

        [HttpGet]
        public ActionResult profile()
        {
            Notification_Count();
            string name = Session["user"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
            Notification_Count();
            string name = Session["user"].ToString();
            User_Table user = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            user.FirstName = obj.FirstName;
            user.LastName = obj.LastName;
            user.UserEmail = obj.UserEmail;
            user.UserAddress = obj.UserAddress;
            user.UserUpdatedDate = System.DateTime.Now;
            db.SaveChanges();

            return View();
        }

        public void logout()
        {
            Session["user"] = null;
            Session.Abandon();
            Response.Redirect("~/User/login");
        }


    }
}