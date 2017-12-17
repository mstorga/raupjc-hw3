using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zad2.Models
{
    public class TodoViewModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? DateDue { get; set; }
        public DateTime? DateCompleted { get; set; }
        

        public TodoViewModel(Guid id, string text, DateTime dateCreated, DateTime? dateDue, DateTime? dateCompleted)
        {
            this.Id = id;
            this.Text = text;
            this.DateCreated = dateCreated;
            this.DateDue = dateDue;
            this.DateCompleted = dateCompleted;
        }

        public TodoViewModel()
        {
            
        }

        public string DaysLeft()
        {
            if (DateDue != null)
            {
                int diffrence=(DateDue.Value.Date - DateTime.Now.Date).Days;
                if (diffrence < 0)
                {
                    return "(Trebalo je biti obavljeno prije " + diffrence.ToString().Substring(1) + " dana!)";
                }
                else if (diffrence == 1)
                {
                    return "(za " + diffrence + " dan!)";
                }
                else
                {
                    return "(za " + diffrence + " dana!)";
                }
            }
            return "";
        }
    }
}
