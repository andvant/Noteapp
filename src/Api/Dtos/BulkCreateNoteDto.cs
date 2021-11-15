using System.Collections.Generic;

namespace Noteapp.Api.Dtos
{
    public record BulkCreateNoteDto(IEnumerable<string> Texts);
}
