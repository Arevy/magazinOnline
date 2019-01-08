using magazinOnline.Models.Data;
using magazinOnline.Models.ViewModels.Shop;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI;

namespace magazinOnline.Areas.Admin.Controllers
{
    public class ShopController : Controller
    {
        // GET: Admin/Shop/Categories
        public ActionResult Categories()
        {
            //Declarare lista modele
            List<CategoryVM> categoryVMList;

            using (Db db = new Db())
            {
                //Init lista
                categoryVMList = db.Categories.ToArray().OrderBy(x => x.Sorting).Select(x => new CategoryVM(x)).ToList();
            }

            return View(categoryVMList);
        }
        // POST: admin/shop/AddNewCategory
        [HttpPost]
        public string AddNewCategory(string catName)
        {
            //Declar ID
            string id;

            using (Db db = new Db())
            {
                //Verific daca numele categoriei este unic
                if (db.Categories.Any(x => x.Name == catName))
                {
                    return "titletaken";
                }

                //Init DTO
                CategoryDTO dto = new CategoryDTO();

                //Adaugare la DTO
                dto.Name = catName;
                dto.Slug = catName.Replace(" ", "-").ToLower();
                dto.Sorting = 100;

                //Salveaza DTO
                db.Categories.Add(dto);
                db.SaveChanges();

                //ID
                id = dto.Id.ToString();
            }
            //Return ID
            return id;

        }

        //POST: Admin/Shop/ReorderCategories
        [HttpPost]
        public void ReorderCategories(int[] id)
        {
            using (Db db = new Db())
            {
                //Setez ordinea initiala
                int count = 1;

                //Declarare CategoryDTO
                CategoryDTO dto;

                //Setare pentru sortarea fiecarei categorii
                foreach (var catId in id)
                {
                    dto = db.Categories.Find(catId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }

        }
        // GET: Admin/Shop/DeleteCategory/id
        public ActionResult DeleteCategory(int id)
        {
            using (Db db = new Db())
            {
                //Cauta categorie
                CategoryDTO dto = db.Categories.Find(id);

                //Stergere categorie
                db.Categories.Remove(dto);

                //Salvare
                db.SaveChanges();
            }
            //Redirectionare
            return RedirectToAction("Categories");
        }

        //POST: admin/shop/RenameCategory
        [HttpPost]
        public string RenameCategory(string newCatName, int id)
        {

            using (Db db = new Db())
            {
                //Verifica daca numele cateogriei este unic
                if (db.Categories.Any(x => x.Name == newCatName))
                    return "titletaken";
                //Get DTO
                CategoryDTO dto = db.Categories.Find(id);

                //Editeaza DTO
                dto.Name = newCatName;
                dto.Slug = newCatName.Replace(" ", "-").ToLower();

                //Salveaza
                db.SaveChanges();
            }
            //Returneaza
            return "ok";
        }

        //GET: admin/shop/product
        public ActionResult AddProduct()
        {
            //Init
            ProductVM model = new ProductVM();
            //Adaugare lista de categorii la model
            using (Db db = new Db())
            {
                model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

            }
            //return model
            return View(model);
        }

        //POST: admin/shop/addproduct
        [HttpPost]
        public ActionResult AddProduct(ProductVM model, HttpPostedFileBase file)
        {
            if(!ModelState.IsValid)
            {
                using (Db db = new Db())
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    return View(model);
                }
            }
            //Verifica daca numele produsului este unic
            using (Db db = new Db())
            {
                if (db.Products.Any(x => x.Name == model.Name))
                {
                    model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                    ModelState.AddModelError(" ", "Numele produsul este luat!");
                    return View(model);
                }
            }
            //Declara id produs
            int id;

            //Init si salvare produsDTO
            using (Db db = new Db())
            {
                ProductDTO product = new ProductDTO();

                product.Name = model.Name;
                product.Slug = model.Name.Replace(" ", "-").ToLower();
                product.Description = model.Description;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;

                CategoryDTO catDTO = db.Categories.FirstOrDefault(x => x.Id == model.CategoryId);
                product.CategoryName = catDTO.Name;

                db.Products.Add(product);
                db.SaveChanges();

                //Get id
                id = product.Id;

            }

            //Set tempdata message
            TempData["SM"] = "Ai adaugat un produs!";

            #region Upload Image

            // Create necessary directories
            var originalDirectory = new DirectoryInfo(string.Format("{0}Images\\Uploads", Server.MapPath(@"\")));

            var pathString1 = Path.Combine(originalDirectory.ToString(), "Products");
            var pathString2 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString());
            var pathString3 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Thumbs");
            var pathString4 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery");
            var pathString5 = Path.Combine(originalDirectory.ToString(), "Products\\" + id.ToString() + "\\Gallery\\Thumbs");

            if (!Directory.Exists(pathString1))
                Directory.CreateDirectory(pathString1);

            if (!Directory.Exists(pathString2))
                Directory.CreateDirectory(pathString2);

            if (!Directory.Exists(pathString3))
                Directory.CreateDirectory(pathString3);

            if (!Directory.Exists(pathString4))
                Directory.CreateDirectory(pathString4);

            if (!Directory.Exists(pathString5))
                Directory.CreateDirectory(pathString5);

            // Check if a file was uploaded
            if (file != null && file.ContentLength > 0)
            {
                // Get file extension
                string ext = file.ContentType.ToLower();

                // Verify extension
                if (ext != "image/jpg" &&
                    ext != "image/jpeg" &&
                    ext != "image/pjpeg" &&
                    ext != "image/gif" &&
                    ext != "image/x-png" &&
                    ext != "image/png")
                {
                    using (Db db = new Db())
                    {
                        model.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");
                        ModelState.AddModelError("", "Nu am putut urca imaginea - extensie gresita.");
                        return View(model);
                    }
                }

                // Init image name
                string imageName = file.FileName;

                // Save image name to DTO
                using (Db db = new Db())
                {
                    ProductDTO dto = db.Products.Find(id);
                    dto.ImageName = imageName;

                    db.SaveChanges();
                }

                // Set original and thumb image paths
                var path = string.Format("{0}\\{1}", pathString2, imageName);
                var path2 = string.Format("{0}\\{1}", pathString3, imageName);

                // Save original
                file.SaveAs(path);

                // Create and save thumb
                WebImage img = new WebImage(file.InputStream);
                img.Resize(200, 200);
                img.Save(path2);
            }

            #endregion
            //Redirect
            return RedirectToAction("AddProduct");
        }

        //GET: admin/shop/products
        public ActionResult Products(int? page, int? catId)
        {
            // Declare a list of ProductVM
            List<ProductVM> listOfProductVM;

            // Set page number
            var pageNumber = page ?? 1;

            using (Db db = new Db())
            {
                // Init the list
                listOfProductVM = db.Products.ToArray()
                                  .Where(x => catId == null || catId == 0 || x.CategoryId == catId)
                                  .Select(x => new ProductVM(x))
                                  .ToList();

                // Populate categories select list
                ViewBag.Categories = new SelectList(db.Categories.ToList(), "Id", "Name");

                // Set selected category
                ViewBag.SelectedCat = catId.ToString();
            }

            // Set pagination
            var onePageOfProducts = listOfProductVM.ToPagedList(pageNumber, 3);
            ViewBag.OnePageOfProducts = onePageOfProducts;

            // Return view with list
            return View(listOfProductVM);
        }
    }
}