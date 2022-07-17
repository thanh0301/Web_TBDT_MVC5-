using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.MultipleModelInOneView
{
    public class ModelSP
    {
        public IEnumerable<SANPHAM> SANPHAMs { get; set; }
        public IEnumerable<ANHPHU> ANHPHUs { get; set; }
    }
}