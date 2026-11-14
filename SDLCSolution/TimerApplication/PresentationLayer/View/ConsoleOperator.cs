using System;
using System.Collections.Generic;
using TimerApplication.ApplicationLayer.Service;
using TimerApplication.Domain.Model;
using TimerApplication.PresentationLayer.Helper;

namespace TimerApplication.PresentationLayer.View
{
    public class ConsoleOperator
    {
        private readonly UserService _userService;

        public ConsoleOperator(UserService userService)
        {
            this._userService = userService;
        }

        public void Run()
        {
            TextColor.WriteColoredLine("Welcome to Time Logger", ConsoleColor.Cyan);
            int choice;
            do
            {
                Console.WriteLine("1. Signup");
                Console.WriteLine("2. Login");
                Console.WriteLine("3. Exit");
                Console.WriteLine("3. Enter Your Option (1/2/3): ");
                bool isValidChoice = InputValidator.ValidateInteger(Console.ReadLine(), out choice);
                if (!isValidChoice)
                {
                    TextColor.WriteColoredLine($"Invalid Integer format, Enter a valid integer!", ConsoleColor.Red);
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        this.SignupUser();
                        break;
                    case 2:
                        this.LoginUser();
                        break;
                    case 3:
                        TextColor.WriteColoredLine("Exiting the App, Bye!", ConsoleColor.Cyan);
                        break;
                    default:
                        TextColor.WriteColoredLine("Invalid Choice, Enter a valid choice between 1 to 3", ConsoleColor.Red);
                        break;
                }
            }
            while (choice != 3);
        }

        private void LoginUser()
        {
            throw new NotImplementedException();
        }

        private void SignupUser()
        {
            var user = this.GetUserDetails();
            if (user == null)
            {
                return;
            }

            var isCreated = this._userService.CreateUser(user);
            if (!isCreated)
            {
                TextColor.WriteColoredLine($"Signup failed, Account already registered with the given Email ID!", ConsoleColor.Red);
                return;
            }

            TextColor.WriteColoredLine("Signup Successfull!", ConsoleColor.Green);
        }

        private User GetUserDetails()
        {
            Console.Clear();
            Console.WriteLine("\x1b[3J"); 
            Console.Clear();
            var userName = this.GetUserName();
            if (userName == null)
            {
                return null;
            }

            var email = this.GetEmail();
            if (email == null)
            {
                return null;
            }

            var hashedPassword = this.GetPassword();
            if (hashedPassword == null)
            {
                return null;
            }

            return new User(Guid.NewGuid(), userName, email, hashedPassword, new List<Activity>());
        }

        private string GetUserName()
        {
            int maxAttempts = 3;
            Console.WriteLine("Enter User Name: ");
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var userName = Console.ReadLine();
                var isValidName = InputValidator.ValidateString(userName);
                if (isValidName)
                {
                    return userName;
                }

                TextColor.WriteColoredLine($"User Name should not be null or empty!", ConsoleColor.Red);
                if (attempt < maxAttempts)
                {
                    Console.WriteLine("Re-enter Email: ");
                }
            }

            TextColor.WriteColoredLine("Too many failed attempts!", ConsoleColor.Red);
            return null;
        }

        private string GetEmail()
        {
            int maxAttempts = 3;
            Console.WriteLine("Enter User Email ID: ");
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                var email = Console.ReadLine();
                var isValidEmail = InputValidator.ValidateEmail(email);
                if (isValidEmail)
                {
                    return email;
                }

                TextColor.WriteColoredLine("Invalid email format!", ConsoleColor.Red);
                if (attempt < maxAttempts)
                {
                    Console.WriteLine("Re-enter Email: ");
                }
            }

            TextColor.WriteColoredLine("Too many failed attempts!", ConsoleColor.Red);
            return null;
        }

        private string GetPassword()
        {
            int maxAttempts = 3;
            Console.WriteLine("Enter Password: ");
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                string password = "";
                while (true)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }

                    password += key.KeyChar;
                    Console.Write("*");
                }

                Console.WriteLine();
                var passwordValidation = InputValidator.ValidatePassword(password);
                if (passwordValidation.IsSuccess)
                {
                    return PasswordHasher.HashPassword(password);
                }

                TextColor.WriteColoredLine(passwordValidation.Message, ConsoleColor.Red);

                if (attempt < maxAttempts)
                {
                    Console.WriteLine("Re-enter Password: ");
                }
            }
            TextColor.WriteColoredLine("Too many failed attempts!", ConsoleColor.Red);
            return null;
        }
    }
}
