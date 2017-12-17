using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zad1;

namespace Zad2.Models
{
    public class IndexViewModel
    {
        public List<TodoViewModel> TodoItems { get; set; }

        public IndexViewModel(List<TodoItem> list)
        {
            TodoItems = new List<TodoViewModel>();
            foreach (TodoItem todoItem in list)
            {
                TodoItems.Add(new TodoViewModel(todoItem.Id, todoItem.Text, todoItem.DateCreated, todoItem.DateDue, todoItem.DateCompleted));
            }
        }
    }
}
