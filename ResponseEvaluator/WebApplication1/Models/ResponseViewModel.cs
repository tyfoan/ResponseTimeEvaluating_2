using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class ResponseViewModel
    {
        [Key]
        public int ResponseId { get; set; }
        [Required(ErrorMessage = "Поле не может быть пустым. Введите url.")]
        [Display(Name = "Url")]
        public string ResponseUrl { get; set; }
        public int ResponseTime { get; set; }
    }
}