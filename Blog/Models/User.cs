namespace Blog.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Salt { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// A3 - Sensitive Data Exposure - Critical vulnerability, this logs out the user's password as a string!
        /// </summary>
        /// <returns>The string value of all the properties on the user.</returns>
        public override string ToString()
        {
            return
                $"Id: {Id} FirstName: {FirstName}  LastName: {LastName} UserName: {Username} Salt: {Salt} Password: {Password}";
        }

        /// <summary>
        /// A3 - Sensitive Data Exposure - This safely doesn't log the password out when called.
        /// </summary>
        /// <returns>A string representation of all the properties except password.</returns>
        public string SafeToString()
        {
            return
                $"Id: {Id} FirstName: {FirstName}  LastName: {LastName} UserName: {Username} Salt: {Salt} Password: ******";

        }
    }
}