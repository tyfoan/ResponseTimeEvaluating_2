using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Response
    {
        [Key]
        public int ResponseId { get; set; }

        [Required(ErrorMessage = "Поле не может быть пустым. Введите url.")]
        [Display(Name = "Url")]
        public string ResponseUrl { get; set; }
        public int ResponseTime { get; set; }
        public string Host { get; set; }


        public int SiteId { get; set; }
    }
}