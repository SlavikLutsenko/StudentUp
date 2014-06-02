using System.Web.Mvc;
using StudentUp.Models;

namespace StudentUp.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View();
        }

		/// <summary>
		/// Производит авторизацию пользователей
		/// </summary>
		/// <param name="email">Email пользователя</param>
		/// <param name="password">Пароль пользователя</param>
		/// <returns>Если пользователь зарегистрирован и ввел правильные данные то он авторизуется иначе его перенаправят на главную страницу с соответствуещим сообщением. </returns>
		[HttpPost]
		public ActionResult Index(string email, string password)
		{
			try
			{
				Users user = new Users(email, password);
				if (!user.IsLogin()) throw new ValidationDataException("no user");
				user.GetInformationAboutUserFromDB();
				user.CreateSession(Response);
				return Redirect("/home");
			}
			catch (ValidationDataException error)
			{
				Messages errorMessages = new Messages();
				foreach (Messages.Message mes in error.GetValue())
				{
					string message = "";
					switch (mes.Value)
					{
						case "no email":
							message = "Вы ввели не правильный формат email";
							break;
						case "no password":
							message = "Вы ввели не правильный формат пароля";
							break;
						case "no user":
							message =
								@"Пользователь с таким Email и паролем не зарегитсрирова или вы ввели неправильные данные.<br/> Возможно вы хотите <a href='/restorePassword'>востановить пароль</a>.";
							break;
					}
					errorMessages.Add(Messages.Message.TypeMessage.error, message);
				}
				ViewData["messages"] = errorMessages;
				return View();
			}
		}

	    public ActionResult Home()
	    {
		    return View();
	    }
    }
}
