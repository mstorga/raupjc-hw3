using System.Collections.Generic;

namespace zad2.Controllers
{
    internal class CompletedViewModel
    {
        private List<TodoViewModel> todoViewModels;

        public CompletedViewModel(List<TodoViewModel> todoViewModels)
        {
            this.todoViewModels = todoViewModels;
        }
    }
}