﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FuturesAssistant.Models
{
    [Serializable]
    [Table(name: "Parameter")]
    public class Parameter
    {
        public Parameter()
        {
            Id = Guid.NewGuid().ToString();
            Name = Value = "";
        }
        [Key]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        [Required(ErrorMessage = "用户外键不能为空！")]
        public string UserId { get; set; }
        /// <summary>
        /// 所属账号
        /// </summary>
        public User User { get; set; }
        public override bool Equals(object obj)
        {
            if (null == obj)
                return false;
            if (obj.GetType() != GetType())
                return false;
            return UserId == ((Parameter)obj).UserId && Name.Equals(((Parameter)obj).Name);
        }

        public override int GetHashCode()
        {
            return (UserId.ToString() + Name).GetHashCode();
        }
    }
}
