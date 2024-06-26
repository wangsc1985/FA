using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FuturesAssistant.Models
{
    [Serializable]
    public class User
    {
        public User()
        {
            Id = Guid.NewGuid();
            UserName = Email = UserPassword = "";
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

        [Required]
        [MinLength(1,ErrorMessage="用户名至少1个字符！")]
        [MaxLength(20,ErrorMessage="用户名最多20个字符！")]
        public string UserName { get; set; }

        [Required]
        public string UserPassword { get; set; }

        [Required]
        //[RegularExpression("/^[0-9a-z_]+@(([0-9a-z]+)[.]){1,2}[a-z]{2,3}$/",ErrorMessage="邮箱格式不正确！")]
        public string Email { get; set; }
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return UserName.Equals(((User)obj).UserName);
        }

        public override int GetHashCode()
        {
            return UserName.GetHashCode();
        }
    }
}
