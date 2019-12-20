﻿using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Domain.Posts
{
    public class Models
    {
        public class NewPostInfo
        {
            public string NickName { get; set; }
            public string Content { get; set; }
            public List<IFormFile> Images { get; set; }

            public (bool, string) IsValid()
            {
                if (string.IsNullOrWhiteSpace(NickName))
                    return (false, "Need NickName");
                if (string.IsNullOrWhiteSpace(Content))
                    return (false, "Need Contene");

                return (true, "");
            }
        }
    }
}
