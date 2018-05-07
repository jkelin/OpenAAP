namespace OpenAAP.Errors
{
    public interface IError
    {
        /// <summary>
        /// Machine readable identifier of the error
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Human readable message to be used for debugging
        /// </summary>
        string Message { get; }
    }
}
