using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Zad1;
using Zad2.Models;
using Zad2.Models.ManageViewModels;

namespace Zad2.Controllers
{
    [Authorize]
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Models.IndexViewModel model = new Models.IndexViewModel(await _repository.GetActive(new Guid(user.Id)));

            return View(model);
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (ModelState.IsValid)
            {
                var todoItem = new TodoItem(model.Text, new Guid(user.Id))
                {
                    DateDue = model.DateDue
                };
                await _repository.Add(todoItem);
                
                if (model.Labels != null)
                {
                       string[] split = model.Labels.Split(",".ToCharArray());
                       foreach (var label in split )
                       {
                          var lab = await _repository.AddLabelToDb(new TodoItemLabel(label.ToUpper().Trim()));
                          await _repository.AddLabelToTodoItem(lab, todoItem);
                       }
                }  
                return RedirectToAction("Index");
            }
            return View(model);
        }

        
        public async Task<IActionResult> Completed()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            CompletedViewModel model = new CompletedViewModel(await _repository.GetCompleted(new Guid(user.Id)));

            return View(model);
        }

        [HttpGet("MarkAsCompleted/{Id}")]
        public async Task<IActionResult> MarkAsCompleted(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _repository.MarkAsCompleted(id, new Guid(user.Id));
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> RemoveFromCompleted(Guid id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await _repository.Remove(id, new Guid(user.Id));
            return RedirectToAction("Completed");
        }
    }
}
