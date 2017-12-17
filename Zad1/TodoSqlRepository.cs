using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Zad1
{
    public class TodoSqlRepository : ITodoRepository
    {
        private readonly TodoDbContext _context;

        public TodoSqlRepository(TodoDbContext context)
        {
            _context = context;
        }


        public async Task<TodoItem> Get(Guid todoId, Guid userId)
        {
            TodoItem item = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id.Equals(todoId));
            if (item != null && !item.UserId.Equals(userId))
            {
                throw new TodoAccessDeniedException("User is not the owner of the Todo item");
            }
            return item;
        }

        public async Task Add(TodoItem todoItem)
        {
            if (await _context.TodoItems.FirstOrDefaultAsync(t => t.Id.Equals(todoItem.Id)) != null)
            {
                throw new DuplicateTodoItemException("Todo item with the same id already exists.");
            }

            _context.TodoItems.Add(todoItem);
            _context.SaveChanges();
        }
        //koristit get
        public async Task<bool> Remove(Guid todoId, Guid userId)
        {
            TodoItem item = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id.Equals(todoId));
            if (item != null && !item.UserId.Equals(userId))
            {
                throw new TodoAccessDeniedException("User is not the owner of the Todo item");
            }

            if (item == null)
            {
                return false;
            }

            _context.TodoItems.Remove(item);
            _context.SaveChanges();
            return true;
        }

        public async Task Update(TodoItem todoItem, Guid userId)
        {
            TodoItem item = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id.Equals(todoItem.Id));
            if (item != null && !item.UserId.Equals(userId))
            {
                throw new TodoAccessDeniedException("User is not the owner of the Todo item");
            }
            if (item == null)
            {
                _context.TodoItems.Add(todoItem);
            }
            else
            {
                _context.Entry(todoItem).State = EntityState.Modified;
                item = todoItem;
            }

            _context.SaveChanges();
        }

        public async Task<bool> MarkAsCompleted(Guid todoId, Guid userId)
        {
            TodoItem item = await _context.TodoItems.FirstOrDefaultAsync(t => t.Id.Equals(todoId));
            if (item != null && !item.UserId.Equals(userId))
            {
                throw new TodoAccessDeniedException("User is not the owner of the Todo item");
            }

            if (item == null)
            {
                return false;
            }

            item.MarkAsCompleted();
            _context.SaveChanges();
            return true;
        }

        public async Task<List<TodoItem>> GetAll(Guid userId)
        {
            return await _context.TodoItems.Where(t => t.UserId.Equals(userId))
                                      .OrderByDescending(t => t.DateCreated).ToListAsync();
        }

        public async Task<List<TodoItem>> GetActive(Guid userId)
        {
            return await _context.TodoItems.Where(t => !t.DateCompleted.HasValue && t.UserId.Equals(userId)).OrderBy(t => t.DateDue).ToListAsync();
        }

        public async Task<List<TodoItem>> GetCompleted(Guid userId)
        {
            return await _context.TodoItems.Where(t => t.DateCompleted.HasValue && t.UserId.Equals(userId)).OrderByDescending(t => t.DateCompleted).ToListAsync();
        }

        public async Task<List<TodoItem>> GetFiltered(Func<TodoItem, bool> filterFunction, Guid userId)
        {
            return _context.TodoItems.Where(filterFunction).Where(t => t.UserId.Equals(userId)).ToList();
        }

        public async Task<TodoItemLabel> AddLabelToDb(TodoItemLabel label)
        {
            TodoItemLabel labelinDb = await _context.TodoLabels.FirstOrDefaultAsync(l => l.Value.Equals(label.Value));
            if (labelinDb == null)
            {
                _context.TodoLabels.Add(label);
                await _context.SaveChangesAsync();
                return label;
            }
            return labelinDb;
        }

        public async Task AddLabelToTodoItem(TodoItemLabel label, TodoItem todoItem)
        {
            if (!todoItem.Labels.Contains(label))
            {
                _context.Entry(todoItem).State = EntityState.Modified;
                todoItem.Labels.Add(label);
                await _context.SaveChangesAsync();
            }

        }
    }
}