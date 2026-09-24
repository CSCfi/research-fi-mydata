namespace api.PublicApiContracts
{
    /// <summary>
    /// Request body for the Public API mockup "hello" endpoint.
    /// </summary>
    public class PublicApiHelloRequest
    {
        /// <summary>
        /// Username to greet.
        /// </summary>
        public string Username { get; set; }
    }
}
