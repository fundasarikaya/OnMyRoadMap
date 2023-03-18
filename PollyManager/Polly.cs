using Polly;
using Polly.Timeout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PollyManager
{
    public class Polly
    {
        /*
         * Polly Kütüphanesi 
         * Servisler arası haberleşme süreçlerinde bir çok pattern’ı uygulamamızı sağlayan operasyonellikler barındıran, Retry, Circuit Breaker, Timeout, Bulkhead Isolation ve Fallback gibi senaryolarda kullanılabilir niteliğe sahip open source bir kütüphanedir.
           Bizler bu içeriğimizde, Retry Pattern’ı Polly kütüphanesi ile uygulamayı ele alıyor olacağız.
         */

        public async void Example()
        {
            //Hem Hataların Hem de Dönüş Değerlerinin Tek Bir Politikada Ele Alınması
            //HttpStatusCode[] httpStatusCodesWorthRetrying = {
            //       HttpStatusCode.RequestTimeout, // 408
            //       HttpStatusCode.InternalServerError, // 500
            //       HttpStatusCode.BadGateway, // 502
            //       HttpStatusCode.ServiceUnavailable, // 503
            //       HttpStatusCode.GatewayTimeout // 504
            //    };

            //HttpResponseMessage result = await Policy
            //  .Handle<HttpRequestException>()
            //  .OrResult<HttpResponseMessage>(r => httpStatusCodesWorthRetrying.Contains(r.StatusCode))
            //  .RetryAsync()
            //  .ExecuteAsync( /* some Func<Task<HttpResponseMessage>> */ );

            //****** ****** ****** ****** *****

            //Başarılı Olana Kadar Yeniden İstek Gönderme,Sonsuza Kadar İstek Gönderirken, Alınan Hatayı Yakalama
            //Policy
            //  .Handle<SomeExceptionType>()
            //  .RetryForever(onRetry: exception =>
            //  {
            //      //Her yeniden denemeden önce gerekli loglama vs. operasyonları burada gerçekleştirilir.     
            //  });

            await Policy.Handle<Exception>().RetryAsync(5,
              (e, r) =>
              {
                  Console.WriteLine("Tekrar deneniyor...");
              }).ExecuteAsync(async () =>
              {
                  HttpClient httpClient = new HttpClient();
                  var message = await httpClient.GetAsync("https://localhost:5001/api/products");
                  var content = message.Content.ReadAsStringAsync();
                  Console.WriteLine(content.Result);
              });
        }

        public async void PollyTimeoutOptimistic()
        {
            //Optimistic Timeout, CancellationToken aracılığıyla çalışmaktadır.
            var policy1 = Policy.Handle<Exception>().RetryAsync(5, (e, i) => Console.WriteLine("Tekrar deneniyor..."));
            var policy2 = Policy.TimeoutAsync(125);
            var combinePolicy = policy1.WrapAsync(policy2);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await combinePolicy.ExecuteAsync(async (cancellationToken) =>
            {
                HttpClient httpClient = new HttpClient();
                var message = await httpClient.GetAsync("https://localhost:5001/api/products");
                var content = message.Content.ReadAsStringAsync();
                Console.WriteLine(content.Result);
            }, cancellationTokenSource.Token);
            //Optimistic Timeout, kesinlikle bir CancellationToken bekleyecektir.
        }

        public async void PollyTimeoutPessimistic()
        {
            //Pessimistic Timeout, CancellationToken olmasa dahi bildirilen zamanın dolması neticesinde net hata fırlatmaktadır.
            var policy1 = Policy.Handle<Exception>().RetryAsync(5, (e, i) => Console.WriteLine("Tekrar deneniyor..."));
            var policy2 = Policy.TimeoutAsync(125, TimeoutStrategy.Pessimistic);
            var combinePolicy = policy1.WrapAsync(policy2);
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            await combinePolicy.ExecuteAsync(async (cancellationToken) =>
            {
                HttpClient httpClient = new HttpClient();
                var message = await httpClient.GetAsync("https://localhost:5001/api/products");
                var content = message.Content.ReadAsStringAsync();
                Console.WriteLine(content.Result);
            }, cancellationTokenSource.Token);
        }

        public async void CircuitBreaker()
        {
            //Circuit Breaker, servisler arasındaki çağrıları düzenleyen bir pattern’dır
            //Belirlenen sayıda istekten sonra Circuit Breaker ile devreyi kesebilmek için aşağıdaki politikanın kullanılması yeterlidir;

            await Policy.Handle<Exception>().CircuitBreakerAsync(2, TimeSpan.FromMinutes(10), (e, t) =>
            {
                Console.WriteLine("Tekrar deneniyor...");
            }, () =>
            {
                Console.WriteLine("Reset");
            }).ExecuteAsync(async () =>
            {
                HttpClient httpClient = new HttpClient();
                var message = await httpClient.GetAsync("https://localhost:5001/api/products");
                var content = message.Content.ReadAsStringAsync();
                Console.WriteLine(content.Result);
            });
        }
    }
}

