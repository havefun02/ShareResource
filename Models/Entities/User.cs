﻿using Microsoft.AspNetCore.Identity;
using ShareResource.Interfaces;
namespace ShareResource.Models.Entities
{
    public class User:UserBase
    {
        //public string? UserId {  get; set; }
        //public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? UserPhone { get; set; }
        public byte[]? UserIcon { get; set; }
        //public string? UserRoleId {  get; set; }
        public virtual Role? UserRole {  get; set; }
        public virtual ICollection<Img>? UserImgs { get; set; }
        public virtual ICollection<ImgLovers>? ImgLovers { get; set; }

        public virtual Token? UserToken {  get; set; }
        

    }
}
