using CmmandService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using mvc.Laparoscopy.Models;
using System.Diagnostics;
using System.Text.Encodings.Web;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Utils;

namespace mvc.Laparoscopy.Controllers
{
    public class ContactenosController : Controller
    {
        private readonly ILogger<ContactenosController> _logger;
        private readonly IOptions<EmailSettingsOptions> _optionsEmial;
        private string? host;
        private string? port;
        private string? user;
        private string? pass;
        private string? from;
        private string? enableSsl;
        private string? noReply;
        private string? destinatario;


        public ContactenosController(ILogger<ContactenosController> logger, 
            IOptions<EmailSettingsOptions> optionsEmail)
        {
            _logger = logger;
            _optionsEmial = optionsEmail;
            host = optionsEmail.Value.Host;
            port = optionsEmail.Value.Port;
            user = optionsEmail.Value.User;
            pass = optionsEmail.Value.Pass;
            enableSsl = optionsEmail.Value.EnableSsl;
            noReply = optionsEmail.Value.NoReply;
            from = optionsEmail.Value.From;
            destinatario = optionsEmail.Value.Destinatario;
        }

        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Contactenos(ContactoViewModel model)
        {
             if (!ModelState.IsValid)
            {
                TempData["MensajeEnviado"] = "El email esta vacio o tiene algun error";
                return View(model);
            }

            try
            {
                var destinatario_ = destinatario ?? "";

                var smtpHost = host ?? "localhost";
                var smtpPort = int.TryParse(port, out var p) ? p : 25;
                var smtpUser = user;
                var smtpPass = pass;
                var smtpEnableSsl = bool.TryParse(enableSsl, out var ssl) ? ssl : false;
                var fromAddress = from ?? "no-reply@importsurgery.com";

                // --- SEGURIDAD AÑADIDA (solo agregado, no se cambió lógica existente) ---
                // 1) Sanitizar cabeceras y campos que se usarán en headers (evitar header injection)
                var cleanedEmail = SanitizeHeader(model.Email);
                var cleanedNombre = SanitizeHeader(model.Nombre);
                var cleanedTelefono = SanitizeHeader(model.Telefono);
                var cleanedFrom = SanitizeHeader(fromAddress);
                var cleanedDestinatario = SanitizeHeader(destinatario_);

                // 2) Validar formato de email antes de usarlo en ReplyTo / cabeceras
                if (!string.IsNullOrWhiteSpace(cleanedEmail) && !RegexUtilities.IsValidEmail(cleanedEmail))
                {
                    ModelState.AddModelError(nameof(model.Email), "Email inválido");
                    TempData["MensajeEnviado"] = "El email tiene formato inválido";
                    return View(model);
                }

                // 3) Limitar longitud del mensaje por seguridad (protección contra abusos)
                var rawMensaje = model.Mensaje ?? string.Empty;
                if (rawMensaje.Length > 2000)
                {
                    ModelState.AddModelError(nameof(model.Mensaje), "Mensaje demasiado largo");
                    TempData["MensajeEnviado"] = "El mensaje es demasiado largo";
                    return View(model);
                }
                // --- FIN SEGURIDAD AÑADIDA ---

                // Escape user input para evitar inyección en el HTML (mantengo tu lógica)
                var encoder = HtmlEncoder.Default;
                var nombre = encoder.Encode(cleanedNombre ?? "");
                var email = encoder.Encode(cleanedEmail ?? "");
                var telefono = encoder.Encode(cleanedTelefono ?? "");
                var mensaje = encoder.Encode(rawMensaje).Replace("\n", "<br/>");

                var textBody =
                    $@"Nombre: {model.Nombre}
                    Email: {model.Email}
                    Teléfono: {model.Telefono}

                    Mensaje:
                    {model.Mensaje}
                    ";

                // HTML body
                var htmlBody =
                        $@"<html>
                          <body>
                            <h2>Nuevo mensaje de contacto</h2>
                            <p><strong>Nombre:</strong> {nombre}</p>
                            <p><strong>Email:</strong> {email}</p>
                            <p><strong>Teléfono:</strong> {telefono}</p>
                            <hr/>
                            <p><strong>Mensaje:</strong></p>
                            <div>{mensaje}</div>
                          </body>
                        </html>";

                var message = new MimeMessage();
                // uso las versiones sanitizadas para From/To/ReplyTo
                message.From.Add(MailboxAddress.Parse(RemoveControlChars(cleanedFrom)));
                message.To.Add(MailboxAddress.Parse(RemoveControlChars(cleanedDestinatario)));
                if (!string.IsNullOrWhiteSpace(cleanedEmail))
                    message.ReplyTo.Add(MailboxAddress.Parse(RemoveControlChars(cleanedEmail)));
                // Subject seguro sin control chars
                message.Subject = RemoveControlChars($"Contacto web - {cleanedNombre}");

                var bodyBuilder = new BodyBuilder
                {
                    TextBody = textBody,
                    HtmlBody = htmlBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();

                var socketOptions = smtpEnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto;
                await client.ConnectAsync(smtpHost, smtpPort, socketOptions);

                if (!string.IsNullOrEmpty(smtpUser))
                {
                    await client.AuthenticateAsync(smtpUser, smtpPass ?? string.Empty);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                TempData["MensajeEnviado"] = "OK";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email desde Contactenos");
                TempData["MensajeEnviado"] = "ERROR";
                return View(model);
            }
        }     

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Helpers añadidos: sanitizan cadenas y eliminan caracteres de control en headers/subject.
        private static string SanitizeHeader(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // eliminar CR/LF y recortar espacios
            return input.Replace("\r", "").Replace("\n", "").Trim();
        }

        private static string RemoveControlChars(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            // elimina caracteres control (por ejemplo \u0000-\u001F y DEL)
            return System.Text.RegularExpressions.Regex.Replace(input, @"[\u0000-\u001F\u007F]+", " ").Trim();
        }
    }
}