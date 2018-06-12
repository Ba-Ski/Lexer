namespace Lexer.Entity
{
    public class Position
    {
        public int Line { get; set; }
        public int Row { get; set; }

        public Position(int line, int row)
        {
            this.Line = line;
            this.Row = row;
        }

        
    }
}