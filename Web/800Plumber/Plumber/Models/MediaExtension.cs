using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Plumber.Models
{
    [Table("tbl_MediaExtension")]
    public class MediaExtension
    {
        [Key]
        [ScaffoldColumn(false)]
        public int MediaExtensionId { get; set; }

        /// <summary>
        /// Get or sets the media type:
        /// 1: Audio
        /// 2: Video
        /// </summary>
        [DisplayName("Media type")]
        [Required(ErrorMessage = "Required")]
        public int MediaType { get; set; }

        /// <summary>
        /// Gets or sets media's extension
        /// Example:
        /// Audio: MP3, OGG, WMA
        /// Video: MPG, VOB, WMV
        /// </summary>
        [DisplayName("Extension")]
        [Required(ErrorMessage = "Required")]
        public string MediaExtensionName { get; set; }

        /// <summary>
        /// Gets or sets media description
        /// </summary>
        [DisplayName("Description")]
        public string Description { get; set; }

        [DefaultValue(false)]
        [DisplayName("Deleted")]
        public bool IsDeleted { get; set; }

        public virtual List<Media> Medias { get; set; }
    }
}