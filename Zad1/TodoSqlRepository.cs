using Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zad1
{
    public class TodoSqlRepository : ITodoRepository{ 
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }
        public async void Add(TodoItem todoItem)
        {
            if (await _context.TodoItems.FirstOrDefaultAsync(i => i.Id.Equals(todoItem.Id)) != null)
                throw new DuplicateTodoItemException(todoItem.Id + "already exists");
            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
        }

        public async Task<TodoItem> Get(Guid todoId, Guid userId)
        {
            TodoItem item = await _context.TodoItems.FirstOrDefaultAsync(i => i.Id.Equals(todoId));
            if (!item.UserId.Equals(userId)) throw new TodoAccessDeniedException("User is not owner of todo item");
            return item;
        }

        public async Task<List<TodoItem>> GetActive(Guid userId)
        {
            return await _context.TodoItems.Where(i => !i.IsCompleted && i.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<TodoItem>> GetAll(Guid userId)
        {
            return await _context.TodoItems.Where(i=> i.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<TodoItem>> GetCompleted(Guid userId)
        {
            return await _context.TodoItems.Where(i => i.IsCompleted && i.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<List<TodoItem>> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return await _context.TodoItems.Where(i => filterFunction(i) && i.UserId.Equals(userId)).ToListAsync();
        }

        public async Task<bool> MarkAsCompleted(Guid todoId, Guid userId)
        {
            TodoItem todoItem = await Get(todoId, userId);
            if (!todoItem.UserId.Equals(userId)) throw new TodoAccessDeniedException("User is not owner of todo item");
            if (todoItem == null) return false;

            todoItem.MarkAsCompleted();
            _context.SaveChanges();
            return true;
        }

        public async Task<bool> Remove(Guid todoId, Guid userId)
        {
            TodoItem todoItem = await Get(todoId, userId);
            if (!todoItem.UserId.Equals(userId)) throw new TodoAccessDeniedException("User is not owner of todo item");
            if (todoItem == null)
                return false;

            _context.TodoItems.Remove(todoItem);
            _context.SaveChanges();
            return true;
        }

        public async void Update(TodoItem todoItem, Guid userId)
        {
            TodoItem item = await Get(todoItem.Id, userId);
            if (!item.UserId.Equals(userId)) throw new TodoAccessDeniedException("User is not owner of todo item");
            if (item != null) item = todoItem;
            else
                Add(todoItem);
            _context.SaveChanges();
        }
    }
}
