
using System;

namespace WWTMVC5.Models
{

    
    /// <summary>
    /// Class representing the details about a User profile.
    /// </summary>
    [Serializable]
    public class ProfileDetails
    {

        public ProfileDetails()
        {

        }

        public ProfileDetails(dynamic json)
        {
            PUID = this.CID = json.id;
            Email = json.emails.preferred;
            FirstName = json.first_name;
            LastName = json.last_name;
            LastLogOnDatetime = DateTime.UtcNow;
        }

        /// <summary>
        /// Gets or sets id.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets PUID of the user.
        /// </summary>
        public string PUID { get; set; }

        public string CID { get; set; }

        //e.g. windowslive|abc123
        public string FederatedID { get; set; }

        /// <summary>
        /// Gets or sets the first name of the user.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the user.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets Email address of the user
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets last login time.
        /// </summary>
        public DateTime? LastLogOnDatetime { get; set; }
        
        /// <summary>
        /// Gets or sets AboutMe of the user
        /// </summary>
        public string AboutMe { get; set; }

        /// <summary>
        /// Gets or sets Affiliation of the user
        /// </summary>
        public string Affiliation { get; set; }

        /// <summary>
        /// Gets or sets the type of the user.
        /// </summary>
        public UserTypes UserType { get; set; }

        /// <summary>
        /// Gets or sets the total size of the user.
        /// </summary>
        public long TotalSize { get; set; }

        /// <summary>
        /// Gets or sets the Consumed size of the user.
        /// </summary>
        public decimal ConsumedSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether user has subscribed to notifications or not.
        /// </summary>
        public bool IsSubscribed { get; set; }

        /// <summary>
        /// Gets or sets the profile picture id of the user.
        /// </summary>
        public Guid? PictureID { get; set; }
    }
}