using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public enum RelationshipAction { Apply, Request, Delete}
    public class RelationshipRequest
    {
        public RelationshipAction RelationshipAction { get; set; }
    }
}
