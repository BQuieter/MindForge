using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public enum Relationship {  Friends = 1, RequestSented = 2, Blocked = 3, None = 4 }
    public class UserRelationshipResponse
    {
        public bool? IsYouInitiator { get; set; }
        public Relationship Relationship { get; set; }
    }
}
