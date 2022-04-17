using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class ErrorDto
    {
        public List<String> Errors { get; private set; } = new List<String>();
        // Private kullanılması sadece bu classtaki const. üzerinden set edilebilmesinden.
        public bool IsShow { get; private set; } //İlgili hatanın kullanıcıya gösterilme durumu.Sadece yazılımcının görmesi gereken hatalar olur.

        public ErrorDto(string Error, bool isShow)
        {
            Errors.Add(Error);
            IsShow = isShow;
        }
        public ErrorDto(List<string> errors, bool isShow)
        {
            Errors = errors;
            IsShow = isShow;
        }
    }
}
