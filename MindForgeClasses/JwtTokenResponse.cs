using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MindForgeClasses
{
    public class JwtTokenResponse
    {
        public string Token {  get; set; }
        public string TokenType { get; set; } = "Bearer";
    }
}
