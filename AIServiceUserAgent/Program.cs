using Newtonsoft.Json;
using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace AIServiceUserAgent
{
    class Program
    {
        public const string CustomVisionPredictionUrl = "https://australiaeast.api.cognitive.microsoft.com/customvision/v3.0/Prediction/f2635174-266f-4c0b-9bb1-2a9aa4773bcf/detect/iterations/Iteration9/image";
        public const string CustomVisionPredictionKey = "";

        static void Main(string[] args)
        {

            startMonitor();
            while (true) ; //this is terrible way to keep open, need to remove and add try/catch
        }

        public static async Task PredictImageContentsAsync(Image image)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Prediction-key", CustomVisionPredictionKey);

            byte[] imageData = GetImageAsByteArray(image);

            HttpResponseMessage response;
            using (var content = new ByteArrayContent(imageData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(CustomVisionPredictionUrl, content);
            }

            var resultJson = await response.Content.ReadAsStringAsync();
            var x = JsonConvert.DeserializeObject<PredictionResult>(resultJson);
            foreach(Prediction pred in x.Predictions)
            {
                if(pred.TagName == "Error" && pred.Probability > 0.45d)
                {
                    //Debug code
                    Console.WriteLine("Error Found");

                    //Create and send ticket here
                }
                else
                {
                    //Debug code
                    Console.WriteLine("No Error Here");
                }
            }
        }

        private byte[] StreamToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        private static byte[] GetImageAsByteArray(Image img)
        {
            using (var stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                return stream.ToArray();
            }
        }

        private static void t_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Console.WriteLine("Taking screenshot");
            var test = CaptureScreen.GetDesktopImage();
            PredictImageContentsAsync(test).Wait();
            Console.WriteLine("Finshed screenshot");
        }

        private static void startMonitor()
        {
            Timer t = new Timer(10000); // 1 sec = 1000, 60 sec = 60000

            t.AutoReset = true;

            t.Elapsed += new System.Timers.ElapsedEventHandler(t_Elapsed);

            t.Start();
        }
    }
}
