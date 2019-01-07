using magazinOnline.Models.Data;
using magazinOnline.Models.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace magazinOnline.Areas.Admin.Controllers
{
    public class PagesController : Controller
    {
/*#pragma warning disable IDE0044 // Add readonly modifier
        private object[] id;
#pragma warning restore IDE0044 // Add readonly modifier */

        // GET: Admin/Pages
        public ActionResult Index()
        {
            //Lista de PageVM
            List<PageVM> pagesList;
            using (Db db = new Db())
            {
                //Initializarea listei
                pagesList = db.Pages.ToArray().OrderBy(x => x.Sorting).Select(x => new PageVM(x)).ToList();

            }
            //returneaza View-ul
            return View(pagesList);
        }
        // GET: Admin/Pages/AddPage
        [HttpGet]
        public ActionResult AddPage()
        {
            return View();
        }
        // POST: Admin/Pages/AddPage
        [HttpPost]
        public ActionResult AddPage(PageVM model)
        {
            // Verifica starea modelului
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            using (Db db = new Db())
            {
                //Declarare slug
                string slug;
                //Init PageDTO
                PageDTO dto = new PageDTO();
                //DTO Title
                dto.Title = model.Title;
                // Seteaza Slug
                if (string.IsNullOrWhiteSpace(model.Slug))
                {
                    slug = model.Title.Replace(" ", "-").ToLower();
                }
                else
                {
                    slug = model.Slug.Replace(" ", "-").ToLower();
                }
                //Titlul si slug sunt unice
                if (db.Pages.Any(x => x.Title == model.Title) || db.Pages.Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Acest titlu deja exista.");
                    return View(model);
                }
                //DTO rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;
                dto.Sorting = 100;
                // Seteaza data
                db.Pages.Add(dto);
                db.SaveChanges();
            }
            //Rederict 
            TempData["SM"] = "Ai adaugat o pagina noua!";
            return View();
        }

        // GET: Admin/Pages/EditPage/id
        [HttpGet]
        public ActionResult EditPage(int id)
        {
            // Declarare pageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get page
                PageDTO dto = db.Pages.Find(id);

                // Confirmare pagina existenta
                if (dto == null)
                {
                    return Content("Pagina nu exista.");
                }
                // Initializare pageVM
                model = new PageVM(dto);
            }

            // Returnare view model
            return View(model);
        }

        // POST: Admin/Pages/EditPage/id
        [HttpPost]
        public ActionResult EditPage(PageVM model)
        {
            // Check model state
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (Db db = new Db())
            {
                // Get page id
                int id = model.Id;

                // Init slug
                string slug = "home";

                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // DTO titlu
                dto.Title = model.Title;

                // Check for slug and set it if need be
                if (model.Slug != "home")
                {
                    if (string.IsNullOrWhiteSpace(model.Slug))
                    {
                        slug = model.Title.Replace(" ", "-").ToLower();
                    }
                    else
                    {
                        slug = model.Slug.Replace(" ", "-").ToLower();
                    }
                }

                // verifica daca e unic
                if (db.Pages.Where(x => x.Id != id).Any(x => x.Title == model.Title) ||
                     db.Pages.Where(x => x.Id != id).Any(x => x.Slug == slug))
                {
                    ModelState.AddModelError("", "Acest titlu deja exista.");
                    return View(model);
                }

                // DTO rest
                dto.Slug = slug;
                dto.Body = model.Body;
                dto.HasSidebar = model.HasSidebar;

                // Save  DTO
                db.SaveChanges();
            }

            // Set TempData message
            TempData["SM"] = "Ai editat pagina!";

            // Redirect
            return RedirectToAction("EditPage");
        }

        // GET: Admin/Pages/PageDetails/id
        public ActionResult PageDetails(int id)
        {
            // Declare PageVM
            PageVM model;

            using (Db db = new Db())
            {
                // Get the page
                PageDTO dto = db.Pages.Find(id);

                // Confirm page exists
                if (dto == null)
                {
                    return Content("Pagina nu exista!");
                }

                // Init PageVM
                model = new PageVM(dto);
            }

            // Return view with model
            return View(model);
        }

        // GET: Admin/Pages/DeletePage/id
        public ActionResult DeletePage(int id)
        {
            using (Db db = new Db())
            {
                //Cauta pagina
                PageDTO dto = db.Pages.Find(id);

                //Stergere pagina
                db.Pages.Remove(dto);

                //Salvare
                db.SaveChanges();
            }
            //Redirectionare
            return RedirectToAction("Index");
        }

        //POST: Admin/Pages/ReorderPages
        [HttpPost]
        public void ReorderPages(int[] id)
        {
            using (Db db = new Db())
            {
                //Setez ordinea initiala
                int count = 1;

                //Declarare PageDTO
                PageDTO dto;

                //Setare pentru sortarea fiecarei pagini
                foreach (var pageId in id)
                {
                    dto = db.Pages.Find(pageId);
                    dto.Sorting = count;

                    db.SaveChanges();
                    count++;
                }
            }

        }

        // GET: Admin/Pages/EditSidebar/id
        [HttpGet]
        public ActionResult EditSidebar()
        {
            //Declare model
            SidebarVM model;

            using (Db db = new Db())
            {

                //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);
                //Init model
                model = new SidebarVM(dto);

            }
            //Return view with model
            return View(model);
        }

        //POST: Admin/Pages/EditSidebar
        [HttpPost]
        public ActionResult EditSidebar(SidebarVM model)
        {
            using (Db db = new Db())
            {

                //Get the DTO
                SidebarDTO dto = db.Sidebar.Find(1);
                //DTO the body
                dto.Body = model.Body;

                //Save
                db.SaveChanges();
            }

            //Set TempData message
            TempData["SM"] = "Ai editat sidebar-ul";

            //Redirect

            return RedirectToAction("EditSidebar");
        }
    }
}