namespace OpenAAP
{
    public interface IWarning
    {
        /// <summary>
        /// Machine readable identifier of the warning
        /// </summary>
        string Name { get;}

        /// <summary>
        /// Human readable message to be used for debugging
        /// </summary>
        string Message { get; }
    }
}
