using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MediatR.LightCore.Tests
{
    public class StringWriter : TextWriter
    {
        private readonly StringBuilder writer = new StringBuilder();

        public StringWriter()
        {
        }

        public override void Write(char value)
        {
            writer.Append(value);
        }

        public override Task WriteLineAsync(string value)
        {
            writer.AppendLine(value);
            return Task.CompletedTask;
        }

        public string Contents => writer.ToString();

        public override Encoding Encoding => Encoding.Default;
    }
}