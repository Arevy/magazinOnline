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
        public ActionResult AddPage()
        {
            return View();
        }
    }
}