using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Transactions;

namespace Mdparser12
{
    public class IdClass
    {
        public string Id { get; set; }
        public string ClassName { get; set; }
    }

    public class Link : IdClass
    {
        public Link(string Id, string className)
        {
            this.Id = Id;
            this.ClassName = className;
        }
        public string Description { get; set; }
        public string URL { get; set; }
    }


}
