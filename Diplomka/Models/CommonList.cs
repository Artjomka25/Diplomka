using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Diplomka.Models
{
    public class CommonList
    {
        public IEnumerable<Application> Applications { get; set; }

        public SelectList Orders { get; set; }
        public SelectList Grains { get; set; }

    }
}
