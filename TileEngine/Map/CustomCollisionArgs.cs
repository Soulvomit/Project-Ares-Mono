namespace TileEngine.LayerMap
{
    public class CustomCollisionArgs : System.EventArgs
    {
        private string message;

        public CustomCollisionArgs(string m)
        {
            this.message = m;
        }

        public string Message()
        {
            return message;
        }
    }
}