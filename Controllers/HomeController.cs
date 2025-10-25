using Microsoft.AspNetCore.Mvc;
using RedBlackTree2.Models;
using RedBlackTree2.Services;

namespace RedBlackTree2.Controllers
{
    
    public class HomeController : Controller
    {
        private  readonly RedBlackTreeService _treeService;
        
        public HomeController(RedBlackTreeService treeService)
        {
            _treeService = treeService;

        }

        public IActionResult Index()
        {
            return View(_treeService.GetTreeData());
        }

        [HttpPost]
        public IActionResult Update(string action, string Value)
        {
            switch (action)
            {
                case "insert":
                    _treeService.InsertNode(Value);
                    break;
                case "delete":
                    var (successD, message1) = _treeService.DeleteNode(Value);
                    TempData["Message"] = message1;
                    break;
                case "search":
                    var (successS, message2) = _treeService.SearchNode(Value);
                    TempData["Message"] = message2;
                  
                    break;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult ManageTree(string action)
        {
            Console.WriteLine(action);
            switch (action)
            {
                case "deleteall":
                    _treeService.ClearTree();
                    break;
                case "info":
                    var stats = _treeService.GetTreeStatistics();
                    if (stats != null) TempData["Message"] = stats;
                    break;
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult HandleFiles(string func, IFormFile file)
        {
            switch (func)
            {
                case "export":
                    var (data, fileName, error) = _treeService.ExportTree();
                    if (!string.IsNullOrEmpty(error))
                        TempData["Message"] = error;
                    else
                        return File(data, "application/json", fileName);
                    break;
                case "import":
                    var importError = _treeService.ImportTree(file);
                    if (!string.IsNullOrEmpty(importError))
                        TempData["Message"] = importError;
                    break;
            }
            return RedirectToAction("Index");
        }
    }
}