namespace PsTest
{
    /// <summary>
    /// Represents a color formatter which formats colors.
    /// </summary>
    public interface IColorFormatter
    {
        /// <summary>
        /// Sets the background color to the specified color.
        /// </summary>
        /// <param name="color">
        /// The color.
        /// </param>
        void SetBackgroundColor(string color);

        /// <summary>
        /// Get the current background color.
        /// </summary>
        /// <returns>
        /// The background color.
        /// </returns>
        string GetBackgroundColor();
    }
}