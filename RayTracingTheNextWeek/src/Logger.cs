namespace RayTracingTheNextWeek
{
    internal class Logger
    {
        // private readonly string fixedMessage;
        private readonly int messageLeftPosition;
        private readonly int messageTopPosition;

        public Logger(in string fixedMessage)
        {
            // this.fixedMessage = fixedMessage;
            Console.Write(fixedMessage);
            messageLeftPosition = Console.CursorLeft;
            messageTopPosition = Console.CursorTop;
        }

        public void UpdateMessage(in string variableMessage)
        {
            Console.SetCursorPosition(messageLeftPosition, messageTopPosition);
            Console.Write(variableMessage + new string(' ', Console.WindowWidth - variableMessage.Length - messageLeftPosition));
        }

        public void ClearAndUpdate(in string finalMessage = "Done.")
        {
            Console.SetCursorPosition(0, this.messageTopPosition);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, this.messageTopPosition);
            Console.WriteLine(finalMessage);
        }
    }
}
