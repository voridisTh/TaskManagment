﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }   
        public string Description { get; set; }
        public int StatusId { get; set; }
        public Status ProjectStatus { get; set; }
        public ICollection<TaskItem> Tasks { get; set; }
    }
}
