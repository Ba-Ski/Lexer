namespace Lexer.Entity
{
    public class Token
    {
        public string Value { get; set; }
        public Position Position { get; set; }
        public Types Type { get; set; }
        public string TokenStr { get; set; }

        public Token(string value, Types type, Position position)
        {
            this.Value = value;
            this.Position = position;
            this.Type = type;
        }
    }
}