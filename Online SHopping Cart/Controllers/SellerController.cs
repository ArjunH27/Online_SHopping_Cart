using MoreLinq;
using Online_SHopping_Cart.Models;
using Online_SHopping_Cart.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;


namespace Online_SHopping_Cart.Controllers
    {
   
    public class SellerController : Controller
        {
        ShoppingCartDbEntities db = new ShoppingCartDbEntities();
        SellerViewModel svm = new SellerViewModel();
        // GET: Seller
        public static int flag = 0;

        [HttpGet]
        public ActionResult Index()
        {
           
            Notification_Count();
            Out_Of_Stock();
            Total();
           // Most_Sold_Pro();
            return View();
        }
        public static class MyChartTheme
        {
            public const string MyCustom = "<Chart BackColor=\"White\" BackGradientStyle=\"TopBottom\" BackSecondaryColor=\"White\" BorderColor=\"26, 59, 105\" BorderlineDashStyle=\"Solid\" BorderWidth=\"2\" Palette=\"BrightPastel\">\r\n    <ChartAreas>\r\n        <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"64, 165, 191, 228\" BackGradientStyle=\"TopBottom\" BackSecondaryColor=\"White\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\" /> \r\n    </ChartAreas>\r\n    <Legends>\r\n        <Legend _Template_=\"All\" BackColor=\"Transparent\" Font=\"Trebuchet MS, 8.25pt, style=Bold\" IsTextAutoFit=\"False\" /> \r\n    </Legends>\r\n    <BorderSkin SkinStyle=\"Emboss\" /> \r\n  </Chart>";
        }
        public void Most_Sold_Pro()
        {

            int count;
            List<SellerViewModel> sellerlist = new List<SellerViewModel>();
            //DateTime datetime = DateTime.Now;
            //var Date = 
            string username = Session["user"].ToString();
            int userid = db.User_Table.Where(x => x.UserName == username).Select(x => x.UserId).FirstOrDefault();



            string prevname = null;
            var productlist = (from a in db.Product_Table where a.SellerId == userid && a.ProductIsDeleted == false select a).ToList();
            var orderlist = (from o in db.Order_Table where /*o.OrderCreatedDate == Date*/  o.OrderIsDeleted == false select o).ToList();
            foreach (var product in productlist)

            {
                int id = 0;
                int? qty = 0;
                count = 0;
                foreach (var order in orderlist)
                {


                    var orders = (from or in db.OrderDetail_Table
                                  where order.OrderId == or.Orderid && or.Productid == product.ProductId
                                  select or).ToList();
                    foreach (var item in orders)
                    {
                        if (prevname == product.ProductName)
                        {
                            qty = qty + item.Quantity;

                        }
                        else
                        {

                            prevname = product.ProductName;
                            qty = item.Quantity;
                            id = product.ProductId;
                        }
                        count = 1;
                    }

                }
                if (prevname != null && count != 0)
                {
                    sellerlist.Add(new SellerViewModel
                    {
                        Quantity = qty,
                        ProductName = prevname,
                        ProductId = id
                    });
                }

            }
            var names = sellerlist.Max(x => x.Quantity);
            var name = (from a in sellerlist where a.Quantity == names select a).FirstOrDefault();
            var images = (from im in db.Image_Table where im.Productid == name.ProductId select im).FirstOrDefault();
            ViewBag.image = images;
            ViewBag.name = name;
        }
        public void Total()
        {
            List<decimal?> od = new List<decimal?>();
            decimal? ods = 0;
            string username = Session["user"].ToString();
            int userid = db.User_Table.Where(x => x.UserName == username).Select(x => x.UserId).FirstOrDefault();
            var productlist = (from a in db.Product_Table where a.SellerId == userid && a.ProductIsDeleted == false select a).ToList();
            var orderlist = (from o in db.Order_Table where o.OrderIsDeleted == false select o).ToList();
            foreach (var product in productlist)

            {


                foreach (var order in orderlist)
                {


                    OrderDetail_Table orders = (from or in db.OrderDetail_Table
                                                where order.OrderId == or.Orderid && or.Productid == product.ProductId
                                                select or).FirstOrDefault();
                    // amt = amt + orders;
                    if (orders != null)
                    {
                        od.Add(orders.Amount);
                    }
                }
            }
            ods = od.Sum();
            ViewBag.total = ods;
        }

        public void Chart(DateTime date)
        {
           
            int count;
            List<SellerViewModel> sellerlist = new List<SellerViewModel>();
            //DateTime datetime = DateTime.Now;
            var Date = date.Date;
            string username = Session["user"].ToString();
            int userid = db.User_Table.Where(x => x.UserName == username).Select(x => x.UserId).FirstOrDefault();



            string prevname = null;
            var productlist = (from a in db.Product_Table where a.SellerId == userid && a.ProductIsDeleted == false select a).ToList();
            var orderlist = (from o in db.Order_Table where o.OrderCreatedDate == Date && o.OrderIsDeleted == false && o.OrderStatus==1 select o).ToList();
            foreach (var product in productlist)

            {

                int? qty = 0;
                count = 0;
                foreach (var order in orderlist)
                {


                    var orders = (from or in db.OrderDetail_Table
                                  where order.OrderId == or.Orderid && or.Productid == product.ProductId
                                  select or).ToList();

                    foreach (var item in orders)
                    {
                        if (prevname == product.ProductName)
                        {
                            qty = qty + item.Quantity;

                        }
                        else
                        {
                            prevname = product.ProductName;
                            qty = item.Quantity;

                        }
                        count = 1;
                    }

                }
                if (prevname != null && count != 0)
                {
                    sellerlist.Add(new SellerViewModel
                    {
                        Quantity = qty,
                        ProductName = prevname
                    });
                }

            }
           //ViewBag.data = sellerlist;
            //List<SellerViewModel> sellerlist = TempData["data"] as List<SellerViewModel>;
            if (sellerlist.Count != 0)
            {
                var chart = new Chart(width: 300, height: 300, theme: MyChartTheme.MyCustom)

                    .AddSeries("Default", chartType: "doughnut",
                    xValue: sellerlist, xField: "ProductName",
                    yValues: sellerlist, yFields: "Quantity")
                    .Write("png");
            }
            
            //return Json(new { sellerlist }, JsonRequestBehavior.AllowGet);
        }


        public void Out_Of_Stock()
        {
            string username = Session["user"].ToString();
            int userid = db.User_Table.Where(x => x.UserName == username).Select(x => x.UserId).FirstOrDefault();

            var productlist = (from a in db.Product_Table where a.SellerId == userid && a.ProductIsDeleted == false && a.ProductStock == 0 select a).ToList();
            ViewBag.outofstock = productlist;
        }





        public void Notification_Count()
        {
            List<string> productnamelist = new List<string>();
            List<string> productnamelists = new List<string>();
            List<int?> countlist = new List<int?>();
            //Getting the user id
            int count = 0;

            string username = Session["user"].ToString();
            int userid = db.User_Table.Where(x => x.UserName == username).Select(x => x.UserId).FirstOrDefault();

            //Getting all the product and order details details of the user

            string prevname = null;
            var productlist = (from a in db.Product_Table where a.SellerId == userid && a.ProductIsDeleted == false select a).ToList();
            var orderlist = (from o in db.Order_Table
                             where o.OrderStatus == 1 && o.OrderIsDeleted == false && o.OrderNotification == "00" || o.OrderNotification == "01"
                             select o).DistinctBy(x => x.OrderId).ToList();
            foreach (var product in productlist)

            {

                int? qty = 0;
                count = 0;
                foreach (var order in orderlist)
                {


                    var orders = (from or in db.OrderDetail_Table
                                  where order.OrderId == or.Orderid && or.Productid == product.ProductId
                                  select or).ToList();

                    foreach (var item in orders)
                    {
                        if (prevname == product.ProductName)
                        {
                            qty = qty + item.Quantity;

                        }
                        else
                        {
                            prevname = product.ProductName;
                            qty = item.Quantity;

                        }
                        count = 1;
                    }

                }
                if (prevname != null && count != 0)
                {
                    productnamelist.Add(prevname);
                    countlist.Add(qty);
                }

            }

            var g = productnamelist.GroupBy(i => i);
            foreach (var grp in g)
            {
                //  countlist.Add(grp.Count());
                productnamelists.Add(grp.Key);

            }
            var counts = countlist.Zip(productnamelists, (first, second) => first + " " + second);


            ViewBag.sellernotification = counts;
            Session["notif-count"] = productnamelists.Count();
        }




        public void DisableNotification()
        {
            Order_Table obj = new Order_Table();
            var order = (from a in db.Order_Table where a.OrderIsDeleted==false select a).ToList();
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

            ViewBag.null_image = TempData["null_image"];
            ViewBag.not_image = TempData["not_image"];
            Notification_Count();
            List<BaseCategory_Table> category = new List<BaseCategory_Table>();
            category = db.BaseCategory_Table.Where(x => x.BaseCatIsDeleted == false).ToList();
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
                List<ProductCategory_Table> productCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == BaseCatId && x.ProductCatIsDeleted == false).ToList();
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
                    if (file.FileName != "")
                    {
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
                    else if(file.FileName== "")
                    {
                        TempData["null_image"] = "Cannot Upload Null Image";
                        return RedirectToAction("Create");
                    }
                    else
                    {
                        TempData["not_image"] = "this is not an image file";
                        return RedirectToAction("Create");
                    }
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
            ViewBag.null_imagedisp = TempData["null_imagedisp"];
            ViewBag.not_imagedisp = TempData["not_imagedisp"];
            ViewBag.not_product = TempData["not_product"];
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
               
                return View(obj2);
       
            }
            else
            {
                TempData["not_product"] = "not a product name";
                return RedirectToAction("display");
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
          
                Image_Table image = new Image_Table();
                int j;
                object[] imgarray = new object[5];
                int p = Convert.ToInt32(TempData["ID"]);
                HttpPostedFileBase file = Request.Files["ImageData"];
                for (j = 0; j < Request.Files.Count; j++)
                {
                    file = Request.Files[j];

                    ContentRepository service = new ContentRepository();

                if (file.ContentType.ToLower() != "image/jpg" &&
                    file.ContentType.ToLower() != "image/jpeg" &&
                    file.ContentType.ToLower() != "image/pjpeg" &&
                    file.ContentType.ToLower() != "image/gif" &&
                    file.ContentType.ToLower() != "image/x-png" &&
                    file.ContentType.ToLower() != "image/png")
                {
                    TempData["not_imagedisp"] = "this is not an image file";
                    return RedirectToAction("display");
                }





                    else if (file.FileName != "")
                    {
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
                    else if (file.FileName == "")
                    {
                        TempData["null_imagedisp"] = "Cannot Upload Null Image";
                        return RedirectToAction("display");
                    }
                  
                }
            
            return RedirectToAction("display");
        }

        public ActionResult Notification()
        {
            Notification_Count();

            List<SellerViewModel> list = new List<SellerViewModel>();
            string name = Session["user"].ToString();
            int id = (from a in db.User_Table where a.UserName == name select a.UserId).FirstOrDefault();
            var product = (from b in db.Product_Table where b.SellerId == id && b.ProductIsDeleted == false select b).ToList();
            foreach (var item in product)
            {
                var order = (from d in db.OrderDetail_Table where d.Productid == item.ProductId select d).ToList();

                foreach (var item1 in order)
                {
                    if (item1 != null)
                    {
                        var userid = (from e in db.Order_Table where e.OrderId == item1.Orderid && e.OrderStatus== 1 select e.Userid).FirstOrDefault();
                        var servicepid = (from c in db.Service_Table where c.ServiceId == item1.Serviceid && c.ServiceIsDeleted != true select c.ServiceProviderid).FirstOrDefault();
                        var servicename = (from d in db.User_Table where d.UserId == servicepid select d.UserName).FirstOrDefault();
                        var username = (from f in db.User_Table where f.UserId == userid select f.UserName).FirstOrDefault();
                        list.Add(new SellerViewModel
                        {
                            ServiceName = servicename,
                            ProductName = item.ProductName,
                            UserName = username,

                        });

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
            ViewBag.fill_msg = TempData["fill_msg"];
            Notification_Count();
                string name = Session["user"].ToString();
                User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
                return View(obj);
            }

        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
            Notification_Count();
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

        public void logout()
            {
                Session["user"] = null;
                Session.Abandon();
                Response.Redirect("~/User/login");
            }


        }
    }