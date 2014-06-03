using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace StudentUp.Models
{
	/// <summary>
	/// Класс ошибок вызванных неправильным заполнением полей пользователем.
	/// В основном возникает когда у пользователея отключен js и нет возможности провереить передаваемые данные на строне клиента.
	/// </summary>
	public class ValidationDataException : Exception
	{
		/// <summary>
		/// Сообщения об ошибке
		/// </summary>
		readonly Messages messages;

		/// <summary>
		/// Конструктор класса. Вызывается если ошибка одна
		/// </summary>
		/// <param name="message"></param>
		public ValidationDataException(string message) : base(message)
		{
			this.messages = new Messages {new Messages.Message(Messages.Message.TypeMessage.error, base.Message)};
		}

		/// <summary>
		/// Конструктор класса. Вызываетмся если ошибок много
		/// </summary>
		/// <param name="newMasseges"></param>
		public ValidationDataException(Messages newMasseges)
		{
			this.messages = newMasseges;
		}

		/// <summary>
		/// Возвращает все ошибки
		/// </summary>
		/// <returns>Ошибки</returns>
		public Messages GetValue()
		{
			return this.messages;
		}
	}

	/// <summary>
	/// Класс валидации
	/// </summary>
	public class Validation
	{
		/// <summary>
		/// Паттерн для почты
		/// Общий вид — логин@поддомен.домен.
		/// Логин, как и поддомен — слова из букв, цифр, подчёркиваний, дефисов и точек.
		/// А домен (имеется в виду 1го уровня) — это от 2 до 6 букв и точек.
		/// </summary>
		static string emailPattern = @"^[a-z0-9._-]+@[a-z0-9-]+\.[a-z]{2,6}$";

		/// <summary>
		/// Возвращает паттерн для почты
		/// </summary>
		public static string EmailPattern { get { return emailPattern; } }

		/// <summary>
		/// Статическая функция валидации
		/// </summary>
		/// <param name="email">Почта</param>
		/// <param name="password">Пароль</param>
		/// <returns>true - валидация удалась, false - не удалась</returns>
		public static bool Valid(string email, string password)
		{
			return IsEmail(email) && IsPassword(password);
		}

		/// <summary>
		/// Проверяет есть ли переданная строка Email
		/// </summary>
		/// <param name="email">Email</param>
		/// <returns>
		/// true - является
		/// false - не явяется
		/// </returns>
		public static bool IsEmail(string email)
		{
			return Regex.IsMatch(email, emailPattern);
		}

		/// <summary>
		/// Проверяет есть ли переданная строка пролем
		/// </summary>
		/// <param name="password">Пароль</param>
		/// <returns>
		/// true - является
		/// false - не явяется
		/// </returns>
		public static bool IsPassword(string password)
		{
			return password.Length >= 6;
		}

		/// <summary>
		/// Функция шифрует входящую строку с помощью алгоритма md5
		/// </summary>
		/// <param name="input">Строка требующая шифрования</param>
		/// <returns>Шифровананная строка</returns>
		public static string StringToMd5Hash(string input)
		{
			byte[] data = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(input));
			StringBuilder sBuilder = new StringBuilder();
			for (int i = 0; i < data.Length; i++)
			{
				sBuilder.Append(data[i].ToString("x2"));
			}
			return sBuilder.ToString();
		}

		/// <summary>
		/// Проверяет равенство обычной строки и шифрованной
		/// </summary>
		/// <param name="input">Обычная строка</param>
		/// <param name="hash">Шифрованная</param>
		/// <returns>
		/// true - строки равны
		/// false - строки не равны
		/// </returns>
		static public bool VerifyMd5Hash(string input, string hash)
		{
			string hashOfInput = StringToMd5Hash(input);
			StringComparer comparer = StringComparer.OrdinalIgnoreCase;
			return 0 == comparer.Compare(hashOfInput, hash);
		}

		/// <summary>
		/// Создает случайный пароль
		/// </summary>
		/// <param name="length">Длина пароля</param>
		/// <returns>Пароль</returns>
		static public string GeneratePassword(int length)
		{
			string result = "";
			if (length > 0)
			{
				Random rand = new Random();
				for (int i = 0; i < length; i++)
					switch (rand.Next(3))
					{
						case 0:
							result += (char)rand.Next(48, 58);
							break;
						case 1:
							result += (char)rand.Next(65, 81);
							break;
						case 2:
							result += (char)rand.Next(97, 123);
							break;
					}
			}
			return result;
		}
	}
}