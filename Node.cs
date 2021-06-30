using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Transactions;

namespace Mdparser12
{
    public class Node
    {
        public string Id { get; set; }
        public List<Node> NodeList = new List<Node>();
    }

    public class Source : Node
    {
        public Source(string Id)
        {
            this.Id = Id;
        }
        public string URL { get; set; }
        public string Description { get; set; }
	}

    public class Column : Node
    {
        public Column(string Id)
        {
            this.Id = Id;
        }
    }

    public class Code : Node
    {
        public String SourceCode { get; set; }
        public Code(string Id)
        {
            this.Id = Id;
        }
    }

    public class BookMark : Node
    {
        public BookMark(string Id)
        {
            this.Id = Id;
        }
        public string Description { get; set; }
        public string URL { get; set; }
    }

}
