namespace api.PublicApiContracts
{
    /// <summary>
    /// Response body for the Public API mockup "hello" endpoint.
    /// </summary>
    public class PublicApiHelloResponse
    {
        /// <summary>
        /// Greeting message, e.g. "Hello username".
        /// </summary>
        public string Message { get; set; }
    }
}
