﻿using System;
using System.Text.Json.Serialization;

namespace Noteapp.Api.Entities
{
    public class Note : BaseEntity
    {
        public string Text { get; set; }
        public bool Locked { get; set; }
        public bool Archived { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        [JsonIgnore]
        public AppUser Author { get; set; }
        public int AuthorId { get; set; }
    }
}
