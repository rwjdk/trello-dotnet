using System;
using System.Collections.Generic;

namespace TrelloDotNet.Model.Options.AddCardToInboxOptions
{
    /// <summary>
    /// Options for adding a card to the inbox of the token Member
    /// </summary>
    public class AddCardToInboxOptions
    {
        /// <summary>
        /// Name of Card
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of Card
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Start Date of Card
        /// </summary>
        public DateTimeOffset? Start { get; set; }

        /// <summary>
        /// Due Date and Time of the Card
        /// </summary>
        public DateTimeOffset? Due { get; set; }

        /// <summary>
        /// Checklists to add to the Card
        /// </summary>
        public List<Checklist> Checklists { get; set; }

        /// <summary>
        /// Cover to add to the Card
        /// </summary>
        public CardCover Cover { get; set; }

        /// <summary>
        /// File Attachments to add to the Card
        /// </summary>
        public List<AttachmentFileUpload> AttachmentFileUploads { get; set; }

        /// <summary>
        /// URL Link Attachments to add to the Card
        /// </summary>
        public List<AttachmentUrlLink> AttachmentUrlLinks { get; set; }

        /// <summary>
        /// Position 
        /// </summary>
        public decimal? Position { get; set; }

        /// <summary>
        /// Named Position
        /// </summary>
        public NamedPosition? NamedPosition { get; set; }

        /// <summary>
        /// If Card is Complete
        /// </summary>
        public bool DueComplete { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Name of the Card</param>
        public AddCardToInboxOptions(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        /// Ids of Labels to add to the Card (Note: Label IDs here are Inbox Specific; aka every user have different inbox labels)
        /// </summary>
        public List<string> LabelIds { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public AddCardToInboxOptions()
        {
            Checklists = new List<Checklist>();
            AttachmentUrlLinks = new List<AttachmentUrlLink>();
            AttachmentFileUploads = new List<AttachmentFileUpload>();
            NamedPosition = Model.NamedPosition.Bottom;
            LabelIds = new List<string>();
        }
    }
}