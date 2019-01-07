using magazinOnline.Models.Data;
using magazinOnline.Models.ViewModels.Shop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    
    }
}