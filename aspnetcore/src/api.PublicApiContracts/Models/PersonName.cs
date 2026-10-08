namespace ResearchFi.PersonPublicApi
{
    /// <summary>
    /// Person's name (schema: person_name).
    /// </summary>
    public class PersonName
    {
        /// <summary>
        /// Person's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Person's last name.
        /// </summary>
        public string LastName { get; set; }
    }
}
