using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.MultipleModelInOneView
{
    public class ModelDanhMucs
    {
        public IEnumerable<DANHMUC> DANHMUCs { get; set; }
        public IEnumerable<LOAISANPHAM> LOAISANPHAMs { get; set; }


    }
}