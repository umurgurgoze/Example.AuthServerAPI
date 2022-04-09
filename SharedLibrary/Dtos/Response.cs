using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }            // T, Object türünden de olabilir fakat cast maliyeti yaratır.
        public int StatusCode { get; private set; }    // Bu propertyler sadece bu class içinden set edilecek.Dışarıdan set edilemeyecek.
        [JsonIgnore]                                   // Response sınıfı json dataya dönüştürülürken Issuccessful İgnore edilecek.
        public bool IsSuccessful { get; private set; } //Kendi iç yapımızda kullanacağız.Olayın başarılı olup olmadığına direkt bakabiliriz.
        public ErrorDto Error { get; private set; }

        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T>
            {
                Data = data,
                StatusCode = statusCode,
                IsSuccessful = true
            };
        }
        public static Response<T> Success(int statusCode)
        {
            return new Response<T>
            {
                Data = default,
                StatusCode = statusCode,
                IsSuccessful = true
            };
        } // Ürünü güncelleme ya da silme yaparsak geriye data dönmeye gerek duymayız. 200 ile boş data döneriz. BestPractise*

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }
        public static Response<T> Fail(string errorMessage, int statusCode, bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage, isShow);
            return new Response<T>
            {
                Error = errorDto,
                StatusCode = statusCode,
                IsSuccessful = false
            };
        }
    }
}
