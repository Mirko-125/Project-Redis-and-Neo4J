using System.Net;
using System.Text;
using System.IO.Compression;
using System.Threading.Tasks;

namespace Databaseaccess.Controllers // Zip_Arc_Task_Edition by yours truly
{
    internal class Program
    {
        private static readonly object cachelock = new object();
        private static string root = "C:\\Users\\Mirko\\source\\repos\\Zip-Archiver\\Files";
        private static LRUCache cache = new LRUCache(3);

        static byte[] ZipFiles(string[] filenames)
        {
            Array.Sort(filenames, (x, y) => String.Compare(x, y));
            string filenamehash = String.Join(',', filenames);
            try
            {
                if (root == "") root = Environment.CurrentDirectory;

                lock (cachelock)
                {
                    byte[] res;
                    if (cache.checkget(filenamehash))
                    {
                        Console.WriteLine($"Zipped file found in cache: {filenamehash}");
                        res = cache.get(filenamehash);
                        return res;
                    }
                }

                byte[] zipbytes;
                using (MemoryStream mem = new MemoryStream())
                {
                    using (ZipArchive zip = new ZipArchive(mem, ZipArchiveMode.Create))
                    {
                        foreach (string f in filenames)
                        {
                            zip.CreateEntryFromFile(Path.Combine(root, f), f, CompressionLevel.Optimal);
                        }
                    }
                    zipbytes = mem.GetBuffer();
                }

                lock (cachelock)
                {
                    Console.WriteLine($"Zipped file added in cache: {filenamehash}");
                    cache.set(filenamehash, zipbytes);
                }

                return zipbytes;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        static async Task SendResponseAsync(HttpListenerContext c, byte[] body, string type = "text/plain; charset=utf-8", HttpStatusCode status = HttpStatusCode.OK)
        {
            HttpListenerResponse response = c.Response;
            response.ContentType = type;
            response.ContentLength64 = body.Length;
            response.StatusCode = (int)status;
            if (type == "application/zip")
            {
                response.AddHeader("Content-Disposition", $"attachment; filename=\"archived.zip\"");
            }
            System.IO.Stream output = response.OutputStream;
            await output.WriteAsync(body, 0, body.Length);
            output.Close();
        }

        static async Task AcceptAsync(HttpListenerContext context)
        {
            try
            {
                if (context.Request.HttpMethod != HttpMethod.Get.Method)
                {
                    await SendResponseAsync(context, Encoding.UTF8.GetBytes("Only GET allowed"), "text/plain; charset=utf-8", HttpStatusCode.BadRequest);
                    return;
                }

                var filenames = context.Request.Url.PathAndQuery.TrimStart('/').Split('&');
                filenames = filenames.Where(f => f != string.Empty).ToArray();

                if (filenames.Length == 0)
                {
                    await SendResponseAsync(context, Encoding.UTF8.GetBytes("There's no files that are asked for"), "text/plain; charset=utf-8", HttpStatusCode.BadRequest);
                    return;
                }

                filenames = filenames.Where(f => File.Exists(Path.Combine(root, f))).ToArray();
                if (filenames.Length == 0)
                {
                    await SendResponseAsync(context, Encoding.UTF8.GetBytes("There are no such files with that name"), "text/plain; charset=utf-8", HttpStatusCode.NotFound);
                    return;
                }

                await SendResponseAsync(context, ZipFiles(filenames), "application/zip");
            }
            catch (System.Exception e)
            {
                await SendResponseAsync(context, Encoding.UTF8.GetBytes(e.Message), "text/plain; charset=utf-8", HttpStatusCode.InternalServerError);
            }
        }

        static async Task MainAsync()
        {
            HttpListener hl = new HttpListener();
            hl.Prefixes.Add("http://127.0.0.1:8080/");
            hl.Start();
            Console.WriteLine("Server is on (http://127.0.0.1:8080/)");
            while (true)
            {
                var c = await hl.GetContextAsync();
                _ = Task.Run(() => AcceptAsync(c));
            }
        }

        static void Main()
        {
            _ = MainAsync();
            Console.ReadLine();
        }
    }
}