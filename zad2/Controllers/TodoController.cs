using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Interfaces;
using Microsoft.AspNetCore.Identity;
using zad2.Models;
using zad2.Models.ManageViewModels;
using AutoMapper;

namespace zad2.Controllers
{
    public class TodoController : Controller
    {
        private readonly ITodoRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;

        public TodoController(ITodoRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            List<TodoItem> items = await _repository.GetActive(new Guid(applicationUser.Id));
            List<TodoViewModel> todoViewModels = Mapper.Map<List<TodoItem>, List<TodoViewModel>>(items);
            IndexViewModel indexViewModel = new IndexViewModel(todoViewModels);
            return View(indexViewModel);
        }

        // GET: Todo/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Todo/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Todo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Todo/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Todo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Todo/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Todo/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        public IActionResult Add()
        {
            return View();
        }

        [HttpGet("MarkAsCompleted/{Id}")]
        public async Task<IActionResult> MarkAsCompleted(Guid Id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            await _repository.MarkAsCompleted(Id, new Guid(applicationUser.Id));
            return RedirectToAction("Index");

        }

        [HttpPost]
        public async Task<IActionResult> Add(AddTodoViewModel item)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = await _userManager.GetUserAsync(HttpContext.User);
                TodoItem todo = new TodoItem(item.Text, new Guid(applicationUser.Id))
                {
                    DateDue = item.DateDue
                };
                if (item.Labels != null)
                {
                    TodoItemLabel todoItemLabel = new TodoItemLabel(item.Labels);
                    todoItemLabel = _repository.AddLabel(todoItemLabel);
                    todo.Labels.Add(todoItemLabel);
                }
                _repository.Add(todo);
                return RedirectToAction("Index");
            }
            return View();
        }

        public async Task<IActionResult> Completed()
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            List<TodoItem> items = await _repository.GetCompleted(new Guid(applicationUser.Id));
            List<TodoViewModel> todoViewModels = Mapper.Map<List<TodoItem>, List<TodoViewModel>>(items);
            CompletedViewModel completedViewModel = new CompletedViewModel(todoViewModels);
            return View(completedViewModel);
        }

        [HttpGet("RemoveFromCompleted/{Id}")]
        public async Task<IActionResult> RemoveFromCompleted(Guid Id)
        {
            ApplicationUser applicationUser = await _userManager.GetUserAsync(HttpContext.User);
            await _repository.Remove(Id, new Guid(applicationUser.Id));
            return RedirectToAction("Index");
        }
    }
}