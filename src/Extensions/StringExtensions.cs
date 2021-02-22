namespace Bytewizer.TinyCLR.DigitalPortal
{
    public static class StringExtensions
    {
        public static bool Contains(this string source, string value)
        {
            if (string.IsNullOrEmpty(source))
            {
                return false;
            }

            return source.ToLower().IndexOf(value.ToLower()) >= 0;
        }
    }
}
