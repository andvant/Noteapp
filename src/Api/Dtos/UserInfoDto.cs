﻿namespace Noteapp.Api.Dtos
{
    public class UserInfoDto
    {
        public string access_token { get; set; }
        public string email { get; set; }
        public string encryption_salt { get; set; }
    }
}
