using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Sloader.Fiddle.Models
{
    public class HomeViewModel
    {
        public string Input { get; set; }
        public string Output { get; set; }
        public bool HasError { get; set; }
    }
}
