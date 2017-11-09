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

        [HttpGet]
        public ActionResult Index()
        {
            Notification_Count();
            Out_Of_Stock();
            return View();
        }


        #region Create Chart
        /// <summary>
        /// To display a chart with the product details
        /// </summary>
        /// <param name="date"></param>
        public void Chart(DateTime date)
        {
          
                bool flag;
                var Date = date.Date;
                string userName = Session["Seller"].ToString();
                string productName = null;

                List<SellerViewModel> sellerList = new List<SellerViewModel>();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();
                var productList = (from a in db.Product_Table where a.SellerId == userId && a.ProductIsDeleted == false select a).ToList();
                var orderList = (from o in db.Order_Table where o.OrderCreatedDate == Date && o.OrderIsDeleted == false && o.OrderStatus == 1 select o).ToList();
                // Get the details of all products and adding to list
                foreach (var product in productList)
                {
                    int? quantity = 0;
                    flag = false;
                    foreach (var order in orderList)
                    {
                        var orders = (from or in db.OrderDetail_Table
                                      where order.OrderId == or.Orderid && or.Productid == product.ProductId
                                      select or).ToList();

                        foreach (var item in orders)
                        {
                            if (productName == product.ProductName)
                            {
                                quantity = quantity + item.Quantity;

                            }
                            else
                            {
                                productName = product.ProductName;
                                quantity = item.Quantity;

                            }
                            flag = true;
                        }

                    }
                    if (productName != null && flag != false)
                    {
                        sellerList.Add(new SellerViewModel
                        {
                            Quantity = quantity,
                            ProductName = productName
                        });
                    }

                }
                //Prepare a chart
                if (sellerList.Count != 0)
                {
                    var chart = new Chart(width: 300, height: 300, theme: MyChartTheme.MyCustom)

                        .AddSeries("Default", chartType: "doughnut",
                        xValue: sellerList, xField: "ProductName",
                        yValues: sellerList, yFields: "Quantity")
                        .Write("png");
                }
           
        }
        #endregion

        #region Compute Out Of Stock
        /// <summary>
        /// Get the notification for out of stock products
        /// </summary>
        public void Out_Of_Stock()
        {
          
                string userName = Session["Seller"].ToString();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();
                var productlist = (from a in db.Product_Table
                                   where a.SellerId == userId
                                   && a.ProductIsDeleted == false
                                   && a.ProductStock == 0
                                   select a).ToList();
                ViewBag.outofstock = productlist;
           
        }
        #endregion

        #region Notification Count

        /// <summary>
        /// Get the notification of sold product and their quantity 
        /// </summary>
        public void Notification_Count()
        {
            
                bool flag;
                string productName = null;

                List<string> productNameList = new List<string>();
                List<string> productNameLists = new List<string>();
                List<int?> countList = new List<int?>();

                string userName = Session["Seller"].ToString();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();

                //Getting all the product and order details details of the user
                var productList = (from a in db.Product_Table
                                   where a.SellerId == userId
                                   && a.ProductIsDeleted == false
                                   select a).ToList();

                var orderList = (from o in db.Order_Table
                                 where o.OrderStatus == 1
                                 && o.OrderIsDeleted == false
                                 && o.OrderNotification == "00" || o.OrderNotification == "01"
                                 select o).DistinctBy(x => x.OrderId).ToList();

                foreach (var product in productList)
                {
                    int? quantity = 0;
                    flag = false;
                    foreach (var order in orderList)
                    {
                        var orders = (from or in db.OrderDetail_Table
                                      where order.OrderId == or.Orderid
                                      && or.Productid == product.ProductId
                                      select or).ToList();

                        foreach (var item in orders)
                        {
                            if (productName == product.ProductName)
                            {
                                quantity = quantity + item.Quantity;
                            }
                            else
                            {
                                productName = product.ProductName;
                                quantity = item.Quantity;
                            }
                            flag = true;
                        }
                    }
                    if (productName != null && flag != false)
                    {
                        productNameList.Add(productName);
                        countList.Add(quantity);
                    }

                }

                var g = productNameList.GroupBy(i => i);
                foreach (var grp in g)
                {
                    productNameLists.Add(grp.Key);
                }
                //combing the list
                var counts = countList.Zip(productNameLists, (first, second) => first + " " + second);
                ViewBag.sellerNotification = counts;
                Session["notif-count"] = productNameLists.Count();
          
        }

        #endregion

        #region Disable Notification
        /// <summary>
        /// Disable the notification  after viewing
        /// </summary>
        public void Disable_Notification()
        {
           
                Order_Table orderObj = new Order_Table();
                var orderList = (from a in db.Order_Table where a.OrderIsDeleted == false select a).ToList();
                foreach (var order in orderList)
                {
                    if (order.OrderNotification == "00")
                    {
                        order.OrderNotification = "10";
                    }
                    else if (order.OrderNotification == "01")
                    {
                        order.OrderNotification = "11";
                    }
                }
                db.SaveChanges();
            
            
        }
        #endregion

        #region Create a product
        /// <summary>
        /// Load View for Creating a product
        /// </summary>
        /// <returns>returns the view of create</returns>

        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                Notification_Count();

                ViewBag.null_image = TempData["null_image"];
                ViewBag.not_image = TempData["not_image"];

                List<BaseCategory_Table> categoryList = new List<BaseCategory_Table>();
                categoryList = db.BaseCategory_Table.Where(x => x.BaseCatIsDeleted == false).ToList();
                var productList = new List<SelectListItem>();
                foreach (var item in categoryList)
                {
                    productList.Add(new SelectListItem
                    {
                        Text = item.BaseCatName.ToString(),
                        Value = item.BaseCatId.ToString(),

                    });
                    ViewBag.basecategory = productList;
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }

        #endregion

        #region create a drop down list
        /// <summary>
        /// Getting the details of product category for the dropdown
        /// </summary>
        /// <param name="id"></param>
        /// <returns>returns to ajax</returns>
        public ActionResult GetProductCat(string id)
        {
            int BaseCatId;
            List<SelectListItem> ProductCategoryList = new List<SelectListItem>();
            if (!string.IsNullOrEmpty(id))
            {
                BaseCatId = Convert.ToInt32(id);
                List<ProductCategory_Table> productCategory = db.ProductCategory_Table.Where(x => x.BaseCatid == BaseCatId && x.ProductCatIsDeleted == false).ToList();
                foreach (var item in productCategory)
                {
                    ProductCategoryList.Add(new SelectListItem
                    {
                        Text = item.ProductCatName.ToString(),
                        Value = item.ProductCatId.ToString(),

                    });
                }

                ViewBag.procat = ProductCategoryList;
            }
            return Json(ProductCategoryList, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region Create a Product
        /// <summary>
        /// Adding the details to db 
        /// </summary>
        [HttpPost]
        public ActionResult Create(Image_Table model, Product_Table product)
        {
            try
            {
                int j;
                Image_Table image = new Image_Table();

                Notification_Count();
                if (ModelState.IsValid)
                {
                    //Add the details to the product table
                    string userName = Session["Seller"].ToString();
                    int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();
                    product.SellerId = userId;
                    product.ProductCreatedBy = Session["Seller"].ToString();
                    product.ProductCreatedDate = DateTime.Now;
                    product.ProductUpdatedBy = Session["Seller"].ToString();
                    product.ProductUpdatedDate = DateTime.Now;
                    product.ProductIsDeleted = false;
                    db.Product_Table.Add(product);
                    db.SaveChanges();

                    //Adding the respective images of the products to the Image table 
                    //checking if the uploaded file is a image file 
                    object[] imgarray = new object[5];
                    int productId = product.ProductId;
                    HttpPostedFileBase file = Request.Files["ImageData"];
                    for (j = 0; j < Request.Files.Count; j++)
                    {
                        file = Request.Files[j];

                        ContentRepository service = new ContentRepository();
                        if (file.FileName != "")
                        {
                            //passing the details to Upload image in Content Repository class
                            image = service.UploadImageInDataBase(file, model);

                            Image_Table imageObj = new Image_Table();
                            imageObj.BinaryImage = image.BinaryImage;
                            imageObj.Productid = product.ProductId;
                            imageObj.ImageCreatedBy = Session["Seller"].ToString();
                            imageObj.ImageCreatedDate = DateTime.Now;
                            imageObj.ImageUpdatedBy = Session["Seller"].ToString();
                            imageObj.ImageUpdatedDate = DateTime.Now;
                            imageObj.ImageIsDeleted = false;
                            db.Image_Table.Add(imageObj);
                            db.SaveChanges();
                        }
                        else if (file.FileName == "")
                        {
                            TempData["null_image"] = "Cannot Upload Null Image";
                            return RedirectToAction("Create");
                        }
                        else
                        {
                            TempData["not_image"] = "This is not an image file";
                            return RedirectToAction("Create");
                        }
                    }
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return RedirectToAction("display");

        }
        #endregion

        #region Edit on Grid
        /// <summary>
        /// Updating the data to the table when an edit operation is done on the grid
        /// </summary>
        [HttpPost]
        public JsonResult Edit(int id, string name, decimal price, string features, int stock)
        {
            Product_Table product = db.Product_Table.Find(id);
            product.ProductName = name;
            product.ProductPrice = price;
            product.ProductDesc = features;
            product.ProductStock = stock;
            product.ProductUpdatedBy = Session["Seller"].ToString();
            product.ProductUpdatedDate = DateTime.Now;
            db.SaveChanges();
            return Json(new { id = id, ProductName = name, ProductPrice = price, ProductDesc = features, ProductStock = stock }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Delete on Grid
        /// <summary>
        /// Deleting the record in the table 
        /// </summary>
        [HttpPost, ActionName("Delete")]
        public JsonResult DeleteConfirmed(int id)
        {
            bool result = true;

            Product_Table product = db.Product_Table.Find(id);
            product.ProductUpdatedBy = Session["Seller"].ToString();
            product.ProductUpdatedDate = DateTime.Now;
            product.ProductIsDeleted = true;
            var imageList = (from a in db.Image_Table where a.Productid == product.ProductId select a).ToList();

            foreach (var image in imageList)
            {
                image.ImageUpdatedBy = Session["Seller"].ToString();
                image.ImageUpdatedDate = DateTime.Now;
                image.ImageIsDeleted = true;
            }

            db.SaveChanges();
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Display the details of product 
        /// <summary>
        /// Display all the details from the product table
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult display()
        {
            try
            {
                Notification_Count();
                ViewBag.null_imagedisp = TempData["null_imagedisp"];
                ViewBag.not_imagedisp = TempData["not_imagedisp"];
                ViewBag.not_product = TempData["not_product"];

                List<SellerViewModel> sellerList = new List<SellerViewModel>();
                string userName = Session["Seller"].ToString();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();
                var productList = (from a in db.Product_Table
                                   where a.ProductIsDeleted == false
                                   && a.SellerId == userId
                                   select a).ToList();
                foreach (var product in productList)
                {
                    SellerViewModel sellerObj = new SellerViewModel();
                    sellerObj.ProductId = product.ProductId;
                    sellerObj.ProductName = product.ProductName;

                    var productCatName = (from a in db.ProductCategory_Table
                                          where a.ProductCatId == product.ProductCatid
                                          select a.ProductCatName).FirstOrDefault();

                    int baseCatId = (from b in db.ProductCategory_Table
                                     where b.ProductCatId == product.ProductCatid
                                     select b.BaseCatid).FirstOrDefault();

                    var catname = (from c in db.BaseCategory_Table
                                   where c.BaseCatId == baseCatId
                                   select c.BaseCatName).FirstOrDefault();

                    sellerObj.BaseCatName = catname;
                    sellerObj.ProductCatName = catname;
                    sellerObj.ProductDesc = product.ProductDesc;
                    sellerObj.ProductPrice = product.ProductPrice;
                    sellerObj.ProductStock = product.ProductStock;
                    //Adding the image to the list to display
                    var image = (from a in db.Image_Table
                                 where a.Productid == product.ProductId
                                 && a.ImageIsDeleted == false
                                 select a.BinaryImage).FirstOrDefault();

                    sellerObj.BinaryImage = image;
                    if (sellerObj.BinaryImage != null)
                    {
                        sellerList.Add(sellerObj);
                    }
                }
                return View(sellerList);
            }
            catch (Exception)
            {
               return View("Error");
            }
            return View();
        }
        #endregion

        #region Details of searched products
        /// <summary>
        /// Displaying the details to those products which where searched
        /// </summary>
        [HttpPost]
        public ActionResult display(string SearchKey)
        {
            try
            {
                List<SellerViewModel> SellerList = new List<SellerViewModel>();
                string userName = Session["Seller"].ToString();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();

                Notification_Count();
                //Comparing the product name with each name in the table
                var search = (from data in db.Product_Table
                              where SearchKey == "" ? true : data.ProductName.Contains(SearchKey)
                              && data.ProductIsDeleted == false && data.SellerId == userId
                              select data);
                foreach (var searchItem in search)
                {
                    SellerViewModel searchObj = new SellerViewModel();
                    searchObj.ProductId = searchItem.ProductId;
                    searchObj.ProductName = searchItem.ProductName;

                    var productCatName = (from a in db.ProductCategory_Table
                                          where a.ProductCatId == searchItem.ProductCatid
                                          select a.ProductCatName).FirstOrDefault();

                    int baseCatId = (from c in db.ProductCategory_Table
                                     where c.ProductCatId == searchItem.ProductCatid
                                     select c.BaseCatid).FirstOrDefault();

                    var baseCatName = (from d in db.BaseCategory_Table
                                       where d.BaseCatId == baseCatId
                                       select d.BaseCatName).FirstOrDefault();

                    searchObj.BaseCatName = baseCatName;
                    searchObj.ProductCatName = productCatName;
                    searchObj.ProductDesc = searchItem.ProductDesc;
                    searchObj.ProductPrice = searchItem.ProductPrice;
                    searchObj.ProductStock = searchItem.ProductStock;
                    var image = (from a in db.Image_Table
                                 where a.Productid == searchItem.ProductId
                                 && a.ImageIsDeleted == false
                                 select a.BinaryImage).FirstOrDefault();
                    if (image != null)
                    {
                        searchObj.BinaryImage = image;
                        SellerList.Add(searchObj);
                    }
                }
                if (SellerList.Count > 0)
                {
                    return View(SellerList);
                }
                else
                {
                    TempData["not_product"] = "not a product name";
                    return RedirectToAction("display");
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        #endregion

        #region Display the image on popup
        /// <summary>
        /// to display an image on a popup
        /// </summary>
        [HttpPost]
        public ActionResult imagedisplay(int id)
        {
            try
            {
                Product_Table product = db.Product_Table.Find(id);

                var image = (from a in db.Image_Table
                             where a.Productid == product.ProductId
                             && a.ImageIsDeleted == false
                             select a).ToList();

                ViewBag.imlist = image.ToList();
                TempData["ID"] = id;
                return PartialView("imagedisplay", ViewBag.imlist);
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        #endregion

        #region Delete image
        /// <summary>
        /// delete an image from the list of images in the pop up
        /// </summary>
        public JsonResult DeleteImage(int id)
        {
            Image_Table image = db.Image_Table.Find(id);
            image.ImageIsDeleted = true;
            image.ImageUpdatedBy = Session["Seller"].ToString();
            image.ImageUpdatedDate = DateTime.Now;
            db.SaveChanges();
            bool result = true;
            return Json(new { result }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region upload image
        /// <summary>
        /// Upload an image from the popup and save it to db
        /// </summary>
        [HttpPost]
        public ActionResult upload(Image_Table model)
        {
            try
            {
                int j;
                Image_Table image = new Image_Table();
                int ProductId = Convert.ToInt32(TempData["ID"]);
                HttpPostedFileBase file = Request.Files["ImageData"];
                //Checking if the file is an image
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
                        imageObj.ImageCreatedBy = Session["Seller"].ToString();
                        imageObj.ImageCreatedDate = DateTime.Now;
                        imageObj.ImageUpdatedBy = Session["Seller"].ToString();
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
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        #endregion

        #region Older Notification
        /// <summary>
        /// Displaying all the details of the older notification of the product
        /// </summary>
        /// <returns></returns>
        public ActionResult Notification()
        {
            try
            {
                Notification_Count();
                List<SellerViewModel> sellerList = new List<SellerViewModel>();
                string userName = Session["Seller"].ToString();
                int userId = db.User_Table.Where(x => x.UserName == userName).Select(x => x.UserId).FirstOrDefault();

                var productList = (from b in db.Product_Table
                                   where b.SellerId == userId
                                   && b.ProductIsDeleted == false
                                   select b).ToList();

                foreach (var product in productList)
                {
                    var orderList = (from d in db.OrderDetail_Table
                                     where d.Productid == product.ProductId
                                     && d.Serviceid!=null
                                     select d).ToList();

                    foreach (var order in orderList)
                    {
                        if (order != null)
                        {
                            var userid = (from e in db.Order_Table
                                          where e.OrderId == order.Orderid
                                          && e.OrderStatus == 1
                                          && e.OrderIsDeleted== false
                                          select e.Userid).FirstOrDefault();

                            var serviceProductId = (from c in db.Service_Table
                                                    where c.ServiceId == order.Serviceid
                                                    && c.ServiceIsDeleted != true
                                                    select c.ServiceProviderid).FirstOrDefault();

                            var serviceName = (from d in db.User_Table
                                               where d.UserId == serviceProductId
                                               select d.UserName).FirstOrDefault();

                            var username = (from f in db.User_Table
                                            where f.UserId == userid
                                            select f.UserName).FirstOrDefault();
                            sellerList.Add(new SellerViewModel
                            {
                                ServiceName = serviceName,
                                ProductName = product.ProductName,
                                UserName = username,
                            });
                        }
                    }
                }
                ViewBag.orderlist = sellerList;
            }
            catch (Exception)
            {
                return View("Error");
            }
            return View();
        }
        #endregion

        #region Change Password
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
            string name = Session["Seller"].ToString();
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
        #endregion

        #region Edit Profile
        [HttpGet]
        public ActionResult profile()
        {
            Notification_Count();
            ViewBag.fill_msg = TempData["fill_msg"];
            Notification_Count();
            string name = Session["Seller"].ToString();
            User_Table obj = db.User_Table.Where(x => x.UserName == name).FirstOrDefault();
            return View(obj);
        }
       
        [HttpPost]
        public ActionResult profile(User_Table obj)
        {
            Notification_Count();
            if (obj.FirstName != null && obj.LastName != null && obj.UserEmail != null && obj.UserAddress != null && obj.UserPhno != null)
            {
                string name = Session["Seller"].ToString();
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
        #endregion

        #region Logout
        public void logout()
        {
            Session["Seller"] = null;
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

        #region Theme for Chart
        public static class MyChartTheme
        {
            public const string MyCustom = "<Chart BackColor=\"White\" BackGradientStyle=\"TopBottom\" BackSecondaryColor=\"White\" BorderColor=\"26, 59, 105\" BorderlineDashStyle=\"Solid\" BorderWidth=\"2\" Palette=\"BrightPastel\">\r\n    <ChartAreas>\r\n        <ChartArea Name=\"Default\" _Template_=\"All\" BackColor=\"64, 165, 191, 228\" BackGradientStyle=\"TopBottom\" BackSecondaryColor=\"White\" BorderColor=\"64, 64, 64, 64\" BorderDashStyle=\"Solid\" ShadowColor=\"Transparent\" /> \r\n    </ChartAreas>\r\n    <Legends>\r\n        <Legend _Template_=\"All\" BackColor=\"Transparent\" Font=\"Trebuchet MS, 8.25pt, style=Bold\" IsTextAutoFit=\"False\" /> \r\n    </Legends>\r\n    <BorderSkin SkinStyle=\"Emboss\" /> \r\n  </Chart>";
        }
        #endregion
    }
}