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
			Users user = new Users(email, password);
			if (!user.IsLogin()) return View();
			user.GetInformationAboutUserFromDB();
			user.CreateSession(Response);
			return Redirect("/home");
		}

	    public ActionResult Home()
	    {
		    return View();
	    }
    }
}
