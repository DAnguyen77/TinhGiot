using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace tinhGiot.BaseClass
{
    public class News
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public int CategoryID { get; set;}
        public string PostContent { get; set; }
        public string Author { get; set; }
        public DateTime PostTime { get; set; }
        public string Image { get; set; }
        public string Lang { get; set; }
        public string MoreInfo { get; set; }
        public HttpPostedFileWrapper ImageFile { get; set; }

    }
}