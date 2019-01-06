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
        private object[] id;

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

        //// GET: Admin/Pages/PageDetails/id
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
    }
}