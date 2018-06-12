using System.Collections.Generic;

namespace Lexer.Entity
{
    public class Result
    {
        public List<string> Ids { get; set; }
        public List<Token> Tokens { get; set; }

        public Result(List<string> ids, List<Token> tokens)
        {
            this.Ids = ids;
            this.Tokens = tokens;
        }
    }
}