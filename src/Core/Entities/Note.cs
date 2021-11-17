﻿using System;
using System.Text.Json.Serialization;

namespace Noteapp.Core.Entities
{
    public class Note : BaseEntity
    {
        public string Text { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public bool Pinned { get; set; }
        public bool Published => PublicUrl != null;
        public string PublicUrl { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        [JsonIgnore]
        public AppUser Author { get; set; }
        public int AuthorId { get; set; }
    }
}